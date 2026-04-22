// ── CITY//ZERO  Ported Systems ───────────────────────────────────────────────
// All logic ported from src/ with Godot API stripped.
// Sources:
//   src/Core/GameBus.cs          → GameBus
//   src/Core/ServiceLocator.cs   → ServiceLocator  (kept as-is, no Godot)
//   src/Core/Events/GameEvents.cs→ all IGameEvent structs
//   src/Systems/HeatSystem.cs    → HeatSystem
//   src/Systems/FactionSystem.cs → FactionSystem
//   src/Systems/EconomySystem.cs → EconomySystem
//   src/Systems/MissionSystem.cs → MissionSystem / MissionInstance
//   src/Systems/SaveSystem.cs    → SaveSystem (atomic write, 4 slots)
//   src/Systems/TimeManager.cs   → TimeManager
//   src/AI/BehaviorTree/BehaviorTree.cs → full BT + AIBlackboard
//   src/Data/Definitions/AllDefinitions.cs → all Definition records

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CityZero
{
    // ═══════════════════════════════════════════════════════════════════════════
    // CORE — GameBus (typed pub/sub; no Godot dependency)
    // ═══════════════════════════════════════════════════════════════════════════
    static class GameBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _subs = new();

        public static void Subscribe<T>(Action<T> h) where T : IGameEvent
        {
            if (!_subs.ContainsKey(typeof(T))) _subs[typeof(T)] = new();
            _subs[typeof(T)].Add(h);
        }

        public static void Unsubscribe<T>(Action<T> h) where T : IGameEvent
        {
            if (_subs.TryGetValue(typeof(T), out var l)) l.Remove(h);
        }

        public static void Emit<T>(T ev) where T : IGameEvent
        {
            if (!_subs.TryGetValue(typeof(T), out var l)) return;
            foreach (var d in l.ToArray()) (d as Action<T>)?.Invoke(ev);
        }
    }

    public interface IGameEvent { }

    // ═══════════════════════════════════════════════════════════════════════════
    // CORE — ServiceLocator (identical to src/Core/ServiceLocator.cs)
    // ═══════════════════════════════════════════════════════════════════════════
    static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<T>(T s) where T : class
            => _services[typeof(T)] = s ?? throw new ArgumentNullException(nameof(s));

        public static T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var s)) return (T)s;
            throw new InvalidOperationException($"[ServiceLocator] {typeof(T).Name} not registered.");
        }

        public static bool TryGet<T>(out T s) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var o)) { s = (T)o; return true; }
            s = null!; return false;
        }

        public static void Unregister<T>() where T : class => _services.Remove(typeof(T));
        public static void Clear() => _services.Clear();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // EVENTS — ported from src/Core/Events/GameEvents.cs
    // ═══════════════════════════════════════════════════════════════════════════
    readonly struct HeatChangedEvent : IGameEvent
    {
        public float  NewHeat  { get; }
        public int    NewLevel { get; }
        public string Reason   { get; }
        public HeatChangedEvent(float h, int l, string r) { NewHeat=h; NewLevel=l; Reason=r; }
    }

    readonly struct PlayerDiedEvent : IGameEvent
    {
        public float LastHealth { get; }
        public PlayerDiedEvent(float h) => LastHealth = h;
    }

    readonly struct MissionStateChangedEvent : IGameEvent
    {
        public string        MissionId { get; }
        public MissionState  OldState  { get; }
        public MissionState  NewState  { get; }
        public MissionStateChangedEvent(string id, MissionState o, MissionState n)
        { MissionId=id; OldState=o; NewState=n; }
    }

    readonly struct ObjectiveCompletedEvent : IGameEvent
    {
        public string MissionId   { get; }
        public string ObjectiveId { get; }
        public bool   IsOptional  { get; }
        public ObjectiveCompletedEvent(string m, string o, bool opt)
        { MissionId=m; ObjectiveId=o; IsOptional=opt; }
    }

    readonly struct FactionRepChangedEvent : IGameEvent
    {
        public string FactionId { get; }
        public float  OldRep    { get; }
        public float  NewRep    { get; }
        public string Reason    { get; }
        public FactionRepChangedEvent(string f, float o, float n, string r)
        { FactionId=f; OldRep=o; NewRep=n; Reason=r; }
    }

    readonly struct EconomyTransactionEvent : IGameEvent
    {
        public float  Amount     { get; }
        public float  NewBalance { get; }
        public string Reason     { get; }
        public EconomyTransactionEvent(float a, float b, string r)
        { Amount=a; NewBalance=b; Reason=r; }
    }

    readonly struct WorldEventSpawnedEvent : IGameEvent
    {
        public string EventId  { get; }
        public string District { get; }
        public WorldEventSpawnedEvent(string e, string d) { EventId=e; District=d; }
    }

    enum MissionState { Available, Active, Completed, Failed }

    // ═══════════════════════════════════════════════════════════════════════════
    // SYSTEM — HeatSystem  (ported from src/Systems/HeatSystem.cs)
    // ═══════════════════════════════════════════════════════════════════════════
    /// <summary>
    /// Manages the player's wanted level (0–5 float).
    /// Per-level cooldown delays before decay begins; immediate clear in safehouse.
    /// Emits HeatChangedEvent on every change.
    /// </summary>
    class HeatSystem
    {
        public float CurrentHeat { get; private set; } = 0f;
        public int   HeatLevel   => (int)Math.Floor(CurrentHeat);
        public bool  IsWanted    => CurrentHeat >= 1f;

        // Per-level cooldown (seconds of hiding before decay starts)
        private static readonly float[] CooldownDelays = { 0f, 15f, 30f, 45f, 60f, 90f };
        private const float DecayRate  = 0.5f;   // heat-units per second
        private const float MaxHeat    = 5f;

        private float _cooldownTimer = 0f;
        private bool  _hidden        = false;
        private bool  _inSafehouse   = false;

        public HeatSystem()
        {
            ServiceLocator.Register(this);
        }

        public void Tick(float dt)
        {
            if (_inSafehouse) { SetHeat(0f, "safehouse"); return; }

            if (_hidden && CurrentHeat > 0f)
            {
                int   level = Math.Clamp(HeatLevel, 0, CooldownDelays.Length - 1);
                float delay = CooldownDelays[level];
                _cooldownTimer += dt;
                if (_cooldownTimer >= delay)
                    SetHeat(Math.Max(0f, CurrentHeat - DecayRate * dt), "decay");
            }
            else
            {
                _cooldownTimer = 0f;
            }
        }

        // ── Public API (mirrors src/Systems/HeatSystem.cs) ───────────────────
        public void AddHeat(float amount, string reason = "unspecified")
        {
            if (amount <= 0f) return;
            SetHeat(Math.Min(MaxHeat, CurrentHeat + amount), reason);
        }

        public void ReduceHeat(float amount, string reason = "unspecified")
        {
            if (amount <= 0f) return;
            SetHeat(Math.Max(0f, CurrentHeat - amount), reason);
        }

        public void ClearHeat(string reason = "cleared") => SetHeat(0f, reason);

        public void SetPlayerHidden(bool hidden)
        {
            if (_hidden == hidden) return;
            _hidden = hidden;
            if (!hidden) _cooldownTimer = 0f;
        }

        public void SetInSafehouse(bool v) => _inSafehouse = v;

        // Sync to/from GameState for backwards compat with save system
        public void SyncFromGameState(float heatScore)
            => CurrentHeat = Math.Clamp(heatScore / 20f, 0f, MaxHeat); // 0-100 → 0-5

        public float ToHeatScore() => CurrentHeat * 20f;  // 0-5 → 0-100

        private void SetHeat(float v, string reason)
        {
            v = Math.Clamp(v, 0f, MaxHeat);
            if (Math.Abs(v - CurrentHeat) < 0.001f) return;
            CurrentHeat = v;
            GameBus.Emit(new HeatChangedEvent(CurrentHeat, HeatLevel, reason));
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // SYSTEM — FactionSystem  (ported from src/Systems/FactionSystem.cs)
    // ═══════════════════════════════════════════════════════════════════════════
    /// <summary>
    /// 5-tier reputation per faction with cross-faction bleed.
    /// Shop price multiplier varies with tier.
    /// </summary>
    class FactionSystem
    {
        public const float MIN_REP = -100f, MAX_REP = 100f;

        private readonly Dictionary<string, float> _rep = new()
        {
            ["blue_saints"]       = 0f,
            ["razor_union"]       = 0f,
            ["velvet_circuit"]    = 0f,
            ["cinder_mob"]        = 0f,
            ["helix_directorate"] = 0f,
        };

        // Cross-faction bleed: (from, to, multiplier)
        private static readonly (string from, string to, float bleed)[] Bleed =
        {
            ("razor_union",    "blue_saints",    -0.35f),
            ("blue_saints",    "razor_union",    -0.35f),
            ("velvet_circuit", "helix_directorate", -0.25f),
            ("helix_directorate", "velvet_circuit", -0.25f),
        };

        public FactionSystem() => ServiceLocator.Register(this);

        public float GetRep(string id)
            => _rep.TryGetValue(id, out var v) ? v : 0f;

        public FactionTier GetTier(string id) => GetRep(id) switch
        {
            <= -61f => FactionTier.Enemy,
            <= -21f => FactionTier.Hostile,
            <=  20f => FactionTier.Neutral,
            <=  60f => FactionTier.Allied,
            _       => FactionTier.Trusted,
        };

        public void ModifyRep(string id, float delta, string reason = "")
        {
            if (!_rep.ContainsKey(id)) return;
            float old = _rep[id];
            _rep[id] = Math.Clamp(old + delta, MIN_REP, MAX_REP);
            GameBus.Emit(new FactionRepChangedEvent(id, old, _rep[id], reason));

            // Apply bleed to related factions
            foreach (var (from, to, mult) in Bleed)
            {
                if (from != id || !_rep.ContainsKey(to)) continue;
                float bd  = delta * mult;
                if (Math.Abs(bd) < 0.01f) continue;
                float bo  = _rep[to];
                _rep[to]  = Math.Clamp(bo + bd, MIN_REP, MAX_REP);
                GameBus.Emit(new FactionRepChangedEvent(to, bo, _rep[to], $"bleed_from_{id}"));
            }
        }

        /// <summary>Price multiplier for shop items: Trusted=0.70, Enemy=no service.</summary>
        public float GetPriceMultiplier(string id) => GetTier(id) switch
        {
            FactionTier.Trusted  => 0.70f,
            FactionTier.Allied   => 0.85f,
            FactionTier.Hostile  => 1.30f,
            FactionTier.Enemy    => float.MaxValue,
            _                    => 1.00f,
        };

        /// <summary>Sync int rep dict from GameState (for save/load compat).</summary>
        public void RestoreFromDict(Dictionary<string, int> saved)
        {
            foreach (var kv in saved)
                if (_rep.ContainsKey(kv.Key))
                    _rep[kv.Key] = Math.Clamp(kv.Value, MIN_REP, MAX_REP);
        }

        public Dictionary<string, int> ToIntDict()
            => _rep.ToDictionary(kv => kv.Key, kv => (int)kv.Value);

        public Dictionary<string, float> GetAll() => new(_rep);
    }

    public enum FactionTier { Enemy, Hostile, Neutral, Allied, Trusted }

    // ═══════════════════════════════════════════════════════════════════════════
    // SYSTEM — EconomySystem  (ported from src/Systems/EconomySystem.cs)
    // ═══════════════════════════════════════════════════════════════════════════
    /// <summary>
    /// Wallet with faction discounts, a global price modifier (territory events),
    /// and a 50-entry transaction log.
    /// </summary>
    class EconomySystem
    {
        public float Wallet { get; private set; } = 800f;

        private float _globalMod = 1.0f;
        private readonly List<(float amount, float balance, string reason)> _log = new(50);

        public EconomySystem() => ServiceLocator.Register(this);

        public bool TrySpend(float amount, string reason = "purchase")
        {
            if (amount <= 0f) return true;
            float final = amount * _globalMod;
            if (Wallet < final) return false;
            Wallet -= final;
            Log(-final, reason);
            return true;
        }

        public void AddCash(float amount, string reason = "reward")
        {
            if (amount <= 0f) return;
            Wallet += amount;
            Log(amount, reason);
        }

        public float GetFinalPrice(float basePrice, float factionMult = 1f)
            => basePrice * factionMult * _globalMod;

        public void SetGlobalPriceModifier(float mod)
            => _globalMod = Math.Max(0.1f, mod);

        public void RestoreWallet(float amount)
            => Wallet = Math.Max(0f, amount);

        public int WalletInt => (int)Wallet;

        private void Log(float amount, string reason)
        {
            if (_log.Count >= 50) _log.RemoveAt(0);
            _log.Add((amount, Wallet, reason));
            GameBus.Emit(new EconomyTransactionEvent(amount, Wallet, reason));
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // SYSTEM — MissionSystem  (ported from src/Systems/MissionSystem.cs)
    // ═══════════════════════════════════════════════════════════════════════════
    /// <summary>
    /// Tracks up to 3 concurrent mission instances.
    /// Auto-fails all active missions on PlayerDiedEvent.
    /// </summary>
    class MissionSystem
    {
        private const int MaxActive = 3;
        private readonly Dictionary<string, MissionInstance> _active    = new();
        private readonly HashSet<string>                     _completed = new();
        private readonly HashSet<string>                     _failed    = new();

        public MissionSystem()
        {
            ServiceLocator.Register(this);
            GameBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
        }

        public bool ActivateMission(string id)
        {
            if (_active.ContainsKey(id) || _active.Count >= MaxActive) return false;
            var inst = new MissionInstance(id);
            inst.OnObjectiveCompleted += (mid, oid, opt) =>
                GameBus.Emit(new ObjectiveCompletedEvent(mid, oid, opt));
            inst.OnMissionResolved += (mid, ok) =>
            {
                if (ok) Complete(mid);
                else    Fail(mid, "objective_failed");
            };
            _active[id] = inst;
            GameBus.Emit(new MissionStateChangedEvent(id, MissionState.Available, MissionState.Active));
            return true;
        }

        public void ReportEvent(string evType, string targetId, object? payload = null)
        {
            foreach (var m in _active.Values.ToList())
                m.EvaluateEvent(evType, targetId, payload);
        }

        public bool IsCompleted(string id) => _completed.Contains(id);
        public bool IsFailed(string id)    => _failed.Contains(id);
        public bool IsActive(string id)    => _active.ContainsKey(id);
        public int  ActiveCount            => _active.Count;

        public bool TryGetActive(string id, out MissionInstance inst)
            => _active.TryGetValue(id, out inst!);

        public void RestoreCompleted(IEnumerable<string> ids)
        {
            foreach (var id in ids) _completed.Add(id);
        }

        public HashSet<string> GetCompleted() => new(_completed);

        private void OnPlayerDied(PlayerDiedEvent _)
        {
            foreach (var id in _active.Keys.ToList())
                Fail(id, "player_death");
        }

        private void Complete(string id)
        {
            if (!_active.Remove(id)) return;
            _completed.Add(id);
            GameBus.Emit(new MissionStateChangedEvent(id, MissionState.Active, MissionState.Completed));
        }

        private void Fail(string id, string reason)
        {
            if (!_active.Remove(id)) return;
            _failed.Add(id);
            GameBus.Emit(new MissionStateChangedEvent(id, MissionState.Active, MissionState.Failed));
        }
    }

    class MissionInstance
    {
        public string MissionId { get; }

        public event Action<string, string, bool>? OnObjectiveCompleted;
        public event Action<string, bool>?          OnMissionResolved;

        private readonly List<string>    _required  = new();
        private readonly HashSet<string> _done      = new();

        public MissionInstance(string id) => MissionId = id;

        public void AddObjective(string oid, bool optional = false)
        { if (!optional) _required.Add(oid); }

        public void EvaluateEvent(string evType, string targetId, object? payload)
        {
            if (_required.Contains(targetId) && !_done.Contains(targetId))
            {
                _done.Add(targetId);
                OnObjectiveCompleted?.Invoke(MissionId, targetId, false);
                if (_done.Count >= _required.Count)
                    OnMissionResolved?.Invoke(MissionId, true);
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // SYSTEM — MultiSlotSaveSystem  (ported from src/Systems/SaveSystem.cs)
    // Atomic write (.tmp rename), 4 slots (0-2 manual, 3 = auto-save)
    // ═══════════════════════════════════════════════════════════════════════════
    static class MultiSlotSaveSystem
    {
        public const int AutoSaveSlot = 3;
        private static readonly string SaveDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "CityZero", "saves");

        static MultiSlotSaveSystem()
        {
            Directory.CreateDirectory(SaveDir);
        }

        public static bool SaveGame(int slot, SlotSaveData data)
        {
            Directory.CreateDirectory(SaveDir);
            string path    = SlotPath(slot);
            string tmpPath = path + ".tmp";
            try
            {
                string json = JsonSerializer.Serialize(data,
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(tmpPath, json);
                if (File.Exists(path)) File.Delete(path);
                File.Move(tmpPath, path);
                return true;
            }
            catch
            {
                if (File.Exists(tmpPath)) File.Delete(tmpPath);
                return false;
            }
        }

        public static SlotSaveData? LoadGame(int slot)
        {
            string path = SlotPath(slot);
            if (!File.Exists(path)) return null;
            try
            {
                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<SlotSaveData>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch { return null; }
        }

        public static bool SlotExists(int slot) => File.Exists(SlotPath(slot));

        public static void DeleteSlot(int slot)
        {
            string p = SlotPath(slot);
            if (File.Exists(p)) File.Delete(p);
        }

        private static string SlotPath(int slot) =>
            Path.Combine(SaveDir, $"save_slot_{slot}.json");
    }

    // Richer save schema — extends existing SaveState in Data.cs
    class SlotSaveData
    {
        public string SaveVersion   { get; set; } = "1.1.0";
        public long   Timestamp     { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        public float  PlaytimeSeconds { get; set; }

        // Player
        public float  PosX          { get; set; }
        public float  PosY          { get; set; }
        public int    Health        { get; set; } = 100;
        public int    Armor         { get; set; } = 50;
        public float  Wallet        { get; set; } = 800f;
        public float  HeatScore     { get; set; }
        public float  WorldTime     { get; set; } = 480f;

        // Faction (float precision)
        public Dictionary<string, float> Reputation { get; set; } = new();

        // Mission
        public string  ActiveMissionId   { get; set; } = "";
        public int     MissionPhaseInt   { get; set; }
        public float   MissionTimer      { get; set; }
        public List<string> CompletedMissions { get; set; } = new();

        // Weapons
        public string?     EquippedWeaponId { get; set; }
        public List<string> OwnedWeapons    { get; set; } = new();
        public int          WeaponAmmo      { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // SYSTEM — TimeManager  (ported from src/Systems/TimeManager.cs)
    // ═══════════════════════════════════════════════════════════════════════════
    /// <summary>
    /// In-game clock: 1 real second = 1 in-game minute by default (configurable).
    /// Fires HourChanged callback for density/AI schedule hooks.
    /// </summary>
    class TimeManager
    {
        public float RealSecondsPerGameMinute { get; set; } = 0.1f; // 10× real-time
        public float TimeOfDay { get; private set; } = 8f;  // 0–24 hours

        public bool IsNight => TimeOfDay >= 21f || TimeOfDay < 6f;
        public bool IsDay   => !IsNight;

        public event Action<int>? HourChanged;

        private float _acc     = 0f;
        private int   _lastHour = -1;

        public TimeManager() => ServiceLocator.Register(this);

        public void Tick(float dt)
        {
            _acc += dt;
            float mins = _acc / RealSecondsPerGameMinute;
            _acc -= mins * RealSecondsPerGameMinute;
            TimeOfDay = (TimeOfDay + mins / 60f) % 24f;

            int hour = (int)TimeOfDay;
            if (hour != _lastHour)
            {
                _lastHour = hour;
                HourChanged?.Invoke(hour);
            }
        }

        // Convert to/from GameState WorldTime (minutes 0–1440)
        public void SyncFromWorldTime(float worldTimeMinutes)
            => TimeOfDay = (worldTimeMinutes / 60f) % 24f;

        public float ToWorldTimeMinutes() => TimeOfDay * 60f;

        public void SetTime(float hours) => TimeOfDay = Math.Clamp(hours, 0f, 24f);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // AI — BehaviorTree  (full port from src/AI/BehaviorTree/BehaviorTree.cs)
    // ═══════════════════════════════════════════════════════════════════════════
    enum BTStatus { Success, Failure, Running }

    abstract class BTNode
    {
        public abstract BTStatus Tick(AIBlackboard b);
        public virtual  void     Reset(AIBlackboard b) { }
    }

    /// <summary>Selector (OR): returns Success on first child success.</summary>
    class BTSelector : BTNode
    {
        private readonly List<BTNode> _c;
        private int _ri = -1;
        public BTSelector(params BTNode[] c) => _c = new(c);

        public override BTStatus Tick(AIBlackboard b)
        {
            int s = _ri >= 0 ? _ri : 0;
            for (int i = s; i < _c.Count; i++)
            {
                var r = _c[i].Tick(b);
                if (r == BTStatus.Running)  { _ri = i; return BTStatus.Running; }
                if (r == BTStatus.Success)  { _ri = -1; return BTStatus.Success; }
            }
            _ri = -1; return BTStatus.Failure;
        }
        public override void Reset(AIBlackboard b) { _ri = -1; foreach (var n in _c) n.Reset(b); }
    }

    /// <summary>Sequence (AND): returns Failure on first child failure.</summary>
    class BTSequence : BTNode
    {
        private readonly List<BTNode> _c;
        private int _ri = -1;
        public BTSequence(params BTNode[] c) => _c = new(c);

        public override BTStatus Tick(AIBlackboard b)
        {
            int s = _ri >= 0 ? _ri : 0;
            for (int i = s; i < _c.Count; i++)
            {
                var r = _c[i].Tick(b);
                if (r == BTStatus.Running)  { _ri = i; return BTStatus.Running; }
                if (r == BTStatus.Failure)  { _ri = -1; return BTStatus.Failure; }
            }
            _ri = -1; return BTStatus.Success;
        }
        public override void Reset(AIBlackboard b) { _ri = -1; foreach (var n in _c) n.Reset(b); }
    }

    class BTInverter : BTNode
    {
        private readonly BTNode _child;
        public BTInverter(BTNode child) => _child = child;
        public override BTStatus Tick(AIBlackboard b) => _child.Tick(b) switch
        {
            BTStatus.Success => BTStatus.Failure,
            BTStatus.Failure => BTStatus.Success,
            _                => BTStatus.Running,
        };
    }

    class BTCondition : BTNode
    {
        private readonly Func<AIBlackboard, bool> _pred;
        public BTCondition(Func<AIBlackboard, bool> pred, string label = "") => _pred = pred;
        public override BTStatus Tick(AIBlackboard b) => _pred(b) ? BTStatus.Success : BTStatus.Failure;
    }

    class BTAction : BTNode
    {
        private readonly Func<AIBlackboard, BTStatus> _act;
        public BTAction(Func<AIBlackboard, BTStatus> act, string label = "") => _act = act;
        public override BTStatus Tick(AIBlackboard b) => _act(b);
    }

    /// <summary>Per-NPC data context shared across BT nodes during one tick.</summary>
    class AIBlackboard
    {
        private readonly Dictionary<string, object> _d = new();

        public void Set<T>(string k, T v) => _d[k] = v!;

        public T Get<T>(string k, T def = default!)
        {
            if (_d.TryGetValue(k, out var v) && v is T t) return t;
            return def;
        }

        public bool Has(string k)    => _d.ContainsKey(k);
        public void Remove(string k) => _d.Remove(k);
        public void Clear()          => _d.Clear();
    }

    // Blackboard key constants (from src/AI/BehaviorTree/BehaviorTree.cs BB class)
    static class BB
    {
        public const string Self              = "self";
        public const string IsUnderAttack     = "is_under_attack";
        public const string LastAttacker      = "last_attacker";
        public const string CurrentCover      = "current_cover";
        public const string PatrolWpIdx       = "patrol_waypoint_idx";
        public const string InvestigateTarget = "investigate_target";
        public const string FleeTarget        = "flee_target";
        public const string IsPlayerHostile   = "is_player_hostile";
        public const string HealthNorm        = "health_normalized";
        public const string CanSeePlayer      = "can_see_player";
        public const string AlertLevel        = "alert_level";  // 0=idle 1=aware 2=alarmed 3=combat
        // Extended for vehicle AI
        public const string TargetPos         = "target_pos";
        public const string IsPolice          = "is_police";
        public const string Speed             = "speed";
        public const string Angle             = "angle";
        public const string Pos               = "pos";
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // NPC BEHAVIOR TREES — wires BTNode graph to NpcVehicle / NpcPed
    // ═══════════════════════════════════════════════════════════════════════════
    /// <summary>
    /// Police vehicle BT:
    ///   Selector
    ///     Sequence [if player hostile AND can see player] → Chase → Ram
    ///     Sequence [alarmed] → Intercept
    ///     Patrol
    /// </summary>
    static class NpcBehaviors
    {
        /// <summary>Build and return a police chase BT rooted node.</summary>
        public static BTNode BuildPoliceBT() =>
            new BTSelector(
                // Branch 1: see player → chase + close in
                new BTSequence(
                    new BTCondition(b => b.Get<bool>(BB.IsPlayerHostile), "player_hostile"),
                    new BTCondition(b => b.Get<bool>(BB.CanSeePlayer),    "can_see_player"),
                    new BTAction(b =>
                    {
                        // Chase handled externally; just signal Running
                        b.Set(BB.AlertLevel, 3);
                        return BTStatus.Running;
                    }, "chase")
                ),
                // Branch 2: alarmed → intercept heading
                new BTSequence(
                    new BTCondition(b => b.Get<int>(BB.AlertLevel) >= 2, "alarmed"),
                    new BTAction(b =>
                    {
                        b.Set(BB.AlertLevel, 2);
                        return BTStatus.Running;
                    }, "intercept")
                ),
                // Branch 3: patrol
                new BTAction(b =>
                {
                    b.Set(BB.AlertLevel, 0);
                    return BTStatus.Running;
                }, "patrol")
            );

        /// <summary>Build a civilian wander BT.</summary>
        public static BTNode BuildCivilianBT() =>
            new BTSelector(
                // Flee if player hostile nearby
                new BTSequence(
                    new BTCondition(b => b.Get<bool>(BB.IsPlayerHostile), "flee_condition"),
                    new BTAction(b =>
                    {
                        b.Set(BB.AlertLevel, 2);
                        return BTStatus.Running;
                    }, "flee")
                ),
                // Normal drive-through
                new BTAction(b =>
                {
                    b.Set(BB.AlertLevel, 0);
                    return BTStatus.Running;
                }, "drive")
            );

        /// <summary>Build a pedestrian wander BT.</summary>
        public static BTNode BuildPedBT() =>
            new BTSelector(
                new BTSequence(
                    new BTCondition(b => b.Get<bool>(BB.IsPlayerHostile), "ped_flee"),
                    new BTAction(b => { b.Set(BB.AlertLevel, 2); return BTStatus.Running; }, "run_away")
                ),
                new BTAction(b => { b.Set(BB.AlertLevel, 0); return BTStatus.Running; }, "wander")
            );
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // DATA — AllDefinitions (ported from src/Data/Definitions/AllDefinitions.cs)
    // Stripped of Godot; JSON attributes kept so DataRegistry loading still works
    // ═══════════════════════════════════════════════════════════════════════════
    namespace Definitions
    {
        public class VehicleDefinition
        {
            [JsonPropertyName("vehicle_id")]          public string VehicleId { get; set; } = "";
            [JsonPropertyName("display_name")]        public string DisplayName { get; set; } = "";
            [JsonPropertyName("class")]               public string Class { get; set; } = "";
            [JsonPropertyName("faction_affiliation")]  public string FactionAffiliation { get; set; } = "";
            [JsonPropertyName("stats")]               public VehicleStats Stats { get; set; } = new();
            [JsonPropertyName("seats")]               public int Seats { get; set; }
            [JsonPropertyName("can_be_stolen")]       public bool CanBeStolen { get; set; }
            [JsonPropertyName("spawn_districts")]     public List<string> SpawnDistricts { get; set; } = new();
            [JsonPropertyName("spawn_weight")]        public int SpawnWeight { get; set; } = 1;
        }
        public class VehicleStats
        {
            [JsonPropertyName("top_speed_kmh")]  public float TopSpeedKmh  { get; set; }
            [JsonPropertyName("acceleration")]   public float Acceleration  { get; set; }
            [JsonPropertyName("handling")]       public float Handling      { get; set; }
            [JsonPropertyName("durability_hp")]  public int   DurabilityHp  { get; set; }
        }

        public class WeaponDefinition
        {
            [JsonPropertyName("weapon_id")]       public string WeaponId    { get; set; } = "";
            [JsonPropertyName("display_name")]    public string DisplayName { get; set; } = "";
            [JsonPropertyName("category")]        public string Category    { get; set; } = "";
            [JsonPropertyName("stats")]           public WeaponDefStats Stats { get; set; } = new();
            [JsonPropertyName("heat_modifier_per_shot")] public float HeatModPerShot { get; set; }
            [JsonPropertyName("price_shop")]      public int? PriceShop     { get; set; }
        }
        public class WeaponDefStats
        {
            [JsonPropertyName("damage_per_bullet")]  public float DamagePerBullet { get; set; }
            [JsonPropertyName("fire_rate_rpm")]      public float FireRateRpm     { get; set; }
            [JsonPropertyName("magazine_size")]      public int   MagazineSize    { get; set; }
            [JsonPropertyName("reload_time_sec")]    public float ReloadTimeSec   { get; set; }
            [JsonPropertyName("effective_range_m")]  public float EffectiveRangeM { get; set; }
        }

        public class FactionDefinition
        {
            [JsonPropertyName("faction_id")]          public string FactionId  { get; set; } = "";
            [JsonPropertyName("display_name")]        public string DisplayName { get; set; } = "";
            [JsonPropertyName("territory_districts")] public List<string> TerritoryDistricts { get; set; } = new();
            [JsonPropertyName("starting_reputation")] public float StartingReputation { get; set; }
            [JsonPropertyName("reputation_bleed")]    public List<RepBleedDef> ReputationBleed { get; set; } = new();
        }
        public class RepBleedDef
        {
            [JsonPropertyName("target_faction")] public string TargetFaction { get; set; } = "";
            [JsonPropertyName("multiplier")]     public float  Multiplier    { get; set; }
        }

        public class DistrictDefinition
        {
            [JsonPropertyName("district_id")]          public string DistrictId  { get; set; } = "";
            [JsonPropertyName("display_name")]         public string DisplayName  { get; set; } = "";
            [JsonPropertyName("controlling_faction")]  public string ControllingFaction { get; set; } = "";
            [JsonPropertyName("heat_escalation_modifier")] public float HeatEscalationModifier { get; set; } = 1f;
            [JsonPropertyName("cctv_coverage")]        public float CctvCoverage { get; set; }
            [JsonPropertyName("gameplay_traits")]      public List<string> GameplayTraits { get; set; } = new();
        }

        public class MissionDefinition
        {
            [JsonPropertyName("mission_id")]      public string MissionId   { get; set; } = "";
            [JsonPropertyName("title")]           public string Title       { get; set; } = "";
            [JsonPropertyName("giver_faction")]   public string GiverFaction { get; set; } = "";
            [JsonPropertyName("time_limit_seconds")] public int? TimeLimitSeconds { get; set; }
            [JsonPropertyName("heat_modifier_base")] public float HeatModifierBase { get; set; }
            [JsonPropertyName("objectives")]      public List<MissionObjectiveDef> Objectives { get; set; } = new();
            [JsonPropertyName("rewards")]         public MissionRewardsDef Rewards { get; set; } = new();
        }
        public class MissionObjectiveDef
        {
            [JsonPropertyName("id")]       public string Id       { get; set; } = "";
            [JsonPropertyName("type")]     public string Type     { get; set; } = "";
            [JsonPropertyName("target")]   public string Target   { get; set; } = "";
            [JsonPropertyName("label")]    public string Label    { get; set; } = "";
            [JsonPropertyName("required")] public bool   Required { get; set; } = true;
            [JsonPropertyName("bonus_cash")] public int  BonusCash { get; set; }
        }
        public class MissionRewardsDef
        {
            [JsonPropertyName("cash")]       public int Cash { get; set; }
            [JsonPropertyName("reputation")] public List<RepDeltaDef> Reputation { get; set; } = new();
            [JsonPropertyName("unlock_flags")] public List<string> UnlockFlags { get; set; } = new();
        }
        public class RepDeltaDef
        {
            [JsonPropertyName("faction")] public string Faction { get; set; } = "";
            [JsonPropertyName("delta")]   public int    Delta   { get; set; }
        }
    }
}
