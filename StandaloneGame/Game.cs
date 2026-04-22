using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text.Json;
using System.Windows.Forms;

namespace CityZero
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new GameWindow());
        }
    }

    // ── Sprite cache ──────────────────────────────────────────────────────────
    // Loads every embedded PNG once at startup into a name→Bitmap dictionary.
    // Key = filename without extension, e.g. "Police", "char_idle_down_1".
    static class Sprites
    {
        private static readonly Dictionary<string, Bitmap> _cache = new();

        static Sprites()
        {
            var asm = Assembly.GetExecutingAssembly();
            foreach (var name in asm.GetManifestResourceNames())
            {
                if (!name.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) continue;
                // resource name is like "StandaloneGame.Sprites.Police.png"
                string key = Path.GetFileNameWithoutExtension(
                    name.Replace("StandaloneGame.Sprites.", ""));
                using var stream = asm.GetManifestResourceStream(name)!;
                // Use Format32bppPArgb to keep alpha correct under DrawImage
                var raw = new Bitmap(stream);
                var bmp = new Bitmap(raw.Width, raw.Height,
                    System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                using var tmpG = Graphics.FromImage(bmp);
                tmpG.DrawImage(raw, 0, 0);
                raw.Dispose();
                _cache[key] = bmp;
            }
        }

        public static Bitmap? Get(string key) =>
            _cache.TryGetValue(key, out var b) ? b : null;

        // Draw a sprite centred on (cx, cy), scaled to fit in targetW×targetH,
        // with optional rotation about the centre.
        public static void Draw(Graphics g, string key, float cx, float cy,
                                int targetW, int targetH, float angleDeg = 0f)
        {
            var bmp = Get(key);
            if (bmp is null) return;
            var saved = g.Save();
            g.TranslateTransform(cx, cy);
            if (angleDeg != 0f) g.RotateTransform(angleDeg);
            g.DrawImage(bmp, -targetW / 2f, -targetH / 2f, targetW, targetH);
            g.Restore(saved);
        }
    }

    // ── TileSheet ─────────────────────────────────────────────────────────────
    // Slices individual tiles out of the roguelikeCity spritesheet at runtime.
    // Tile index layout: 37 columns, stride = 17px (16px tile + 1px gap).
    static class TileSheet
    {
        private const int Stride = 17;
        private const int Cols   = 37;
        private const int TW     = 16;

        private static readonly Bitmap? _sheet = Sprites.Get("roguelikeCity");
        private static readonly Dictionary<int, Bitmap> _tileCache = new();

        // Returns a 16×16 Format32bppPArgb bitmap for tile index i.
        public static Bitmap? Tile(int i)
        {
            if (_sheet is null) return null;
            if (_tileCache.TryGetValue(i, out var cached)) return cached;

            int col = i % Cols, row = i / Cols;
            int sx = col * Stride, sy = row * Stride;
            if (sx + TW > _sheet.Width || sy + TW > _sheet.Height) return null;

            var bmp = new Bitmap(TW, TW,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            using var g = Graphics.FromImage(bmp);
            g.DrawImage(_sheet,
                new Rectangle(0, 0, TW, TW),
                new Rectangle(sx, sy, TW, TW),
                GraphicsUnit.Pixel);
            _tileCache[i] = bmp;
            return bmp;
        }
    }

    // ── Siren audio ───────────────────────────────────────────────────────────
    // Loads the embedded siren.mp3 and plays it in a looping background thread
    // whenever heat ≥ 3.  Uses SoundPlayer (WAV only) via a temp WAV copy; for
    // MP3 we shell out to Media.SoundPlayer's underlying mciSendString via
    // a native helper wrapped in WMPLib-free approach: write to temp file,
    // play with Windows Media Player COM — but to keep it zero-dependency we
    // use the MCIInterop trick inline.
    static class SirenAudio
    {
        private static string? _tempPath;
        private static bool    _playing;
        private static bool    _initialized;

        private static void EnsureTemp()
        {
            if (_initialized) return;
            _initialized = true;
            try
            {
                var asm  = Assembly.GetExecutingAssembly();
                var name = asm.GetManifestResourceNames()
                              .FirstOrDefault(n => n.EndsWith("siren.mp3",
                                  StringComparison.OrdinalIgnoreCase));
                if (name is null) return;
                _tempPath = Path.Combine(Path.GetTempPath(), "cityzero_siren.mp3");
                using var src = asm.GetManifestResourceStream(name)!;
                using var dst = File.Create(_tempPath);
                src.CopyTo(dst);
            }
            catch { }
        }

        public static void Update(int heatLevel)
        {
            EnsureTemp();
            if (_tempPath is null) return;

            bool wantPlay = heatLevel >= 3;
            if (wantPlay && !_playing)
            {
                _playing = true;
                MciPlay(_tempPath);
            }
            else if (!wantPlay && _playing)
            {
                _playing = false;
                MciStop();
            }
        }

        // Thin wrapper around Windows MCI — no external DLLs, no COM.
        [System.Runtime.InteropServices.DllImport("winmm.dll")]
        private static extern int mciSendString(string cmd, System.Text.StringBuilder? ret, int retLen, IntPtr callback);

        private static void MciPlay(string path)
        {
            try
            {
                mciSendString("close cityzero_siren", null, 0, IntPtr.Zero);
                mciSendString($"open \"{path}\" type mpegvideo alias cityzero_siren", null, 0, IntPtr.Zero);
                mciSendString("play cityzero_siren repeat", null, 0, IntPtr.Zero);
            }
            catch { }
        }

        private static void MciStop()
        {
            try
            {
                mciSendString("stop cityzero_siren", null, 0, IntPtr.Zero);
                mciSendString("close cityzero_siren", null, 0, IntPtr.Zero);
            }
            catch { }
        }
    }

    // ── Faction ───────────────────────────────────────────────────────────────
    record Faction(string Id, string Name, Color FactionColor);

    // ── WorldBaker ────────────────────────────────────────────────────────────
    // Pre-renders the entire static world (ground + roads + sidewalks +
    // district ground-fill) once into a Format32bppPArgb bitmap at startup.
    // The Renderer draws this single bitmap per frame with one DrawImage call.
    static class WorldBaker
    {
        // Tile size we render each 16×16 Kenney tile at (world-pixels).
        // 16 fits 37.5 tiles per RoadStep-160, giving a nice look at zoom 0.28.
        public const int TSize = 16;

        private static Bitmap? _world;

        public static Bitmap World => _world ??= Bake();

        private static Bitmap Bake()
        {
            int wW   = GameState.DistrictSize * 3;
            int wH   = GameState.DistrictSize * 2;
            int step = GameState.RoadStep;
            int hw   = GameState.RoadHalfW;
            int sw   = 10;  // sidewalk width in world-px

            // roguelikeCity tile indices (identified by colour sampling)
            // Ground per district (GridX + GridY*3 index)
            int[] groundIdx = { 32, 35, 36, 33, 34, 142 };
            // Pavement: tile 016 = light grey stone (rgb 166,169,174)
            int pavIdx   = 16;
            // Asphalt:  tile 713 = dark road (rgb 64,64,64)
            int roadIdx  = 713;
            // Intersection: tile 750 (slightly different asphalt patch)
            int interIdx = 750;

            var bmp = new Bitmap(wW, wH,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            using var g = Graphics.FromImage(bmp);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode   = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            g.SmoothingMode     = SmoothingMode.None;

            // 1. Ground — each district gets its own tile variant
            foreach (var d in GameState.Districts)
            {
                int ox  = d.GridX * GameState.DistrictSize;
                int oy  = d.GridY * GameState.DistrictSize;
                int idx = d.GridX + d.GridY * 3;
                var gt  = TileSheet.Tile(groundIdx[Math.Clamp(idx, 0, groundIdx.Length - 1)]);
                if (gt is null)
                {
                    using var fb = new SolidBrush(d.Ground);
                    g.FillRectangle(fb, ox, oy, GameState.DistrictSize, GameState.DistrictSize);
                    continue;
                }
                for (int tx = ox; tx < ox + GameState.DistrictSize; tx += TSize)
                for (int ty = oy; ty < oy + GameState.DistrictSize; ty += TSize)
                    g.DrawImage(gt, tx, ty, TSize, TSize);
            }

            // 2. Sidewalk strips
            var pav = TileSheet.Tile(pavIdx);
            if (pav is not null)
            {
                for (int x = step; x < wW; x += step)
                {
                    for (int ty = 0; ty < wH; ty += TSize)
                    {
                        g.DrawImage(pav, x - hw - sw, ty, sw, TSize);
                        g.DrawImage(pav, x + hw,       ty, sw, TSize);
                    }
                }
                for (int y = step; y < wH; y += step)
                {
                    for (int tx = 0; tx < wW; tx += TSize)
                    {
                        g.DrawImage(pav, tx, y - hw - sw, TSize, sw);
                        g.DrawImage(pav, tx, y + hw,      TSize, sw);
                    }
                }
            }

            // 3. Road asphalt
            var road = TileSheet.Tile(roadIdx);
            if (road is not null)
            {
                for (int x = step; x < wW; x += step)
                    for (int ty = 0; ty < wH; ty += TSize)
                        g.DrawImage(road, x - hw, ty, hw * 2, TSize);
                for (int y = step; y < wH; y += step)
                    for (int tx = 0; tx < wW; tx += TSize)
                        g.DrawImage(road, tx, y - hw, TSize, hw * 2);
            }

            // 4. Intersections
            var inter = TileSheet.Tile(interIdx) ?? road;
            if (inter is not null)
                for (int x = step; x < wW; x += step)
                for (int y = step; y < wH; y += step)
                    g.DrawImage(inter, x - hw, y - hw, hw * 2, hw * 2);

            // 5. Centre-line dashes
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var markPen = new Pen(Color.FromArgb(130, 220, 190, 50), 2f)
            {
                DashStyle   = DashStyle.Custom,
                DashPattern = new float[] { 8f, 12f },
            };
            for (int x = step; x < wW; x += step) g.DrawLine(markPen, x, 0, x, wH);
            for (int y = step; y < wH; y += step) g.DrawLine(markPen, 0, y, wW, y);

            // 6. District accent borders
            foreach (var d in GameState.Districts)
            {
                int wx = d.GridX * GameState.DistrictSize;
                int wy = d.GridY * GameState.DistrictSize;
                using var pen = new Pen(Color.FromArgb(70, d.Accent), 3f);
                g.DrawRectangle(pen, wx, wy, GameState.DistrictSize, GameState.DistrictSize);
            }

            return bmp;
        }
    }

    // ── NPC vehicle ───────────────────────────────────────────────────────────
    class NpcVehicle
    {
        public PointF Pos;
        public float  Angle;
        public float  Speed;
        public bool   IsPolice;
        public Color  BodyColor;
        public string SpriteKey;
        public NpcVehicle(PointF pos, float angle, float speed, bool police, Color color, string sprite)
        { Pos = pos; Angle = angle; Speed = speed; IsPolice = police; BodyColor = color; SpriteKey = sprite; }
    }

    // ── NPC pedestrian ────────────────────────────────────────────────────────
    class NpcPed
    {
        public PointF Pos;
        public float  Angle;         // heading in degrees (0=up)
        public float  Speed;
        public int    WalkFrame;     // 0-7
        public float  FrameTimer;
        public bool   FlipH;
        public NpcPed(PointF pos, float angle, float speed)
        { Pos = pos; Angle = angle; Speed = speed; }
    }

    // ── Mission phase ─────────────────────────────────────────────────────────
    enum MissionPhase { Inactive, Pickup, Deliver, EscapeHeat, Complete, Failed }

    // ── Game screen ──────────────────────────────────────────────────────────
    enum GameScreen { Title, Playing, Paused, Dead }

    // ── District ─────────────────────────────────────────────────────────────
    record District(string Id, string Name, Color Ground, Color Accent, int GridX, int GridY);

    // ── Main window ───────────────────────────────────────────────────────────
    class GameWindow : Form
    {
        private readonly GameLoop      _loop;
        private readonly GameState     _state;
        private readonly Renderer      _renderer;
        private readonly HashSet<Keys> _keys    = new();
        private readonly HashSet<Keys> _pressed = new();

        private GameScreen _screen = GameScreen.Title;

        public GameWindow()
        {
            Text            = "CITY//ZERO";
            ClientSize      = new Size(1280, 720);
            DoubleBuffered  = true;
            BackColor       = Color.Black;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;
            StartPosition   = FormStartPosition.CenterScreen;

            _state    = new GameState();
            _renderer = new Renderer(ClientSize);

            _loop = new GameLoop(16, () =>
            {
                switch (_screen)
                {
                    case GameScreen.Playing:
                        _state.Update(_keys, _pressed);
                        if (_state.Health <= 0) _screen = GameScreen.Dead;
                        break;
                    case GameScreen.Title:
                    case GameScreen.Dead:
                        if (_pressed.Contains(Keys.Enter) || _pressed.Contains(Keys.Space))
                        {
                            if (_screen == GameScreen.Dead) _state.Respawn();
                            _screen = GameScreen.Playing;
                        }
                        break;
                    case GameScreen.Paused:
                        if (_pressed.Contains(Keys.Escape) || _pressed.Contains(Keys.P))
                            _screen = GameScreen.Playing;
                        break;
                }
                if (_screen == GameScreen.Playing &&
                    (_pressed.Contains(Keys.Escape) || _pressed.Contains(Keys.P)))
                    _screen = GameScreen.Paused;
                _pressed.Clear();
                Invalidate();
            });

            KeyDown    += (_, e) => { _keys.Add(e.KeyCode); _pressed.Add(e.KeyCode); };
            KeyUp      += (_, e) =>   _keys.Remove(e.KeyCode);
            FormClosed += (_, _) =>   _loop.Stop();

            _loop.Start();
        }

        protected override void OnPaint(PaintEventArgs e) =>
            _renderer.Draw(e.Graphics, _state, _screen);
    }

    // ── Game state ────────────────────────────────────────────────────────────
    class GameState
    {
        // ── World constants ───────────────────────────────────────────────────
        public const int   DistrictSize = 600;
        public const int   RoadStep     = 160;  // spacing between road centres
        public const int   RoadHalfW    =  14;  // half road width (world-px)
        public const float MoveSpeed    = 220f;

        // ── Six lore districts (3 × 2 grid) ──────────────────────────────────
        public static readonly District[] Districts =
        {
            new("old_quarter",    "Old Quarter",    Color.FromArgb(110, 80,  55), Color.FromArgb(255,200,100), 0, 0),
            new("glass_heights",  "Glass Heights",  Color.FromArgb( 50, 80, 125), Color.FromArgb(180,220,255), 1, 0),
            new("ash_industrial", "Ash Industrial", Color.FromArgb( 68, 63,  58), Color.FromArgb(200,160, 80), 2, 0),
            new("iron_docks",     "Iron Docks",     Color.FromArgb( 45, 82,  82), Color.FromArgb(255,140, 60), 0, 1),
            new("the_spire",      "The Spire",      Color.FromArgb( 52, 48,  68), Color.FromArgb(220, 40, 60), 1, 1),
            new("neon_flats",     "Neon Flats",     Color.FromArgb( 14, 11,  24), Color.FromArgb(160, 20,255), 2, 1),
        };

        // ── Five lore factions ────────────────────────────────────────────────
        public static readonly Faction[] Factions =
        {
            new("blue_saints",       "Blue Saints",       Color.FromArgb( 60,120,220)),
            new("razor_union",       "Razor Union",       Color.FromArgb(200, 60, 40)),
            new("velvet_circuit",    "Velvet Circuit",    Color.FromArgb(180, 40,180)),
            new("cinder_mob",        "Cinder Mob",        Color.FromArgb(220,120, 20)),
            new("helix_directorate", "Helix Directorate", Color.FromArgb( 40,200,160)),
        };

        // ── Player ────────────────────────────────────────────────────────────
        public PointF PlayerPos   = new(DistrictSize / 2f, DistrictSize / 2f);
        public float  PlayerAngle = 0f;
        public int    Health      = 100;
        public int    Armor       = 50;
        public int    Cash        = 800;

        // ── Weapons ───────────────────────────────────────────────────────────
        public Weapon?          EquippedWeapon  = GameData.GetWeapon("pipe_hook");
        public HashSet<string>  OwnedWeaponIds  = new() { "pipe_hook" };
        public int              WeaponAmmo      = 0;
        public float            ReloadTimer     = 0f;
        public float            FireTimer       = 0f;

        // ── Heat ──────────────────────────────────────────────────────────────
        public int   HeatLevel = 0;
        public float HeatScore = 0f;

        // ── Reputation (keyed by faction id) ─────────────────────────────────
        public Dictionary<string, int> Reputation = new()
        {
            ["blue_saints"]       = 0,
            ["razor_union"]       = 0,
            ["velvet_circuit"]    = 0,
            ["cinder_mob"]        = 0,
            ["helix_directorate"] = 0,
        };

        // ── Mission chain (data-driven via GameData) ──────────────────────────
        public MissionDef?     ActiveMission    = GameData.GetMission("mq_a1_01_first_contact");
        public MissionPhase    MissionPhase     = MissionPhase.Inactive;
        public float           MissionTimer     = 0f;
        public bool            MissionAvailable = true;
        public HashSet<string> CompletedMissions = new();

        public string MissionTitle     => ActiveMission?.Title     ?? "";
        public string MissionObjective = "Find the pickup marker.  [E near marker]";
        public PointF PickupPos        => ActiveMission?.PickupPos  ?? new(80, 80);
        public PointF DeliveryPos      => ActiveMission?.DeliverPos ?? new(520, 520);

        // ── World markers (safehouse / garage / shop) ─────────────────────────
        public static readonly WorldMarker[] Markers = GameData.Markers;

        // ── World events ──────────────────────────────────────────────────────
        public readonly WorldEvent[] WorldEvents   = GameData.Events;
        public string  ActiveEventText             = "";
        private float  _eventSpawnTimer            = 60f;

        // ── NPC vehicles ──────────────────────────────────────────────────────
        public readonly List<NpcVehicle> Npcs = new();
        public readonly List<NpcPed>     Peds = new();
        private readonly Random _rng          = new(99);
        private float _trafficTimer           = 0f;
        private float _policeSpawnTimer       = 0f;
        private float _pedSpawnTimer          = 0f;
        public  int   AnimTick                = 0;
        public  bool  IsMoving                = false;

        // ── Day / night ───────────────────────────────────────────────────────
        public float WorldTime = 480f;          // minutes; 0-1440 day cycle
        public const float DayLength = 1440f;

        // ── Muzzle flash ──────────────────────────────────────────────────────
        public float MuzzleFlashTimer = 0f;     // > 0 = draw flash

        // ── Interaction prompt ────────────────────────────────────────────────
        public bool   ShowPrompt  = false;
        public string PromptLabel = "";

        // ── Camera ────────────────────────────────────────────────────────────
        public float CameraZoom = 0.28f;

        // ── FPS ───────────────────────────────────────────────────────────────
        public int       FPS;
        private int      _frames;
        private DateTime _fpsTimer = DateTime.Now;

        // ─────────────────────────────────────────────────────────────────────
        public void Update(HashSet<Keys> keys, HashSet<Keys> pressed)
        {
            const float dt = 0.016f;

            // Movement
            float dx = 0, dy = 0;
            if (keys.Contains(Keys.W) || keys.Contains(Keys.Up))    dy -= MoveSpeed * dt;
            if (keys.Contains(Keys.S) || keys.Contains(Keys.Down))  dy += MoveSpeed * dt;
            if (keys.Contains(Keys.A) || keys.Contains(Keys.Left))  dx -= MoveSpeed * dt;
            if (keys.Contains(Keys.D) || keys.Contains(Keys.Right)) dx += MoveSpeed * dt;

            if (dx != 0 || dy != 0)
            {
                float wW = DistrictSize * 3f, wH = DistrictSize * 2f;
                PlayerPos = new PointF(
                    Math.Clamp(PlayerPos.X + dx, 0, wW - 1),
                    Math.Clamp(PlayerPos.Y + dy, 0, wH - 1));
                PlayerAngle = (float)(Math.Atan2(dy, dx) * 180.0 / Math.PI) + 90f;
                // Heat gain scaled by district heatMultiplier
                var distData = GameData.GetDistrict(CurrentDistrict()?.Id ?? "");
                float heatMul = distData?.HeatMultiplier ?? 1.0f;
                HeatScore   = Math.Min(HeatScore + 0.25f * dt * heatMul, 100f);
                IsMoving    = true;
            }
            else
            {
                // Heat decays 3×faster if player is off-road (hiding)
                float decayMul = IsOnRoad() ? 1f : 3f;
                HeatScore = Math.Max(HeatScore - 3f * dt * decayMul, 0f);
                IsMoving  = false;
            }

            if (MuzzleFlashTimer > 0) MuzzleFlashTimer -= dt;

            HeatLevel = HeatScore switch
            {
                >= 80 => 4,
                >= 60 => 3,
                >= 40 => 2,
                >= 20 => 1,
                _     => 0,
            };

            // Zoom
            if (keys.Contains(Keys.Oemplus)  || keys.Contains(Keys.Add))
                CameraZoom = Math.Min(CameraZoom + 0.01f, 1.2f);
            if (keys.Contains(Keys.OemMinus) || keys.Contains(Keys.Subtract))
                CameraZoom = Math.Max(CameraZoom - 0.01f, 0.12f);

            UpdateMission(pressed);
            UpdateNpcs(dt);
            UpdatePeds(dt);
            UpdateWorldEvents(dt);
            UpdateWeapon(dt, pressed);
            SirenAudio.Update(HeatLevel);
            AnimTick++;

            // Save on F5, load on F9, delete on F12
            if (pressed.Contains(Keys.F5))  SaveManager.Save(this);
            if (pressed.Contains(Keys.F9))  SaveManager.Load(this);
            if (pressed.Contains(Keys.F12)) SaveManager.Delete();

            // Cycle weapon on Q
            if (pressed.Contains(Keys.Q)) CycleWeapon();

            // Day/night at 10× real-time
            WorldTime = (WorldTime + dt * 10f) % DayLength;

            // FPS
            _frames++;
            if ((DateTime.Now - _fpsTimer).TotalSeconds >= 1.0)
            {
                FPS = _frames; _frames = 0; _fpsTimer = DateTime.Now;
            }
        }

        // ── Mission state machine ─────────────────────────────────────────────
        private void UpdateMission(HashSet<Keys> pressed)
        {
            ShowPrompt = false; PromptLabel = "";
            const float reach = 55f;

            // ── World marker interactions ─────────────────────────────────────
            foreach (var m in Markers)
            {
                if (Dist(PlayerPos, m.Pos) >= reach) continue;
                switch (m.Type)
                {
                    case MarkerType.Safehouse:
                        ShowPrompt  = true;
                        PromptLabel = $"[E] {m.DisplayName} — Rest & Save";
                        if (pressed.Contains(Keys.E))
                        {
                            Health    = Math.Min(Health + 30, 100);
                            Armor     = Math.Min(Armor  + 20, 100);
                            HeatScore = Math.Max(HeatScore - 40f, 0f);
                            SaveManager.Save(this);
                        }
                        return;
                    case MarkerType.Garage:
                        ShowPrompt  = true;
                        PromptLabel = $"[E] {m.DisplayName} — Repair ($200)";
                        if (pressed.Contains(Keys.E) && Cash >= 200)
                        { Cash -= 200; Health = 100; Armor = 100; }
                        return;
                    case MarkerType.Shop:
                    {
                        // Find the cheapest weapon the player doesn't own
                        var buyable = GameData.Weapons
                            .Where(w => !OwnedWeaponIds.Contains(w.Id) && w.BuyPrice > 0)
                            .OrderBy(w => w.BuyPrice)
                            .FirstOrDefault();
                        if (buyable is null)
                        {
                            ShowPrompt  = true;
                            PromptLabel = $"[E] {m.DisplayName} — All weapons owned!";
                        }
                        else
                        {
                            ShowPrompt  = true;
                            PromptLabel = $"[E] {m.DisplayName} — Buy {buyable.DisplayName} (${buyable.BuyPrice})";
                            if (pressed.Contains(Keys.E) && Cash >= buyable.BuyPrice)
                            {
                                Cash -= buyable.BuyPrice;
                                OwnedWeaponIds.Add(buyable.Id);
                                EquippedWeapon = buyable;
                                WeaponAmmo     = buyable.MagazineSize;
                            }
                        }
                        return;
                    }
                }
            }

            if (ActiveMission is null) return;

            switch (MissionPhase)
            {
                case MissionPhase.Inactive when MissionAvailable:
                    if (Dist(PlayerPos, PickupPos) < reach)
                    {
                        ShowPrompt  = true;
                        PromptLabel = $"[E] Start: {ActiveMission.Title}";
                        if (pressed.Contains(Keys.E))
                        {
                            MissionPhase     = MissionPhase.Pickup;
                            MissionObjective = "Pick up the package at the marker.";
                        }
                    }
                    break;

                case MissionPhase.Pickup:
                    if (Dist(PlayerPos, PickupPos) < reach)
                    {
                        ShowPrompt  = true;
                        PromptLabel = "[E] Grab the package";
                        if (pressed.Contains(Keys.E))
                        {
                            MissionPhase     = MissionPhase.Deliver;
                            MissionObjective = ActiveMission.TimeLimit > 0
                                ? $"Deliver before time runs out. ({(int)ActiveMission.TimeLimit}s)"
                                : "Deliver to the drop point.";
                            MissionTimer = ActiveMission.TimeLimit;
                        }
                    }
                    break;

                case MissionPhase.Deliver:
                    if (ActiveMission.TimeLimit > 0)
                    {
                        MissionTimer -= 0.016f;
                        if (MissionTimer <= 0)
                        {
                            MissionPhase     = MissionPhase.Failed;
                            MissionObjective = "Mission failed — time ran out.  [R] Retry";
                            break;
                        }
                    }
                    if (Dist(PlayerPos, DeliveryPos) < reach)
                    {
                        ShowPrompt  = true;
                        PromptLabel = "[E] Drop the package";
                        if (pressed.Contains(Keys.E))
                        {
                            MissionPhase = MissionPhase.EscapeHeat;
                            MissionObjective = "Lay low — lose all heat stars.";
                            HeatScore    = Math.Max(HeatScore, ActiveMission.HeatInjected);
                        }
                    }
                    break;

                case MissionPhase.EscapeHeat:
                    if (HeatLevel == 0)
                    {
                        Cash += ActiveMission.CashReward;
                        foreach (var (fid, delta) in ActiveMission.RepRewards)
                            if (Reputation.ContainsKey(fid))
                                Reputation[fid] = Math.Clamp(Reputation[fid] + delta, -100, 100);
                        CompletedMissions.Add(ActiveMission.Id);
                        string next = ActiveMission.NextMissionId;
                        MissionObjective = $"Complete! +${ActiveMission.CashReward}  [R] Next";
                        MissionPhase = MissionPhase.Complete;
                        if (next != "")
                        {
                            var nextDef = GameData.GetMission(next);
                            if (nextDef is not null) { ActiveMission = nextDef; MissionAvailable = true; }
                        }
                        else MissionAvailable = false;
                    }
                    break;

                case MissionPhase.Complete:
                case MissionPhase.Failed:
                    if (pressed.Contains(Keys.R))
                    {
                        bool wasFailed = MissionPhase == MissionPhase.Failed;
                        MissionPhase     = MissionPhase.Inactive;
                        MissionObjective = "Find the pickup marker.  [E near marker]";
                        MissionTimer     = 0f;
                        MissionAvailable = true;
                        if (wasFailed) HeatScore = 0f;
                    }
                    break;
            }
        }

        // ── NPC traffic + police ──────────────────────────────────────────────
        private void UpdateNpcs(float dt)
        {
            // Max traffic count driven by current district density
            var dd       = GameData.GetDistrict(CurrentDistrict()?.Id ?? "");
            float dens   = dd is null ? 0.7f : (IsNight() ? dd.CivilianDensityNight : dd.TrafficDensityDay);
            int   maxCiv = Math.Max(4, (int)(dens * 20f));
            int   maxPol = dd is null ? 0 : (int)(dd.PolicePatrolIntensity * (HeatLevel + 1) * 3f);

            _trafficTimer -= dt;
            if (_trafficTimer <= 0 && Npcs.Count(n => !n.IsPolice) < maxCiv)
            {
                _trafficTimer = 1.5f + (float)_rng.NextDouble() * 2f;
                SpawnTraffic();
            }

            _policeSpawnTimer -= dt;
            if (_policeSpawnTimer <= 0 && HeatLevel >= 2 &&
                Npcs.Count(n => n.IsPolice) < Math.Min(maxPol, HeatLevel * 2))
            {
                _policeSpawnTimer = 5f;
                SpawnPolice();
            }

            if (HeatLevel < 2) Npcs.RemoveAll(n => n.IsPolice);

            foreach (var npc in Npcs)
            {
                if (npc.IsPolice)
                {
                    float tx  = PlayerPos.X - npc.Pos.X;
                    float ty  = PlayerPos.Y - npc.Pos.Y;
                    float len = MathF.Sqrt(tx * tx + ty * ty);
                    if (len > 1f)
                    {
                        npc.Angle = (float)(Math.Atan2(ty, tx) * 180.0 / Math.PI) + 90f;
                        npc.Pos   = new PointF(npc.Pos.X + tx / len * npc.Speed * dt,
                                               npc.Pos.Y + ty / len * npc.Speed * dt);
                        if (len < 90f)
                        {
                            HeatScore = Math.Min(HeatScore + 10f * dt, 100f);
                            // Police deal 6 dmg/s to player on contact (< 30px)
                            if (len < 30f)
                            {
                                int dmg = Armor > 0
                                    ? (int)(6f * dt)  // absorbed by armor first
                                    : (int)(8f * dt);
                                if (Armor > 0) Armor  = Math.Max(0, Armor  - dmg);
                                else           Health = Math.Max(0, Health - dmg);
                            }
                        }
                    }
                }
                else
                {
                    float rad = (npc.Angle - 90f) * MathF.PI / 180f;
                    npc.Pos = new PointF(npc.Pos.X + MathF.Cos(rad) * npc.Speed * dt,
                                         npc.Pos.Y + MathF.Sin(rad) * npc.Speed * dt);
                }
            }

            float wW = DistrictSize * 3f + 150, wH = DistrictSize * 2f + 150;
            Npcs.RemoveAll(n => !n.IsPolice &&
                (n.Pos.X < -150 || n.Pos.X > wW || n.Pos.Y < -150 || n.Pos.Y > wH));
        }

        // Civilian sprite keys matching extracted PNGs
        private static readonly string[] CivSprites =
            { "Audi", "Car", "Mini_truck", "Mini_van", "taxi", "truck", "Black_viper", "Ambulance" };

        private void SpawnTraffic()
        {
            float speed  = 75f + (float)_rng.NextDouble() * 65f;
            string sprite = CivSprites[_rng.Next(CivSprites.Length)];

            int wW = DistrictSize * 3, wH = DistrictSize * 2;
            // Pick a random road and lane offset (-6 or +6 from centre = two lanes)
            bool vertical = _rng.Next(2) == 0;
            float lane    = (_rng.Next(2) == 0 ? -6f : 6f);

            PointF pos; float angle;
            if (vertical)
            {
                int roadX = (1 + _rng.Next(wW / RoadStep - 1)) * RoadStep;
                if (_rng.Next(2) == 0)
                { pos = new(roadX + lane, -30f);  angle = 180f; }   // south-bound
                else
                { pos = new(roadX - lane,  wH + 30f); angle =   0f; }  // north-bound
            }
            else
            {
                int roadY = (1 + _rng.Next(wH / RoadStep - 1)) * RoadStep;
                if (_rng.Next(2) == 0)
                { pos = new(-30f,        roadY + lane); angle =  90f; } // east-bound
                else
                { pos = new(wW + 30f,    roadY - lane); angle = 270f; } // west-bound
            }
            Npcs.Add(new NpcVehicle(pos, angle, speed, false, Color.White, sprite));
        }

        private void SpawnPolice()
        {
            float a   = (float)(_rng.NextDouble() * Math.PI * 2);
            float d   = 320f + (float)_rng.NextDouble() * 180f;
            var   pos = new PointF(
                Math.Clamp(PlayerPos.X + MathF.Cos(a) * d, 0, DistrictSize * 3 - 1),
                Math.Clamp(PlayerPos.Y + MathF.Sin(a) * d, 0, DistrictSize * 2 - 1));
            Npcs.Add(new NpcVehicle(pos, 0f, 175f, true, Color.White, "Police"));
        }

        // ── World events ──────────────────────────────────────────────────────
        private void UpdateWorldEvents(float dt)
        {
            _eventSpawnTimer -= dt;
            if (_eventSpawnTimer <= 0)
            {
                _eventSpawnTimer = 45f + (float)_rng.NextDouble() * 90f;
                var dist = CurrentDistrict();
                if (dist != null)
                {
                    var eligible = WorldEvents.Where(e => e.State == WorldEventState.Idle).ToArray();
                    if (eligible.Length > 0)
                        eligible[_rng.Next(eligible.Length)].TryTrigger(dist.Id, _rng);
                }
            }
            ActiveEventText = "";
            foreach (var ev in WorldEvents)
            {
                string? txt = ev.Tick(dt);
                if (txt != null)
                {
                    ActiveEventText = txt;
                    HeatScore = Math.Min(HeatScore + ev.HeatImpact * dt, 100f);
                    if (ev.CashBonus > 0 && ev.State == WorldEventState.Cooldown)
                    {
                        var d = CurrentDistrict();
                        if (d != null && Array.IndexOf(ev.Districts, d.Id) >= 0)
                            Cash += ev.CashBonus;
                    }
                }
            }
        }

        // ── Weapon system ─────────────────────────────────────────────────────
        private void UpdateWeapon(float dt, HashSet<Keys> pressed)
        {
            if (ReloadTimer > 0) { ReloadTimer -= dt; return; }
            if (FireTimer   > 0) FireTimer -= dt;

            bool fireKey = pressed.Contains(Keys.Space) || pressed.Contains(Keys.LControlKey);
            if (fireKey && FireTimer <= 0 && EquippedWeapon != null)
            {
                if (EquippedWeapon.Category == "melee")
                {
                    HeatScore = Math.Min(HeatScore + EquippedWeapon.HeatNoise * 5f, 100f);
                    FireTimer = 1f / EquippedWeapon.FireRate;
                }
                else if (WeaponAmmo > 0)
                {
                    WeaponAmmo--;
                    HeatScore       = Math.Min(HeatScore + EquippedWeapon.HeatNoise * 3f, 100f);
                    FireTimer       = 1f / EquippedWeapon.FireRate;
                    MuzzleFlashTimer = 0.06f;
                }
                else
                {
                    ReloadTimer = EquippedWeapon.ReloadTime;
                    WeaponAmmo  = EquippedWeapon.MagazineSize;
                }
            }
        }

        private void CycleWeapon()
        {
            var owned = OwnedWeaponIds.ToArray();
            if (owned.Length <= 1) return;
            int cur  = Array.IndexOf(owned, EquippedWeapon?.Id ?? "");
            int next = (cur + 1) % owned.Length;
            EquippedWeapon = GameData.GetWeapon(owned[next]);
            if (EquippedWeapon != null) WeaponAmmo = EquippedWeapon.MagazineSize;
        }

        // ── NPC pedestrians ───────────────────────────────────────────────────
        private void UpdatePeds(float dt)
        {
            var  dd     = GameData.GetDistrict(CurrentDistrict()?.Id ?? "");
            float dens  = dd is null ? 0.7f : (IsNight() ? dd.CivilianDensityNight : dd.CivilianDensityDay);
            int   maxP  = Math.Max(4, (int)(dens * 25f));

            _pedSpawnTimer -= dt;
            if (_pedSpawnTimer <= 0 && Peds.Count < maxP)
            {
                _pedSpawnTimer = 2f + (float)_rng.NextDouble() * 3f;
                SpawnPed();
            }

            float wW = DistrictSize * 3f, wH = DistrictSize * 2f;
            foreach (var p in Peds)
            {
                float rad = (p.Angle - 90f) * MathF.PI / 180f;
                p.Pos = new PointF(
                    p.Pos.X + MathF.Cos(rad) * p.Speed * dt,
                    p.Pos.Y + MathF.Sin(rad) * p.Speed * dt);

                // Wander: small random turn every ~3 s
                p.FrameTimer += dt;
                if (p.FrameTimer > 3f + (float)_rng.NextDouble() * 2f)
                {
                    p.FrameTimer = 0f;
                    p.Angle += -30f + (float)_rng.NextDouble() * 60f;
                }
                p.WalkFrame = (int)(AnimTick / 6) % 8;
            }
            Peds.RemoveAll(p => p.Pos.X < -20 || p.Pos.X > wW + 20
                             || p.Pos.Y < -20 || p.Pos.Y > wH + 20);
        }

        private void SpawnPed()
        {
            int step = RoadStep;
            // Place on sidewalk next to a random road
            int rx = _rng.Next(1, (DistrictSize * 3) / step) * step;
            int ry = _rng.Next(1, (DistrictSize * 2) / step) * step;
            bool horiz = _rng.Next(2) == 0;
            PointF pos = horiz
                ? new(rx + (float)_rng.NextDouble() * 40 - 20, ry + RoadHalfW + 5)
                : new(rx + RoadHalfW + 5, ry + (float)_rng.NextDouble() * 40 - 20);
            float angle  = (float)(_rng.NextDouble() * 360f);
            float speed  = 28f + (float)_rng.NextDouble() * 20f;
            Peds.Add(new NpcPed(pos, angle, speed));
        }
        public static float Dist(PointF a, PointF b)
        {
            float dx = a.X - b.X, dy = a.Y - b.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        public District? CurrentDistrict()
        {
            foreach (var d in Districts)
            {
                float x = d.GridX * DistrictSize, y = d.GridY * DistrictSize;
                if (PlayerPos.X >= x && PlayerPos.X < x + DistrictSize &&
                    PlayerPos.Y >= y && PlayerPos.Y < y + DistrictSize)
                    return d;
            }
            return null;
        }

        public Color SkyColor()
        {
            float t = WorldTime / DayLength;
            if (t < 0.20f || t >= 0.87f)  return Color.FromArgb(8, 6, 18);
            if (t < 0.30f) return LerpColor(Color.FromArgb(8,6,18),    Color.FromArgb(255,120,40), (t-0.20f)/0.10f);
            if (t < 0.36f) return LerpColor(Color.FromArgb(255,120,40),Color.FromArgb(28,48,78),   (t-0.30f)/0.06f);
            if (t < 0.70f) return Color.FromArgb(28, 48, 78);
            if (t < 0.80f) return LerpColor(Color.FromArgb(28,48,78),  Color.FromArgb(200,80,20),  (t-0.70f)/0.10f);
            return LerpColor(Color.FromArgb(200,80,20), Color.FromArgb(8,6,18), Math.Min((t-0.80f)/0.07f,1f));
        }

        public static Color LerpColor(Color a, Color b, float t) => Color.FromArgb(
            Math.Clamp((int)(a.R+(b.R-a.R)*t),0,255),
            Math.Clamp((int)(a.G+(b.G-a.G)*t),0,255),
            Math.Clamp((int)(a.B+(b.B-a.B)*t),0,255));

        public bool IsNight()
        {
            // roughly 18:00–06:00
            float h = (WorldTime / 60f) % 24f;
            return h >= 18f || h < 6f;
        }

        public string TimeString()
        {
            int h = (int)(WorldTime / 60) % 24, m = (int)(WorldTime % 60);
            return $"{h:D2}:{m:D2}";
        }

        // Respawn at Old Quarter safehouse
        public void Respawn()
        {
            Health    = 100; Armor = 0; HeatScore = 0f;
            PlayerPos = new PointF(55f, 55f);
            Npcs.RemoveAll(n => n.IsPolice);
            SirenAudio.Update(0);
        }

        // 0 = noon bright, 1 = full dark midnight
        public float NightAlpha()
        {
            float t = WorldTime / DayLength;   // 0–1
            // Dusk 0.70–0.80, Night 0.87–0.20, Dawn 0.20–0.30
            if (t >= 0.30f && t < 0.70f) return 0f;         // full day
            if (t >= 0.70f && t < 0.80f) return (t - 0.70f) / 0.10f;   // darkening
            if (t >= 0.80f || t < 0.20f) return 1f;         // full night
            return 1f - (t - 0.20f) / 0.10f;                // brightening
        }

        // True when player is roughly on a road (within RoadHalfW + sidewalk of a road centre)
        public bool IsOnRoad()
        {
            int step = RoadStep, hw = RoadHalfW + 12;
            int wW = DistrictSize * 3, wH = DistrictSize * 2;
            for (int x = step; x < wW; x += step)
                if (MathF.Abs(PlayerPos.X - x) < hw) return true;
            for (int y = step; y < wH; y += step)
                if (MathF.Abs(PlayerPos.Y - y) < hw) return true;
            return false;
        }
    }   // end GameState

    // ── Renderer ──────────────────────────────────────────────────────────────
    class Renderer
    {
        private readonly Size _screen;

        private readonly Font _hudFont   = new("Consolas", 11, FontStyle.Bold);
        private readonly Font _smallFont = new("Consolas",  9, FontStyle.Regular);
        private readonly Font _tinyFont  = new("Consolas",  7, FontStyle.Regular);
        private readonly Font _labelFont = new("Consolas", 13, FontStyle.Bold);

        private readonly Color[] _heatColors =
        {
            Color.FromArgb(  0,210,  0),
            Color.FromArgb(220,220,  0),
            Color.FromArgb(240,150,  0),
            Color.FromArgb(220, 60,  0),
            Color.FromArgb(200,  0,  0),
        };

        private int _tick;
        private bool Blink => (_tick / 18) % 2 == 0;

        public Renderer(Size screen) => _screen = screen;

        // ── Master draw ───────────────────────────────────────────────────────
        public void Draw(Graphics g, GameState s, GameScreen screen)
        {
            _tick++;

            if (screen == GameScreen.Title)
            { DrawTitleScreen(g); return; }

            g.Clear(s.SkyColor());
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float zoom = s.CameraZoom;
            float cx   = _screen.Width  / 2f - s.PlayerPos.X * zoom;
            float cy   = _screen.Height / 2f - s.PlayerPos.Y * zoom;

            g.TranslateTransform(cx, cy);
            g.ScaleTransform(zoom, zoom);

            DrawDistricts(g, s);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(WorldBaker.World, 0, 0);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            DrawBuildings(g, s);
            DrawPeds(g, s);
            DrawMissionMarkers(g, s);
            DrawNpcs(g, s);
            DrawPlayer(g, s);
            if (s.MuzzleFlashTimer > 0) DrawMuzzleFlash(g, s);

            g.ResetTransform();

            // Night darkness overlay
            float na = s.NightAlpha();
            if (na > 0.01f)
            {
                using var nb = new SolidBrush(Color.FromArgb((int)(na * 140), 5, 5, 18));
                g.FillRectangle(nb, 0, 0, _screen.Width, _screen.Height);
            }

            DrawVignette(g, s);
            DrawHUD(g, s);
            DrawMinimap(g, s);

            if (screen == GameScreen.Paused) DrawPauseOverlay(g);
            if (screen == GameScreen.Dead)   DrawDeadOverlay(g, s);
        }

        // ── District name labels only (ground is baked into WorldBaker) ─────
        private void DrawDistricts(Graphics g, GameState s)
        {
            foreach (var d in GameState.Districts)
            {
                int wx = d.GridX * GameState.DistrictSize;
                int wy = d.GridY * GameState.DistrictSize;
                using var lb = new SolidBrush(Color.FromArgb(40, d.Accent));
                var sz = g.MeasureString(d.Name.ToUpper(), _labelFont);
                g.DrawString(d.Name.ToUpper(), _labelFont, lb,
                    wx + (GameState.DistrictSize - sz.Width) / 2f,
                    wy + (GameState.DistrictSize - sz.Height) / 2f);
            }
        }

        // ── Roads removed — fully baked into WorldBaker.World ─────────────────

        // ── Buildings ─────────────────────────────────────────────────────────
        private void DrawBuildings(Graphics g, GameState s)
        {
            var  rng   = new Random(42);
            int  step  = GameState.RoadStep;
            bool night = s.IsNight();

            foreach (var d in GameState.Districts)
            {
                int ox = d.GridX * GameState.DistrictSize;
                int oy = d.GridY * GameState.DistrictSize;

                for (int row = 0; row < GameState.DistrictSize / step; row++)
                for (int col = 0; col < GameState.DistrictSize / step; col++)
                {
                    int pad = 20 + rng.Next(8);
                    int bx  = ox + col * step + pad;
                    int by  = oy + row * step + pad;
                    int bw  = step - pad * 2 + rng.Next(14) - 7;
                    int bh  = step - pad * 2 + rng.Next(14) - 7;
                    if (bw < 10 || bh < 10) continue;

                    float sh = 0.30f + (float)rng.NextDouble() * 0.28f;
                    var bc = Color.FromArgb(
                        Math.Clamp((int)(d.Ground.R * sh + d.Accent.R * 0.12f), 0, 255),
                        Math.Clamp((int)(d.Ground.G * sh + d.Accent.G * 0.12f), 0, 255),
                        Math.Clamp((int)(d.Ground.B * sh + d.Accent.B * 0.12f), 0, 255));

                    using var br  = new SolidBrush(bc);
                    using var pen = new Pen(Color.FromArgb(110, d.Accent), 1);
                    g.FillRectangle(br, bx, by, bw, bh);
                    g.DrawRectangle(pen, bx, by, bw, bh);

                    // Rooftop detail + AC units
                    if (bw > 22 && bh > 22)
                    {
                        using var rb  = new SolidBrush(Color.FromArgb(35, d.Accent));
                        using var dot = new SolidBrush(Color.FromArgb(60, d.Accent));
                        g.FillRectangle(rb, bx + 5, by + 5, bw / 2 - 3, bh / 2 - 3);
                        for (int i = 0; i < 3; i++)
                            g.FillRectangle(dot, bx + bw - 8 - i * 6, by + 4, 4, 4);

                        // Small building-use label drawn from a seeded tag pool
                        string[] tags = d.Id switch
                        {
                            "old_quarter"    => new[]{"BAR","PAWN","LAUNDRY","DINER","MARKET"},
                            "glass_heights"  => new[]{"CORP","BANK","HOTEL","LAW","MEDIA"},
                            "ash_industrial" => new[]{"DEPOT","FORGE","WASTE","SMELTER"},
                            "iron_docks"     => new[]{"DOCK","STORAGE","FREIGHT","FUEL"},
                            "the_spire"      => new[]{"EXEC","VAULT","PENTHOUSE","CLUB"},
                            _                => new[]{"BAR","HOSTEL","TATTOO","ARCADE"},
                        };
                        int tagIdx = (bx * 3 + by * 7) % tags.Length;
                        using var tl = new SolidBrush(Color.FromArgb(55, d.Accent));
                        g.DrawString(tags[tagIdx], _tinyFont, tl, bx + 3, by + bh - 9);
                    }

                    // Lit windows at night — scattered 4×4 yellow/warm rects
                    if (night && bw > 14 && bh > 14)
                    {
                        int winSeed = bx * 7 + by * 13;
                        var wrng   = new Random(winSeed);
                        int cols   = Math.Max(1, bw / 10);
                        int rows   = Math.Max(1, bh / 10);
                        for (int wr = 0; wr < rows; wr++)
                        for (int wc = 0; wc < cols; wc++)
                        {
                            if (wrng.NextDouble() < 0.45) continue;  // ~55% of windows lit
                            int wx2  = bx + 3 + wc * 10;
                            int wy2  = by + 3 + wr * 10;
                            // Warm yellow-orange tint; vary slightly per window
                            int rr = 220 + wrng.Next(35);
                            int gg = 180 + wrng.Next(40);
                            int bb =  40 + wrng.Next(30);
                            using var win = new SolidBrush(Color.FromArgb(210, rr, gg, bb));
                            g.FillRectangle(win, wx2, wy2, 4, 4);
                        }
                    }
                }
            }
        }

        // ── Mission markers + world markers (world space) ─────────────────────
        private void DrawMissionMarkers(Graphics g, GameState s)
        {
            bool b = Blink;

            // World markers (safehouse, garage, shop)
            foreach (var m in GameState.Markers)
                DrawMarker(g, m.Pos, m.Col, m.DisplayName.Split(' ')[0].ToUpper(), b);

            // Mission pickup / deliver
            if ((s.MissionPhase == MissionPhase.Inactive || s.MissionPhase == MissionPhase.Pickup)
                && s.MissionAvailable)
                DrawMarker(g, s.PickupPos, Color.Yellow, "PICKUP", b);

            if (s.MissionPhase == MissionPhase.Deliver)
                DrawMarker(g, s.DeliveryPos, Color.Cyan, "DELIVER", b);
        }

        private void DrawMarker(Graphics g, PointF pos, Color col, string label, bool blink)
        {
            float r = blink ? 22f : 17f;
            using var fill = new SolidBrush(Color.FromArgb(55, col));
            using var pen  = new Pen(Color.FromArgb(220, col), 2f);
            g.FillEllipse(fill, pos.X - r, pos.Y - r, r * 2, r * 2);
            g.DrawEllipse(pen,  pos.X - r, pos.Y - r, r * 2, r * 2);
            using var beam = new Pen(Color.FromArgb(blink ? 150 : 70, col), 1.5f);
            g.DrawLine(beam, pos.X, pos.Y - r, pos.X, pos.Y - r - 55);
            using var lb = new SolidBrush(col);
            var sz = g.MeasureString(label, _tinyFont);
            g.DrawString(label, _tinyFont, lb, pos.X - sz.Width / 2, pos.Y - r - 67);
        }

        // ── NPC vehicles ──────────────────────────────────────────────────────
        private void DrawNpcs(Graphics g, GameState s)
        {
            foreach (var npc in s.Npcs)
                DrawVehicleSprite(g, npc.Pos, npc.Angle, npc.SpriteKey, npc.IsPolice, s.AnimTick);
        }

        // ── Vehicle sprite (police uses animated light-bar frames) ────────────
        private static void DrawVehicleSprite(Graphics g, PointF pos, float angle,
                                              string spriteKey, bool police, int animTick)
        {
            string key = spriteKey;
            if (police)
            {
                int pf = (animTick / 10) % 3 + 1;
                string ak = $"Police_anim_{pf}";
                if (Sprites.Get(ak) is not null) key = ak;
            }
            Sprites.Draw(g, key, pos.X, pos.Y, 40, 40, angle);
        }

        // ── Player character sprite (8-dir, walk/idle animated) ────────────────
        private static void DrawPlayer(Graphics g, GameState s)
        {
            float a = ((s.PlayerAngle % 360f) + 360f) % 360f;
            string dir = a switch
            {
                >= 337.5f or < 22.5f   => "up",
                >= 22.5f  and < 67.5f  => "up_right",
                >= 67.5f  and < 112.5f => "right",
                >= 112.5f and < 157.5f => "down_right",
                >= 157.5f and < 202.5f => "down",
                >= 202.5f and < 247.5f => "down_right",
                >= 247.5f and < 292.5f => "right",
                _                      => "up_right",
            };
            bool flipH = a >= 202.5f && a < 337.5f;

            Bitmap? bmp;
            if (s.IsMoving)
            {
                int wf = (s.AnimTick / 5) % 8 + 1;  // 8 walk frames at ~12fps
                bmp = Sprites.Get($"char_walk_{dir}_{wf}")
                   ?? Sprites.Get("char_walk_down_1");
            }
            else
            {
                int idleF = (s.AnimTick / 8) % 4 + 1;
                bmp = Sprites.Get($"char_idle_{dir}_{idleF}")
                   ?? Sprites.Get("char_idle_down_1");
            }
            if (bmp is null) return;

            const int size = 32;
            var saved = g.Save();
            g.TranslateTransform(s.PlayerPos.X, s.PlayerPos.Y);
            if (flipH) g.ScaleTransform(-1f, 1f);
            g.DrawImage(bmp, -size / 2, -size / 2, size, size);
            g.Restore(saved);
        }

        // ── NPC pedestrians ──────────────────────────────────────────────────
        private static void DrawPeds(Graphics g, GameState s)
        {
            foreach (var p in s.Peds)
            {
                float a = ((p.Angle % 360f) + 360f) % 360f;
                string dir = a switch
                {
                    >= 337.5f or < 22.5f   => "up",
                    >= 22.5f  and < 67.5f  => "up_right",
                    >= 67.5f  and < 112.5f => "right",
                    >= 112.5f and < 157.5f => "down_right",
                    >= 157.5f and < 202.5f => "down",
                    >= 202.5f and < 247.5f => "down_right",
                    >= 247.5f and < 292.5f => "right",
                    _                      => "up_right",
                };
                bool flipH = a >= 202.5f && a < 337.5f;
                int wf    = p.WalkFrame + 1;
                var bmp   = Sprites.Get($"char_walk_{dir}_{wf}")
                         ?? Sprites.Get("char_walk_down_1");
                if (bmp is null) continue;
                var saved = g.Save();
                g.TranslateTransform(p.Pos.X, p.Pos.Y);
                if (flipH) g.ScaleTransform(-1f, 1f);
                g.DrawImage(bmp, -12, -12, 24, 24);
                g.Restore(saved);
            }
        }

        // ── Muzzle flash ──────────────────────────────────────────────────────
        private static void DrawMuzzleFlash(Graphics g, GameState s)
        {
            float rad = (s.PlayerAngle - 90f) * MathF.PI / 180f;
            float fx  = s.PlayerPos.X + MathF.Cos(rad) * 22f;
            float fy  = s.PlayerPos.Y + MathF.Sin(rad) * 22f;
            using var fb = new SolidBrush(Color.FromArgb(210, 255, 240, 80));
            g.FillEllipse(fb, fx - 8, fy - 8, 16, 16);
            using var fo = new SolidBrush(Color.FromArgb(120, 255, 255, 255));
            g.FillEllipse(fo, fx - 4, fy - 4, 8, 8);
        }

        // ── Title screen ──────────────────────────────────────────────────────
        private void DrawTitleScreen(Graphics g)
        {
            g.Clear(Color.FromArgb(6, 4, 14));
            // Grid lines for atmosphere
            using var grid = new Pen(Color.FromArgb(18, 0, 200, 100), 1f);
            for (int x = 0; x < _screen.Width;  x += 40) g.DrawLine(grid, x, 0, x, _screen.Height);
            for (int y = 0; y < _screen.Height; y += 40) g.DrawLine(grid, 0, y, _screen.Width, y);

            // Title
            using var titleFont = new Font("Consolas", 52, FontStyle.Bold);
            using var titleBr   = new SolidBrush(Color.FromArgb(220, 60, 255, 140));
            string title = "CITY//ZERO";
            var ts = g.MeasureString(title, titleFont);
            g.DrawString(title, titleFont, titleBr,
                (_screen.Width - ts.Width) / 2f, _screen.Height / 2f - 120f);

            // Subtitle
            using var subFont = new Font("Consolas", 13, FontStyle.Regular);
            using var subBr   = new SolidBrush(Color.FromArgb(170, 180, 220, 180));
            string sub = "A top-down open world crime game";
            var ss = g.MeasureString(sub, subFont);
            g.DrawString(sub, subFont, subBr, (_screen.Width - ss.Width) / 2f, _screen.Height / 2f - 40f);

            // Controls
            string[] lines =
            {
                "WASD ─ Move          E ─ Interact",
                "Q ─ Cycle Weapon    Space ─ Fire",
                "F5 ─ Save           F9 ─ Load",
                "P / ESC ─ Pause     +/- ─ Zoom",
            };
            using var ctrlFont = new Font("Consolas", 9, FontStyle.Regular);
            using var ctrlBr   = new SolidBrush(Color.FromArgb(120, 140, 200, 140));
            float cy2 = _screen.Height / 2f + 20f;
            foreach (var line in lines)
            {
                var ls = g.MeasureString(line, ctrlFont);
                g.DrawString(line, ctrlFont, ctrlBr, (_screen.Width - ls.Width) / 2f, cy2);
                cy2 += 18f;
            }

            // Prompt
            if ((_tick / 30) % 2 == 0)
            {
                using var pFont = new Font("Consolas", 14, FontStyle.Bold);
                using var pBr   = new SolidBrush(Color.FromArgb(220, 255, 220, 60));
                string prompt = "PRESS  ENTER  TO  START";
                var ps2 = g.MeasureString(prompt, pFont);
                g.DrawString(prompt, pFont, pBr,
                    (_screen.Width - ps2.Width) / 2f, _screen.Height / 2f + 130f);
            }
        }

        // ── Pause overlay ─────────────────────────────────────────────────────
        private void DrawPauseOverlay(Graphics g)
        {
            using var dim = new SolidBrush(Color.FromArgb(160, 4, 4, 12));
            g.FillRectangle(dim, 0, 0, _screen.Width, _screen.Height);
            using var pFont = new Font("Consolas", 36, FontStyle.Bold);
            using var pBr   = new SolidBrush(Color.FromArgb(230, 255, 220, 60));
            string txt = "PAUSED";
            var sz = g.MeasureString(txt, pFont);
            g.DrawString(txt, pFont, pBr,
                (_screen.Width - sz.Width) / 2f, _screen.Height / 2f - 40f);
            using var hFont = new Font("Consolas", 11, FontStyle.Regular);
            using var hBr   = new SolidBrush(Color.FromArgb(160, 180, 180, 180));
            string hint = "P or ESC to resume  •  F5 to save";
            var hs = g.MeasureString(hint, hFont);
            g.DrawString(hint, hFont, hBr,
                (_screen.Width - hs.Width) / 2f, _screen.Height / 2f + 20f);
        }

        // ── Death overlay ─────────────────────────────────────────────────────
        private void DrawDeadOverlay(Graphics g, GameState s)
        {
            using var dim = new SolidBrush(Color.FromArgb(185, 60, 0, 0));
            g.FillRectangle(dim, 0, 0, _screen.Width, _screen.Height);
            using var dFont = new Font("Consolas", 42, FontStyle.Bold);
            using var dBr   = new SolidBrush(Color.FromArgb(240, 230, 30, 30));
            string txt = "WASTED";
            var sz = g.MeasureString(txt, dFont);
            g.DrawString(txt, dFont, dBr,
                (_screen.Width - sz.Width) / 2f, _screen.Height / 2f - 60f);
            using var sFont = new Font("Consolas", 11, FontStyle.Regular);
            using var sBr   = new SolidBrush(Color.FromArgb(180, 200, 180, 180));
            string sub = $"Cash lost: ${Math.Min(s.Cash / 5, 500)}   You respawn at the Safehouse";
            var ss2 = g.MeasureString(sub, sFont);
            g.DrawString(sub, sFont, sBr, (_screen.Width - ss2.Width) / 2f, _screen.Height / 2f + 10f);
            if ((_tick / 28) % 2 == 0)
            {
                string hint = "PRESS  ENTER  TO  RESPAWN";
                var hs = g.MeasureString(hint, sFont);
                g.DrawString(hint, sFont, new SolidBrush(Color.FromArgb(210, 255, 200, 60)),
                    (_screen.Width - hs.Width) / 2f, _screen.Height / 2f + 50f);
            }
        }

        // ── HUD ───────────────────────────────────────────────────────────────
        private void DrawHUD(Graphics g, GameState s)
        {
            // Top bar
            g.FillRectangle(new SolidBrush(Color.FromArgb(215, 8, 8, 14)),
                0, 0, _screen.Width, 54);
            g.DrawLine(new Pen(Color.FromArgb(50, Color.Gray)), 0, 54, _screen.Width, 54);

            // Title / FPS / clock
            g.DrawString("CITY//ZERO", _hudFont, new SolidBrush(Color.FromArgb(200,255,255,255)), 10, 6);
            g.DrawString($"FPS {s.FPS}", _tinyFont, Brushes.DimGray, 10, 26);
            g.DrawString(s.TimeString(), _hudFont, new SolidBrush(Color.FromArgb(200,255,210,90)), 10, 38);

            // Cash
            g.DrawString($"${s.Cash}", _hudFont, new SolidBrush(Color.FromArgb(255,70,210,70)), 130, 8);

            // District
            var dist = s.CurrentDistrict();
            g.DrawString(dist?.Name.ToUpper() ?? "—", _smallFont,
                new SolidBrush(Color.FromArgb(180, dist?.Accent ?? Color.Gray)), 130, 26);

            // Coords
            g.DrawString($"X:{s.PlayerPos.X:0}  Y:{s.PlayerPos.Y:0}",
                _tinyFont, Brushes.DimGray, 130, 40);

            // Heat meter
            DrawHeatMeter(g, s, 340, 8);

            // HP / Armor bars
            DrawBar(g, "HP",    s.Health, 100, Color.FromArgb(210,55,55),  620, 8);
            DrawBar(g, "ARMOR", s.Armor,  100, Color.FromArgb(55,115,210), 620, 28);

            // Controls hint
            g.DrawString("WASD=Move  E=Interact  Q=Weapon  Space=Fire  +/-=Zoom  F5=Save  F9=Load",
                _tinyFont, Brushes.DimGray, _screen.Width - 490, 40);

            // Weapon display
            if (s.EquippedWeapon is not null)
            {
                string wLabel = s.ReloadTimer > 0
                    ? $"{s.EquippedWeapon.DisplayName}  RELOADING..."
                    : $"{s.EquippedWeapon.DisplayName}  {(s.EquippedWeapon.Category=="melee" ? "MELEE" : $"{s.WeaponAmmo}/{s.EquippedWeapon.MagazineSize}")}";
                g.DrawString(wLabel, _smallFont,
                    new SolidBrush(Color.FromArgb(220, 255, 200, 60)), _screen.Width / 2 - 80, 8);
            }

            // Active world event banner
            if (s.ActiveEventText != "")
            {
                g.DrawString(s.ActiveEventText, _smallFont,
                    new SolidBrush(Color.FromArgb(220, 255, 80, 60)),
                    (_screen.Width - g.MeasureString(s.ActiveEventText, _smallFont).Width) / 2, 58);
            }

            // Faction rep sidebar
            DrawFactionRep(g, s);

            // Mission strip (bottom bar)
            if (s.MissionPhase != MissionPhase.Inactive || !s.MissionAvailable)
                DrawMissionStrip(g, s);

            // Interact prompt (centre screen)
            if (s.ShowPrompt) DrawPrompt(g, s.PromptLabel);
        }

        private void DrawHeatMeter(Graphics g, GameState s, int x, int y)
        {
            g.DrawString("HEAT", _tinyFont, Brushes.DimGray, x, y);
            for (int i = 0; i < 5; i++)
            {
                bool lit = i < s.HeatLevel;
                var  col = lit ? _heatColors[Math.Min(s.HeatLevel-1, 4)]
                               : Color.FromArgb(38,38,44);
                using var br = new SolidBrush(col);
                g.FillRectangle(br, x + i * 30, y + 16, 26, 16);
                if (lit)
                {
                    using var glow = new Pen(Color.FromArgb(130, col), 1);
                    g.DrawRectangle(glow, x + i * 30, y + 16, 26, 16);
                }
            }
        }

        private void DrawBar(Graphics g, string label, int val, int max, Color col, int x, int y)
        {
            const int bw = 130, bh = 13;
            g.DrawString(label, _tinyFont, Brushes.DimGray, x, y);
            g.FillRectangle(new SolidBrush(Color.FromArgb(38,38,44)), x+48, y, bw, bh);
            g.FillRectangle(new SolidBrush(col), x+48, y, (int)(bw * Math.Clamp((float)val/max,0,1)), bh);
            g.DrawRectangle(new Pen(Color.FromArgb(55, Color.White)), x+48, y, bw, bh);
            g.DrawString($"{val}", _tinyFont, Brushes.DimGray, x+48+bw+3, y);
        }

        private void DrawMissionStrip(Graphics g, GameState s)
        {
            int sy = _screen.Height - 58;
            g.FillRectangle(new SolidBrush(Color.FromArgb(200,7,7,13)), 0, sy, _screen.Width, 42);
            g.DrawLine(new Pen(Color.FromArgb(70, Color.Gray)), 0, sy, _screen.Width, sy);

            Color tcol = s.MissionPhase switch
            {
                MissionPhase.Complete => Color.FromArgb( 70,230, 70),
                MissionPhase.Failed   => Color.FromArgb(230, 55, 55),
                _                     => Color.FromArgb(255,190, 50),
            };
            g.DrawString(s.MissionTitle.ToUpper(), _hudFont, new SolidBrush(tcol), 12, sy + 4);
            g.DrawString(s.MissionObjective, _smallFont,
                new SolidBrush(Color.FromArgb(210,210,210)), 12, sy + 22);

            if (s.MissionPhase == MissionPhase.Deliver && s.MissionTimer > 0)
            {
                Color tc = s.MissionTimer < 30f ? Color.Red : Color.Yellow;
                g.DrawString($"{(int)s.MissionTimer}s", _hudFont,
                    new SolidBrush(tc), _screen.Width - 90, sy + 8);
            }
        }

        private void DrawPrompt(Graphics g, string label)
        {
            const int pw = 320, ph = 30;
            int px = (_screen.Width - pw) / 2, py = _screen.Height / 2 - 50;
            g.FillRectangle(new SolidBrush(Color.FromArgb(190,8,8,18)), px, py, pw, ph);
            g.DrawRectangle(new Pen(Color.FromArgb(200, Color.Yellow), 1f), px, py, pw, ph);
            g.DrawString(label, _smallFont, Brushes.Yellow, px + 10, py + 8);
        }

        // ── Heat vignette (screen-edge red pulse) ─────────────────────────────
        private void DrawVignette(Graphics g, GameState s)
        {
            if (s.HeatLevel == 0) return;
            int alpha = Math.Clamp((int)(s.HeatScore * 0.55f), 0, 140);
            // Blink at max heat
            if (s.HeatLevel >= 4 && !Blink) alpha /= 3;
            var col = Color.FromArgb(alpha, 180, 0, 0);
            const int th = 80;  // vignette thickness
            using var br = new SolidBrush(col);
            g.FillRectangle(br, 0,                          0,                        _screen.Width, th);
            g.FillRectangle(br, 0,                          _screen.Height - th,       _screen.Width, th);
            g.FillRectangle(br, 0,                          0,                        th,             _screen.Height);
            g.FillRectangle(br, _screen.Width - th,        0,                        th,             _screen.Height);
        }

        // ── Faction rep sidebar ───────────────────────────────────────────────
        private void DrawFactionRep(Graphics g, GameState s)
        {
            const int panW = 155, rowH = 18, pad = 4;
            int px = pad;
            int py = _screen.Height - 60 - GameState.Factions.Length * rowH - pad;

            // Panel background
            g.FillRectangle(new SolidBrush(Color.FromArgb(170,7,7,13)),
                px - 2, py - 2, panW + 4, GameState.Factions.Length * rowH + 6);
            g.DrawString("REP", _tinyFont, Brushes.DimGray, px, py - 12);

            foreach (var f in GameState.Factions)
            {
                int rep = s.Reputation.TryGetValue(f.Id, out int v) ? v : 0;
                var tier = RepHelper.Tier(rep);
                string tierStr = tier switch
                {
                    RepTier.Hostile  => "\u2666HOSTILE",
                    RepTier.Friendly => "\u2665FRIEND",
                    RepTier.Trusted  => "\u2605TRUSTED",
                    _                => "",
                };
                // Label
                using var lb = new SolidBrush(Color.FromArgb(170, f.FactionColor));
                g.DrawString(f.Name[..Math.Min(f.Name.Length, 12)], _tinyFont, lb, px, py + 2);
                // Tier string (right of name)
                if (tierStr != "")
                {
                    var tierCol = tier == RepTier.Hostile
                        ? Color.FromArgb(200, 220, 50, 50)
                        : Color.FromArgb(180, f.FactionColor);
                    using var tb = new SolidBrush(tierCol);
                    g.DrawString(tierStr, _tinyFont, tb, px + 78, py + 2);
                }
                // Bar track
                const int bx = 96, bw = 55, bh = 7;
                g.FillRectangle(new SolidBrush(Color.FromArgb(38,38,44)), px + bx, py + 4, bw, bh);
                if (rep != 0)
                {
                    float frac   = Math.Abs(rep) / 100f;
                    var   barCol = rep > 0 ? f.FactionColor : Color.FromArgb(190,50,50);
                    using var bb = new SolidBrush(Color.FromArgb(200, barCol));
                    g.FillRectangle(bb, px + bx, py + 4, (int)(bw * frac), bh);
                }
                g.DrawRectangle(new Pen(Color.FromArgb(40, Color.White)), px + bx, py + 4, bw, bh);
                py += rowH;
            }
        }

        // ── Minimap ───────────────────────────────────────────────────────────
        private void DrawMinimap(Graphics g, GameState s)
        {
            const int mW = 210, mH = 155, pad = 12;
            int mx = _screen.Width  - mW - pad;
            int my = _screen.Height - mH - pad - 20;

            g.FillRectangle(new SolidBrush(Color.FromArgb(195,7,7,13)), mx, my, mW, mH);

            float wW = GameState.DistrictSize * 3f, wH = GameState.DistrictSize * 2f;

            // District tiles
            foreach (var d in GameState.Districts)
            {
                float dx = mx + (d.GridX * GameState.DistrictSize / wW) * mW;
                float dy = my + (d.GridY * GameState.DistrictSize / wH) * mH;
                float dw = (GameState.DistrictSize / wW) * mW;
                float dh = (GameState.DistrictSize / wH) * mH;
                using var br  = new SolidBrush(Color.FromArgb(155, d.Ground));
                using var pen = new Pen(Color.FromArgb(70, d.Accent), 1);
                g.FillRectangle(br, dx, dy, dw, dh);
                g.DrawRectangle(pen, dx, dy, dw, dh);
                using var lb = new SolidBrush(Color.FromArgb(90, d.Accent));
                g.DrawString(d.Name, _tinyFont, lb, dx + 2, dy + 2);
            }

            // World marker blips (safehouse=green, garage=amber, shop=purple)
            foreach (var m in GameState.Markers)
            {
                float mmx = mx + (m.Pos.X / wW) * mW;
                float mmy = my + (m.Pos.Y / wH) * mH;
                g.FillRectangle(new SolidBrush(m.Col), mmx - 3, mmy - 3, 6, 6);
                g.DrawRectangle(new Pen(Color.FromArgb(180, m.Col), 1f), mmx - 3, mmy - 3, 6, 6);
            }

            // NPC blips
            foreach (var npc in s.Npcs)
            {
                float nx = mx + (npc.Pos.X / wW) * mW;
                float ny = my + (npc.Pos.Y / wH) * mH;
                var col = npc.IsPolice ? Color.RoyalBlue : Color.FromArgb(110,110,110);
                g.FillRectangle(new SolidBrush(col), nx - 1.5f, ny - 1.5f, 3, 3);
            }

            // Mission blip
            PointF? blip = null; Color blipCol = Color.White;
            if ((s.MissionPhase == MissionPhase.Inactive || s.MissionPhase == MissionPhase.Pickup)
                && s.MissionAvailable)
            { blip = s.PickupPos; blipCol = Color.Yellow; }
            else if (s.MissionPhase == MissionPhase.Deliver)
            { blip = s.DeliveryPos; blipCol = Color.Cyan; }

            if (blip.HasValue)
            {
                float bx = mx + (blip.Value.X / wW) * mW;
                float by = my + (blip.Value.Y / wH) * mH;
                g.FillEllipse(new SolidBrush(blipCol), bx - 4, by - 4, 8, 8);
            }

            // Player arrow
            float ppx = mx + (s.PlayerPos.X / wW) * mW;
            float ppy = my + (s.PlayerPos.Y / wH) * mH;
            var ps = g.Save();
            g.TranslateTransform(ppx, ppy);
            g.RotateTransform(s.PlayerAngle);
            g.FillPolygon(Brushes.Yellow, new PointF[] { new(0,-5), new(-3,3), new(3,3) });
            g.Restore(ps);

            g.DrawRectangle(new Pen(Color.FromArgb(85, Color.Gray)), mx, my, mW, mH);
            g.DrawString("MAP", _tinyFont, Brushes.DimGray, mx + 3, my - 13);
        }
    }

    // ── Game loop (WinForms timer, ~60 fps) ───────────────────────────────────
    class GameLoop
    {
        private readonly Timer _timer;

        public GameLoop(int intervalMs, Action tick)
        {
            _timer = new Timer { Interval = intervalMs };
            _timer.Tick += (_, _) => tick();
        }

        public void Start() => _timer.Start();
        public void Stop()  => _timer.Stop();
    }
}
