// ============================================================
//  CityZero — Core Systems (pure C#, no Unity)
//  Ported from Unity Code Scaffold
// ============================================================

using System;
using System.Collections.Generic;

// ── Namespaces mirrored from the original scaffold ──────────────────────────

namespace CityZero.Gameplay.Heat
{
    public readonly struct HeatLevelChangedEvent
    {
        public readonly int   PreviousLevel;
        public readonly int   CurrentLevel;
        public readonly float HeatScore;
        public HeatLevelChangedEvent(int prev, int cur, float score)
        { PreviousLevel = prev; CurrentLevel = cur; HeatScore = score; }
    }

    public readonly struct CrimeCommittedEvent
    {
        public readonly int   Severity;
        public readonly int   WitnessCount;
        public readonly float DistrictMultiplier;
        public CrimeCommittedEvent(int sev, int wit, float mult)
        { Severity = sev; WitnessCount = wit; DistrictMultiplier = mult; }
    }

    /// <summary>Pure-C# port of HeatModel. No Unity dependencies.</summary>
    public sealed class HeatModel
    {
        private float _heatScore;
        private int   _heatLevel;
        private readonly float _decayPerSecond;

        public HeatModel(float decayPerSecond = 4f) => _decayPerSecond = decayPerSecond;

        public int   HeatLevel => _heatLevel;
        public float HeatScore => _heatScore;

        public event Action<HeatLevelChangedEvent>? LevelChanged;

        public void UpdateDecay(float deltaTime)
        {
            if (_heatScore <= 0f) return;
            _heatScore = Math.Max(0f, _heatScore - _decayPerSecond * deltaTime);
            RefreshLevel();
        }

        public void CommitCrime(int severity, int witnessCount, float districtMultiplier = 1f)
        {
            float added = severity * (1f + witnessCount * 0.25f) * Math.Max(0.1f, districtMultiplier);
            _heatScore  = Math.Min(100f, _heatScore + added);
            RefreshLevel();
        }

        /// <summary>Add (or subtract with negative delta) heat score. Clamps 0–100.</summary>
        public void AddScore(float delta)
        {
            _heatScore = Math.Clamp(_heatScore + delta, 0f, 100f);
            RefreshLevel();
        }

        /// <summary>Directly set heat score. Clamps 0–100.</summary>
        public void SetScore(float score)
        {
            _heatScore = Math.Clamp(score, 0f, 100f);
            RefreshLevel();
        }

        private void RefreshLevel()
        {
            int prev = _heatLevel;
            _heatLevel = _heatScore switch
            {
                < 10f  => 0,
                < 25f  => 1,
                < 45f  => 2,
                < 70f  => 3,
                < 100f => 4,
                _      => 5,
            };
            if (_heatLevel != prev)
                LevelChanged?.Invoke(new HeatLevelChangedEvent(prev, _heatLevel, _heatScore));
        }
    }
}

namespace CityZero.Gameplay.Missions
{
    public enum MissionState { Inactive, Active, Succeeded, Failed }

    public readonly struct MissionStateChangedEvent
    {
        public readonly string      MissionId;
        public readonly MissionState State;
        public MissionStateChangedEvent(string id, MissionState s) { MissionId = id; State = s; }
    }

    public sealed class MissionObjectiveData
    {
        public string Type    = "";
        public string TargetId= "";
        public string Label   = "";
        public bool   Done;
    }

    public sealed class MissionData
    {
        public string                     Id          = "";
        public string                     Title       = "";
        public string                     Description = "";
        public List<MissionObjectiveData> Objectives  = new();
    }

    public sealed class MissionRuntime
    {
        private readonly MissionData _data;
        private int _objectiveIndex;

        public string        Id    => _data.Id;
        public MissionState  State { get; private set; } = MissionState.Inactive;

        public event Action<MissionStateChangedEvent>? StateChanged;

        public MissionRuntime(MissionData data) => _data = data;

        public void Start()
        {
            State = MissionState.Active;
            StateChanged?.Invoke(new MissionStateChangedEvent(Id, State));
        }

        public string GetObjectiveText()
        {
            if (_objectiveIndex >= _data.Objectives.Count) return _data.Title + " — complete";
            return _data.Objectives[_objectiveIndex].Label;
        }

        public void CompleteCurrentObjective()
        {
            if (_objectiveIndex < _data.Objectives.Count)
            {
                _data.Objectives[_objectiveIndex].Done = true;
                _objectiveIndex++;
            }
            if (_objectiveIndex >= _data.Objectives.Count)
                Complete();
        }

        public void Complete()
        {
            State = MissionState.Succeeded;
            StateChanged?.Invoke(new MissionStateChangedEvent(Id, State));
        }

        public void Fail()
        {
            State = MissionState.Failed;
            StateChanged?.Invoke(new MissionStateChangedEvent(Id, State));
        }
    }

    public sealed class MissionManager
    {
        private readonly Dictionary<string, MissionData>    _catalog = new();
        private readonly Dictionary<string, MissionRuntime> _active  = new();

        public event Action<MissionStateChangedEvent>? MissionStateChanged;

        public void RegisterMission(MissionData data) => _catalog[data.Id] = data;

        public MissionRuntime? StartMission(string id)
        {
            if (!_catalog.TryGetValue(id, out var data)) return null;
            if (_active.ContainsKey(id))                 return _active[id];

            var runtime = new MissionRuntime(data);
            runtime.StateChanged += e => MissionStateChanged?.Invoke(e);
            _active[id] = runtime;
            runtime.Start();
            return runtime;
        }

        public MissionRuntime? GetRuntime(string id) =>
            _active.TryGetValue(id, out var r) ? r : null;

        public IEnumerable<MissionRuntime> ActiveMissions => _active.Values;
    }
}

namespace CityZero.Core.EventBus
{
    using CityZero.Gameplay.Heat;
    using CityZero.Gameplay.Missions;

    public static class GameEventBus
    {
        public static event Action<HeatLevelChangedEvent>?    HeatLevelChanged;
        public static event Action<CrimeCommittedEvent>?      CrimeCommitted;
        public static event Action<MissionStateChangedEvent>? MissionStateChanged;

        public static void Raise(HeatLevelChangedEvent e)    => HeatLevelChanged?.Invoke(e);
        public static void Raise(CrimeCommittedEvent e)      => CrimeCommitted?.Invoke(e);
        public static void Raise(MissionStateChangedEvent e) => MissionStateChanged?.Invoke(e);
    }
}

namespace CityZero.Gameplay.Combat
{
    public interface IDamageable
    {
        void ApplyDamage(int amount);
        void Heal(int amount);
        bool IsAlive { get; }
    }

    public sealed class HealthComponent : IDamageable
    {
        public int  MaxHealth     { get; }
        public int  CurrentHealth { get; private set; }
        public bool IsAlive       => CurrentHealth > 0;

        public event Action? Died;

        public HealthComponent(int maxHealth = 100)
        {
            MaxHealth     = maxHealth;
            CurrentHealth = maxHealth;
        }

        public void ApplyDamage(int amount)
        {
            if (!IsAlive || amount <= 0) return;
            CurrentHealth = Math.Max(0, CurrentHealth - amount);
            if (CurrentHealth == 0) Died?.Invoke();
        }

        public void Heal(int amount)
        {
            if (!IsAlive || amount <= 0) return;
            CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount);
        }

        public void FullRestore() => CurrentHealth = MaxHealth;
    }
}

namespace CityZero.Gameplay.Inventory
{
    public sealed class SimpleInventory
    {
        private readonly Dictionary<string, int> _items = new();

        public void Add(string itemId, int count = 1)
        {
            _items[itemId] = _items.GetValueOrDefault(itemId) + count;
        }

        public bool Remove(string itemId, int count = 1)
        {
            if (_items.GetValueOrDefault(itemId) < count) return false;
            _items[itemId] -= count;
            return true;
        }

        public int  GetCount(string itemId) => _items.GetValueOrDefault(itemId);
        public bool Has(string itemId, int count = 1) => GetCount(itemId) >= count;

        public IReadOnlyDictionary<string, int> AllItems => _items;
    }
}

namespace CityZero.Core.SaveLoad
{
    using System.Text.Json;
    using System.IO;

    public sealed class SaveGameData
    {
        public float  PlayerX      { get; set; }
        public float  PlayerY      { get; set; }
        public float  HeatScore    { get; set; }
        public int    Cash         { get; set; }
        public string ActiveMission{ get; set; } = "";
    }

    public static class SaveSystem
    {
        private static readonly string SavePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "CityZero", "save.json");

        public static void Save(SaveGameData data)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath)!);
            File.WriteAllText(SavePath, JsonSerializer.Serialize(data,
                new JsonSerializerOptions { WriteIndented = true }));
        }

        public static SaveGameData Load()
        {
            if (!File.Exists(SavePath)) return new SaveGameData();
            try   { return JsonSerializer.Deserialize<SaveGameData>(File.ReadAllText(SavePath)) ?? new(); }
            catch { return new SaveGameData(); }
        }
    }
}

namespace CityZero.Gameplay.Vehicles
{
    /// <summary>Pure-C# vehicle physics model (no Unity Rigidbody).</summary>
    public sealed class VehicleModel
    {
        public float X, Y;
        public float Angle;       // degrees
        public float Speed;       // pixels/sec

        private readonly float _acceleration = 220f;
        private readonly float _brakeForce   = 340f;
        private readonly float _maxSpeed     = 380f;
        private readonly float _steerSpeed   = 140f;
        private readonly float _drag         = 0.96f;

        public bool HasDriver { get; private set; }
        public void EnterVehicle()  => HasDriver = true;
        public void ExitVehicle()   => HasDriver = false;

        public void Update(float throttle, float steer, float dt)
        {
            if (!HasDriver) { Speed *= _drag; return; }

            // Steering
            if (Math.Abs(Speed) > 5f)
                Angle += steer * _steerSpeed * dt * Math.Sign(Speed);

            // Throttle / brake
            if (throttle > 0)
                Speed = Math.Min(Speed + _acceleration * throttle * dt, _maxSpeed);
            else if (throttle < 0)
                Speed = Math.Max(Speed + _brakeForce * throttle * dt, -_maxSpeed * 0.5f);
            else
                Speed *= _drag;

            // Move
            float rad = Angle * MathF.PI / 180f;
            X -= MathF.Sin(rad) * Speed * dt;
            Y -= MathF.Cos(rad) * Speed * dt;
        }
    }
}
