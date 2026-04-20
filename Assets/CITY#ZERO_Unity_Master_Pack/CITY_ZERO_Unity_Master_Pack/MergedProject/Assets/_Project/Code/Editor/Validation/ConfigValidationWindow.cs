using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CityZero.Editor.Validation
{
    public sealed class ConfigValidationWindow : EditorWindow
    {
        private Vector2 _scroll;
        private readonly List<string> _messages = new();

        [MenuItem("CityZero/Validation/Config Validation Window")]
        public static void Open()
        {
            GetWindow<ConfigValidationWindow>("Config Validation");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("CITY//ZERO Config Validation", EditorStyles.boldLabel);

            if (GUILayout.Button("Run Validation"))
            {
                RunValidation();
            }

            EditorGUILayout.Space();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (string message in _messages)
            {
                EditorGUILayout.HelpBox(message, MessageType.Info);
            }
            EditorGUILayout.EndScrollView();
        }

        private void RunValidation()
        {
            _messages.Clear();

            string[] jsonGuids = AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets/_Project/Data/Config" });
            _messages.Add($"Found {jsonGuids.Length} TextAsset config files.");

            foreach (string guid in jsonGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                if (asset == null || string.IsNullOrWhiteSpace(asset.text))
                {
                    _messages.Add($"Invalid or empty config: {path}");
                }
            }

            if (_messages.Count == 1)
            {
                _messages.Add("No obvious empty config files detected.");
            }
        }
    }
}
