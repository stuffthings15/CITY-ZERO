using System.Collections.Generic;
using UnityEngine;
using CityZero.Core.Utilities;
using CityZero.Data.RuntimeDTOs;

namespace CityZero.Data.Importers
{
    public sealed class ConfigDatabase : MonoBehaviour
    {
        [Header("District Configs")]
        [SerializeField] private TextAsset[] _districtFiles;

        [Header("Faction Configs")]
        [SerializeField] private TextAsset[] _factionFiles;

        [Header("Vehicle Configs")]
        [SerializeField] private TextAsset[] _vehicleFiles;

        [Header("Weapon Configs")]
        [SerializeField] private TextAsset[] _weaponFiles;

        [Header("Shop Configs")]
        [SerializeField] private TextAsset[] _shopFiles;

        [Header("Event Configs")]
        [SerializeField] private TextAsset[] _eventFiles;

        [Header("Mission Configs")]
        [SerializeField] private TextAsset[] _missionFiles;

        public readonly Dictionary<string, DistrictData> Districts = new();
        public readonly Dictionary<string, FactionData> Factions = new();
        public readonly Dictionary<string, VehicleData> Vehicles = new();
        public readonly Dictionary<string, WeaponData> Weapons = new();
        public readonly Dictionary<string, ShopData> Shops = new();
        public readonly Dictionary<string, WorldEventData> Events = new();
        public readonly Dictionary<string, MissionData> Missions = new();

        public void LoadAll()
        {
            LoadCollection(_districtFiles, Districts);
            LoadCollection(_factionFiles, Factions);
            LoadCollection(_vehicleFiles, Vehicles);
            LoadCollection(_weaponFiles, Weapons);
            LoadCollection(_shopFiles, Shops);
            LoadCollection(_eventFiles, Events);
            LoadCollection(_missionFiles, Missions);

            Debug.Log($"ConfigDatabase loaded: {Districts.Count} districts, {Factions.Count} factions, {Vehicles.Count} vehicles, {Weapons.Count} weapons, {Shops.Count} shops, {Events.Count} events, {Missions.Count} missions.");
        }

        private static void LoadCollection<T>(TextAsset[] files, Dictionary<string, T> target) where T : class
        {
            target.Clear();

            if (files == null)
            {
                return;
            }

            foreach (TextAsset file in files)
            {
                if (file == null)
                {
                    continue;
                }

                T data = JsonFileUtility.LoadFromTextAsset<T>(file);
                string id = GetId(data);

                if (string.IsNullOrWhiteSpace(id))
                {
                    Debug.LogWarning($"Config asset {file.name} has no id field.");
                    continue;
                }

                target[id] = data;
            }
        }

        private static string GetId<T>(T data)
        {
            var field = typeof(T).GetField("id");
            return field?.GetValue(data) as string;
        }
    }
}
