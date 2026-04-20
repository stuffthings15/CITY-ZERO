using System.IO;
using UnityEditor;
using UnityEngine;

namespace CityZero.Editor.Windows
{
    public sealed class MissionAuthoringWindow : EditorWindow
    {
        private string _missionId = "mq_new_mission";
        private string _displayName = "New Mission";
        private string _missionType = "courier_escape";
        private string _giver = "ivy_quill";
        private string _district = "old_quarter";
        private string _objectiveType1 = "pickup";
        private string _objectiveTarget1 = "package_dead_drop_terminal";
        private string _objectiveType2 = "deliver";
        private string _objectiveTarget2 = "rooftop_mailbox_07";
        private string _saveFolder = "Assets/_Project/Data/Config/missions";

        [MenuItem("CityZero/Windows/Mission Authoring")]
        public static void Open()
        {
            GetWindow<MissionAuthoringWindow>("Mission Authoring");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Mission JSON Generator", EditorStyles.boldLabel);

            _missionId = EditorGUILayout.TextField("Mission Id", _missionId);
            _displayName = EditorGUILayout.TextField("Display Name", _displayName);
            _missionType = EditorGUILayout.TextField("Mission Type", _missionType);
            _giver = EditorGUILayout.TextField("Giver", _giver);
            _district = EditorGUILayout.TextField("District", _district);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Objective 1", EditorStyles.boldLabel);
            _objectiveType1 = EditorGUILayout.TextField("Type", _objectiveType1);
            _objectiveTarget1 = EditorGUILayout.TextField("Target", _objectiveTarget1);

            EditorGUILayout.LabelField("Objective 2", EditorStyles.boldLabel);
            _objectiveType2 = EditorGUILayout.TextField("Type", _objectiveType2);
            _objectiveTarget2 = EditorGUILayout.TextField("Target", _objectiveTarget2);

            EditorGUILayout.Space();
            _saveFolder = EditorGUILayout.TextField("Save Folder", _saveFolder);

            if (GUILayout.Button("Generate Mission JSON"))
            {
                GenerateMissionJson();
            }
        }

        private void GenerateMissionJson()
        {
            if (string.IsNullOrWhiteSpace(_missionId))
            {
                Debug.LogWarning("Mission id is required.");
                return;
            }

            string json = $@"{{
  ""id"": ""{_missionId}"",
  ""displayName"": ""{_displayName}"",
  ""missionType"": ""{_missionType}"",
  ""giver"": ""{_giver}"",
  ""district"": ""{_district}"",
  ""recommendedTier"": 1,
  ""prerequisites"": {{
    ""storyFlags"": [],
    ""minRep"": []
  }},
  ""objectives"": [
    {{""type"": ""{_objectiveType1}"", ""target"": ""{_objectiveTarget1}"", ""failOnAlert"": false, ""timeLimit"": 0, ""requiredHeatBelow"": 0, ""timeout"": 0}},
    {{""type"": ""{_objectiveType2}"", ""target"": ""{_objectiveTarget2}"", ""failOnAlert"": false, ""timeLimit"": 0, ""requiredHeatBelow"": 0, ""timeout"": 0}}
  ],
  ""rewards"": {{
    ""cash"": 500,
    ""rep"": [],
    ""unlockFlags"": []
  }},
  ""failureConditions"": [""player_dead""],
  ""runtimeModifiers"": {{
    ""spawnExtraPatrols"": false,
    ""civilianDensityMultiplier"": 1.0,
    ""trafficMultiplier"": 1.0
  }}
}}";

            Directory.CreateDirectory(_saveFolder);
            string fullPath = Path.Combine(_saveFolder, $"{_missionId}.json");
            File.WriteAllText(fullPath, json);
            AssetDatabase.Refresh();

            Debug.Log($"Generated mission JSON: {fullPath}");
        }
    }
}
