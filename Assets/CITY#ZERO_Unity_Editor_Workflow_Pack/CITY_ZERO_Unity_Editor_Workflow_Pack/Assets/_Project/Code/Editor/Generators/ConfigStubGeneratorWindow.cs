using System.IO;
using UnityEditor;
using UnityEngine;

namespace CityZero.Editor.Generators
{
    public sealed class ConfigStubGeneratorWindow : EditorWindow
    {
        private string _id = "new_config";
        private string _displayName = "New Config";
        private ConfigType _configType = ConfigType.Vehicle;
        private string _saveRoot = "Assets/_Project/Data/Config";

        private enum ConfigType
        {
            District,
            Faction,
            Vehicle,
            Weapon,
            Shop,
            Event,
            Mission
        }

        [MenuItem("CityZero/Generators/Config Stub Generator")]
        public static void Open()
        {
            GetWindow<ConfigStubGeneratorWindow>("Config Stub Generator");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Config Stub Generator", EditorStyles.boldLabel);

            _configType = (ConfigType)EditorGUILayout.EnumPopup("Config Type", _configType);
            _id = EditorGUILayout.TextField("Id", _id);
            _displayName = EditorGUILayout.TextField("Display Name", _displayName);
            _saveRoot = EditorGUILayout.TextField("Save Root", _saveRoot);

            if (GUILayout.Button("Generate Stub"))
            {
                GenerateStub();
            }
        }

        private void GenerateStub()
        {
            string folder = Path.Combine(_saveRoot, GetFolderName(_configType));
            Directory.CreateDirectory(folder);

            string json = BuildJson(_configType, _id, _displayName);
            string path = Path.Combine(folder, $"{_id}.json");
            File.WriteAllText(path, json);
            AssetDatabase.Refresh();

            Debug.Log($"Generated config stub: {path}");
        }

        private static string GetFolderName(ConfigType type)
        {
            return type switch
            {
                ConfigType.District => "districts",
                ConfigType.Faction => "factions",
                ConfigType.Vehicle => "vehicles",
                ConfigType.Weapon => "weapons",
                ConfigType.Shop => "shops",
                ConfigType.Event => "events",
                ConfigType.Mission => "missions",
                _ => "misc"
            };
        }

        private static string BuildJson(ConfigType type, string id, string displayName)
        {
            return type switch
            {
                ConfigType.District => $@"{{""id"":""{id}"",""displayName"":""{displayName}"",""theme"":""theme_here""}}",
                ConfigType.Faction => $@"{{""id"":""{id}"",""displayName"":""{displayName}"",""archetype"":""archetype_here""}}",
                ConfigType.Vehicle => $@"{{""id"":""{id}"",""displayName"":""{displayName}"",""class"":""compact"",""maxSpeed"":40.0}}",
                ConfigType.Weapon => $@"{{""id"":""{id}"",""displayName"":""{displayName}"",""category"":""pistol"",""damage"":10}}",
                ConfigType.Shop => $@"{{""id"":""{id}"",""displayName"":""{displayName}"",""shopType"":""black_market""}}",
                ConfigType.Event => $@"{{""id"":""{id}"",""displayName"":""{displayName}"",""eventType"":""moving_opportunity""}}",
                ConfigType.Mission => $@"{{""id"":""{id}"",""displayName"":""{displayName}"",""missionType"":""courier_escape""}}",
                _ => "{}"
            };
        }
    }
}
