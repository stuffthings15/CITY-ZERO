using Godot;
using System.Collections.Generic;
using System.Text.Json;

namespace CityZero.Data
{
    /// <summary>
    /// Loads and caches all JSON game data definitions at startup.
    /// Provides typed accessors for missions, vehicles, weapons, factions, etc.
    /// All paths relative to res://data/
    /// </summary>
    public partial class DataRegistry : Node
    {
        public static DataRegistry Instance { get; private set; }

        // Caches — populated on _Ready
        private readonly Dictionary<string, Definitions.VehicleDefinition> _vehicles = new();
        private readonly Dictionary<string, Definitions.WeaponDefinition>  _weapons  = new();
        private readonly Dictionary<string, Definitions.FactionDefinition> _factions = new();
        private readonly Dictionary<string, Definitions.DistrictDefinition> _districts = new();
        private readonly Dictionary<string, Definitions.MissionDefinition> _missions = new();

        public override void _Ready()
        {
            if (Instance != null) { QueueFree(); return; }
            Instance = this;

            LoadAll();
            GD.Print("[DataRegistry] All data loaded.");
        }

        // ── Public Accessors ─────────────────────────────────────────────────────

        public bool TryGetVehicle(string id, out Definitions.VehicleDefinition def)
            => _vehicles.TryGetValue(id, out def);

        public bool TryGetWeapon(string id, out Definitions.WeaponDefinition def)
            => _weapons.TryGetValue(id, out def);

        public bool TryGetFaction(string id, out Definitions.FactionDefinition def)
            => _factions.TryGetValue(id, out def);

        public bool TryGetDistrict(string id, out Definitions.DistrictDefinition def)
            => _districts.TryGetValue(id, out def);

        public bool TryGetMission(string id, out Definitions.MissionDefinition def)
            => _missions.TryGetValue(id, out def);

        // ── Private Loading ───────────────────────────────────────────────────────

        private void LoadAll()
        {
            LoadDirectory<Definitions.VehicleDefinition>("res://data/vehicles/", _vehicles, d => d.VehicleId);
            LoadDirectory<Definitions.WeaponDefinition>("res://data/weapons/",   _weapons,  d => d.WeaponId);
            LoadDirectory<Definitions.FactionDefinition>("res://data/factions/", _factions, d => d.FactionId);
            LoadDirectory<Definitions.DistrictDefinition>("res://data/districts/", _districts, d => d.DistrictId);
            LoadDirectory<Definitions.MissionDefinition>("res://data/missions/", _missions, d => d.MissionId);
        }

        private void LoadDirectory<T>(string resPath, Dictionary<string, T> cache, System.Func<T, string> idSelector)
        {
            using var dir = DirAccess.Open(resPath);
            if (dir == null)
            {
                GD.PrintErr($"[DataRegistry] Directory not found: {resPath}");
                return;
            }

            dir.ListDirBegin();
            string fileName;
            while ((fileName = dir.GetNext()) != string.Empty)
            {
                if (!fileName.EndsWith(".json")) continue;
                string fullPath = resPath + fileName;
                var fileAccess = FileAccess.Open(fullPath, FileAccess.ModeFlags.Read);
                if (fileAccess == null)
                {
                    GD.PrintErr($"[DataRegistry] Cannot open: {fullPath}");
                    continue;
                }

                string json = fileAccess.GetAsText();
                fileAccess.Close();

                try
                {
                    var def = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (def != null)
                    {
                        string id = idSelector(def);
                        cache[id] = def;
                        GD.Print($"[DataRegistry] Loaded {typeof(T).Name}: {id}");
                    }
                }
                catch (System.Exception ex)
                {
                    GD.PrintErr($"[DataRegistry] Failed to parse {fullPath}: {ex.Message}");
                }
            }
            dir.ListDirEnd();
        }
    }
}
