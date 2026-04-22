// ── CITY//ZERO  Data Layer ────────────────────────────────────────────────────
// All data-driven types derived from the master prompt schemas:
//   district, faction, mission, weapon, world event, world marker, save state.
// No engine dependencies — pure C# records and classes.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CityZero
{
    // ── District data (from UNITY.txt §2.1) ──────────────────────────────────
    record DistrictData(
        string   Id,
        string   DisplayName,
        float    HeatMultiplier,       // 0.8–1.5 — scales heat gain in this district
        float    CivilianDensityDay,
        float    CivilianDensityNight,
        float    TrafficDensityDay,
        float    PolicePatrolIntensity,
        float    WealthLevel,          // 0–1  used for shop stock and ped variety
        string[] DominantFactions
    );

    // ── Faction relationship tier ─────────────────────────────────────────────
    enum RepTier { Hostile = 0, Neutral = 1, Friendly = 2, Trusted = 3 }

    static class RepHelper
    {
        public static RepTier Tier(int rep) => rep switch
        {
            < -20  => RepTier.Hostile,
            < 35   => RepTier.Neutral,
            < 70   => RepTier.Friendly,
            _      => RepTier.Trusted,
        };
    }

    // ── Weapon (from UNITY.txt §2.5 and weapon.schema.example.json) ──────────
    record Weapon(
        string Id,
        string DisplayName,
        string Category,       // "melee" | "pistol" | "smg" | "shotgun" | "rifle"
        int    Damage,
        float  FireRate,       // shots per second
        int    MagazineSize,
        float  ReloadTime,
        float  Range,          // world-units
        float  HeatNoise,      // how much this weapon adds to HeatScore per shot
        int    BuyPrice,
        int    SellPrice,
        bool   DriveByCompatible
    );

    // ── World marker types (from vertical slice manifest / wiring doc) ────────
    enum MarkerType { Safehouse, Garage, Shop, MissionPickup, MissionDeliver }

    record WorldMarker(
        string     Id,
        string     DisplayName,
        MarkerType Type,
        PointF     Pos,
        Color      Col
    );

    // ── Mission definition (from VERTICAL_SLICE_FLOW + mission JSON IDs) ─────
    // NOTE: MissionPhase enum lives in Game.cs to avoid duplicate definition.

    record MissionDef(
        string  Id,
        string  Title,
        string  GiverFaction,          // faction id
        PointF  PickupPos,
        PointF  DeliverPos,
        float   TimeLimit,             // seconds; 0 = no limit
        int     CashReward,
        int     HeatInjected,          // heat score added on delivery
        (string FactionId, int Delta)[] RepRewards,
        string  NextMissionId          // "" = end of chain
    );

    // ── World event (from world_event.schema.example.json) ───────────────────
    enum WorldEventState { Idle, Active, Cooldown }

    class WorldEvent
    {
        public string          Id          { get; }
        public string          DisplayName { get; }
        public float           Duration    { get; }    // seconds active
        public float           Cooldown    { get; }    // seconds until can retrigger
        public float           HeatImpact  { get; }    // added to HeatScore while active
        public int             CashBonus   { get; }    // rewarded if player is nearby
        public string[]        Districts   { get; }    // which districts it can fire in
        public WorldEventState State       { get; private set; } = WorldEventState.Idle;
        public float           Timer       { get; private set; }
        public string          ActiveIn    { get; private set; } = "";

        public WorldEvent(string id, string name, float dur, float cd,
                          float heat, int cash, string[] districts)
        {
            Id = id; DisplayName = name; Duration = dur; Cooldown = cd;
            HeatImpact = heat; CashBonus = cash; Districts = districts;
        }

        public void TryTrigger(string districtId, Random rng)
        {
            if (State != WorldEventState.Idle) return;
            if (Array.IndexOf(Districts, districtId) < 0) return;
            State    = WorldEventState.Active;
            Timer    = Duration;
            ActiveIn = districtId;
        }

        public string? Tick(float dt)   // returns display string when active, null otherwise
        {
            if (State == WorldEventState.Idle) return null;
            Timer -= dt;
            if (State == WorldEventState.Active && Timer <= 0)
            {
                State = WorldEventState.Cooldown;
                Timer = Cooldown;
            }
            else if (State == WorldEventState.Cooldown && Timer <= 0)
            {
                State = WorldEventState.Idle;
            }
            return State == WorldEventState.Active ? $"EVENT: {DisplayName}  ({ActiveIn})" : null;
        }
    }

    // ── Save state (from Project.txt §Save System) ────────────────────────────
    class SaveState
    {
        public float                    PosX             { get; set; }
        public float                    PosY             { get; set; }
        public int                      Health           { get; set; } = 100;
        public int                      Armor            { get; set; } = 50;
        public int                      Cash             { get; set; } = 800;
        public float                    HeatScore        { get; set; }
        public float                    WorldTime        { get; set; } = 480f;
        public Dictionary<string, int>  Reputation       { get; set; } = new();
        public string                   ActiveMissionId  { get; set; } = "";
        public int                      MissionPhase     { get; set; }  // cast to MissionPhase
        public float                    MissionTimer     { get; set; }
        public List<string>             CompletedMissions{ get; set; } = new();
        public string?                  EquippedWeaponId { get; set; }
        public List<string>             OwnedWeapons     { get; set; } = new();
    }

    // ── Static game data registry (all content defined here) ─────────────────
    static class GameData
    {
        // ── Districts (mirrors District records + schema data) ─────────────────
        public static readonly DistrictData[] Districts =
        {
            new("old_quarter",    "Old Quarter",    1.1f, 0.85f, 0.65f, 0.70f, 0.45f, 0.35f,
                new[]{"blue_saints","razor_union"}),
            new("glass_heights",  "Glass Heights",  0.85f, 0.60f, 0.40f, 0.60f, 0.70f, 0.85f,
                new[]{"helix_directorate"}),
            new("ash_industrial", "Ash Industrial", 1.0f, 0.70f, 0.55f, 0.80f, 0.40f, 0.40f,
                new[]{"cinder_mob","razor_union"}),
            new("iron_docks",     "Iron Docks",     1.2f, 0.75f, 0.60f, 0.75f, 0.35f, 0.30f,
                new[]{"razor_union"}),
            new("the_spire",      "The Spire",      0.90f, 0.50f, 0.35f, 0.55f, 0.80f, 0.90f,
                new[]{"helix_directorate","velvet_circuit"}),
            new("neon_flats",     "Neon Flats",     1.3f, 0.90f, 0.80f, 0.65f, 0.30f, 0.20f,
                new[]{"velvet_circuit","blue_saints"}),
        };

        // ── Weapons (6 for MVP per Project.txt §5.1) ─────────────────────────
        public static readonly Weapon[] Weapons =
        {
            new("pipe_hook",    "Pipe Hook",    "melee",   22, 1.5f,  1, 0.0f,  8f, 0.0f,  0,   0,   false),
            new("pistol_vz11",  "VZ-11 Pistol","pistol",  14, 2.5f, 12, 1.4f, 28f, 0.3f,  600, 200, true),
            new("smg_union9",   "Union-9 SMG", "smg",     14,11.5f, 28, 1.9f, 24f, 0.8f, 1450, 500, true),
            new("shotgun_cut",  "Cutlass SG",  "shotgun", 38, 1.2f,  6, 2.2f, 14f, 1.2f, 1200, 420, false),
            new("rifle_spec",   "Specter R",   "rifle",   28, 5.0f, 20, 2.8f, 48f, 1.0f, 2200, 800, false),
            new("molotov",      "Molotov",     "thrown",  55, 0.5f,  1, 0.0f, 18f, 2.5f,  250, 80,  false),
        };

        // ── Mission chain (mq_a1 = act 1, Old Quarter) ────────────────────────
        // Mirrors mq_a1_03_dead_drop_dive from manifest + 3 follow-up missions
        public static readonly MissionDef[] Missions =
        {
            new(
                Id:            "mq_a1_01_first_contact",
                Title:         "First Contact",
                GiverFaction:  "blue_saints",
                PickupPos:     new(70, 70),
                DeliverPos:    new(520, 90),
                TimeLimit:     0,
                CashReward:    400,
                HeatInjected:  20,
                RepRewards:    new[]{("blue_saints", 5)},
                NextMissionId: "mq_a1_02_street_tax"
            ),
            new(
                Id:            "mq_a1_02_street_tax",
                Title:         "Street Tax",
                GiverFaction:  "razor_union",
                PickupPos:     new(540, 70),
                DeliverPos:    new(80, 520),
                TimeLimit:     0,
                CashReward:    600,
                HeatInjected:  30,
                RepRewards:    new[]{("razor_union", 6), ("blue_saints", -3)},
                NextMissionId: "mq_a1_03_dead_drop_dive"
            ),
            new(
                Id:            "mq_a1_03_dead_drop_dive",  // the canonical vertical slice mission
                Title:         "Dead Drop Dive",
                GiverFaction:  "blue_saints",
                PickupPos:     new(80, 80),                // package_dead_drop_terminal
                DeliverPos:    new(520, 520),              // rooftop_mailbox_07
                TimeLimit:     180f,
                CashReward:    900,
                HeatInjected:  55,
                RepRewards:    new[]{("blue_saints", 8), ("velvet_circuit", 4)},
                NextMissionId: "mq_a1_04_iron_run"
            ),
            new(
                Id:            "mq_a1_04_iron_run",
                Title:         "Iron Run",
                GiverFaction:  "cinder_mob",
                PickupPos:     new(120, 620),              // iron_docks area (row 1)
                DeliverPos:    new(460, 680),
                TimeLimit:     240f,
                CashReward:    1100,
                HeatInjected:  65,
                RepRewards:    new[]{("cinder_mob", 8), ("helix_directorate", -4)},
                NextMissionId: ""
            ),
        };

        // ── World markers (safehouse, garage, shop from manifest JSON) ─────────
        public static readonly WorldMarker[] Markers =
        {
            new("safehouse_01", "Safehouse",        MarkerType.Safehouse, new(55,  55),  Color.FromArgb(80,230,80)),
            new("garage_01",    "Milo's Garage",    MarkerType.Garage,    new(290, 55),  Color.FromArgb(255,180,30)),
            new("shop_01",      "Backroom Exchange",MarkerType.Shop,      new(55, 290),  Color.FromArgb(140,90,220)),
        };

        // ── World events (10 for MVP per Project.txt §5.1) ────────────────────
        public static readonly WorldEvent[] Events =
        {
            new("gang_shootout",      "Gang Shootout",      90f,  600f, 1.3f,  0,   new[]{"old_quarter","neon_flats","iron_docks"}),
            new("illegal_race",       "Illegal Street Race",60f,  480f, 0.4f,  300, new[]{"old_quarter","ash_industrial"}),
            new("blackout",           "District Blackout",  120f, 900f, 0.8f,  0,   new[]{"neon_flats","the_spire"}),
            new("armored_transport",  "Armored Transport",  420f, 1200f,1.6f, 1200, new[]{"glass_heights","old_quarter"}),
            new("riot",               "Street Riot",        180f, 720f, 1.5f,  0,   new[]{"old_quarter","ash_industrial","neon_flats"}),
            new("undercover_sweep",   "Undercover Sweep",   90f,  600f, 1.2f,  0,   new[]{"glass_heights","the_spire"}),
            new("contraband_drop",    "Contraband Drop",    60f,  480f, 0.6f,  500, new[]{"iron_docks","ash_industrial"}),
            new("chase_through",      "High-Speed Chase",   45f,  360f, 0.9f,  0,   new[]{"old_quarter","glass_heights"}),
            new("faction_standoff",   "Faction Standoff",   120f, 900f, 1.1f,  0,   new[]{"old_quarter","neon_flats"}),
            new("market_shakedown",   "Market Shakedown",   75f,  540f, 0.7f,  200, new[]{"old_quarter","ash_industrial"}),
        };

        // ── Lookup helpers ────────────────────────────────────────────────────
        public static DistrictData? GetDistrict(string id)
        {
            foreach (var d in Districts) if (d.Id == id) return d;
            return null;
        }
        public static MissionDef? GetMission(string id)
        {
            foreach (var m in Missions) if (m.Id == id) return m;
            return null;
        }
        public static Weapon? GetWeapon(string id)
        {
            foreach (var w in Weapons) if (w.Id == id) return w;
            return null;
        }
    }

    // ── Save / Load (persists to %APPDATA%\CityZero\save.json) ───────────────
    static class SaveManager
    {
        private static readonly string SaveDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "CityZero");
        private static readonly string SavePath = Path.Combine(SaveDir, "save.json");

        private static readonly JsonSerializerOptions _opts = new()
        {
            WriteIndented        = true,
            PropertyNameCaseInsensitive = true,
        };

        public static void Save(GameState s)
        {
            try
            {
                Directory.CreateDirectory(SaveDir);
                var state = new SaveState
                {
                    PosX             = s.PlayerPos.X,
                    PosY             = s.PlayerPos.Y,
                    Health           = s.Health,
                    Armor            = s.Armor,
                    Cash             = s.Cash,
                    HeatScore        = s.HeatScore,
                    WorldTime        = s.WorldTime,
                    Reputation       = new Dictionary<string, int>(s.Reputation),
                    ActiveMissionId  = s.ActiveMission?.Id ?? "",
                    MissionPhase     = (int)s.MissionPhase,
                    MissionTimer     = s.MissionTimer,
                    CompletedMissions= new List<string>(s.CompletedMissions),
                    EquippedWeaponId = s.EquippedWeapon?.Id,
                    OwnedWeapons     = new List<string>(s.OwnedWeaponIds),
                };
                File.WriteAllText(SavePath, JsonSerializer.Serialize(state, _opts));
            }
            catch { /* silently skip save errors in demo */ }
        }

        public static bool Load(GameState s)
        {
            try
            {
                if (!File.Exists(SavePath)) return false;
                var state = JsonSerializer.Deserialize<SaveState>(
                    File.ReadAllText(SavePath), _opts);
                if (state is null) return false;

                s.PlayerPos        = new System.Drawing.PointF(state.PosX, state.PosY);
                s.Health           = state.Health;
                s.Armor            = state.Armor;
                s.Cash             = state.Cash;
                s.HeatScore        = state.HeatScore;
                s.WorldTime        = state.WorldTime;
                foreach (var kv in state.Reputation) s.Reputation[kv.Key] = kv.Value;
                s.MissionPhase     = (MissionPhase)state.MissionPhase;
                s.MissionTimer     = state.MissionTimer;
                s.CompletedMissions= new HashSet<string>(state.CompletedMissions);
                s.OwnedWeaponIds   = new HashSet<string>(state.OwnedWeapons);
                if (state.EquippedWeaponId is not null)
                    s.EquippedWeapon = GameData.GetWeapon(state.EquippedWeaponId);
                if (state.ActiveMissionId != "")
                    s.ActiveMission = GameData.GetMission(state.ActiveMissionId);
                return true;
            }
            catch { return false; }
        }

        public static void Delete()
        {
            try { if (File.Exists(SavePath)) File.Delete(SavePath); } catch { }
        }
    }
}
