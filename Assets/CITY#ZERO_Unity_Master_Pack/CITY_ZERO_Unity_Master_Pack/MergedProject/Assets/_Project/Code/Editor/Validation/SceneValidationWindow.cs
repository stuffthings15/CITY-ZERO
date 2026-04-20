using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CityZero.Editor.Validation
{
    public sealed class SceneValidationWindow : EditorWindow
    {
        private Vector2 _scroll;
        private readonly List<string> _messages = new();

        [MenuItem("CityZero/Validation/Scene Validation Window")]
        public static void Open()
        {
            GetWindow<SceneValidationWindow>("Scene Validation");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("CITY//ZERO Scene Validation", EditorStyles.boldLabel);

            if (GUILayout.Button("Validate Open Scenes"))
            {
                ValidateScenes();
            }

            EditorGUILayout.Space();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (string message in _messages)
            {
                EditorGUILayout.HelpBox(message, MessageType.Info);
            }
            EditorGUILayout.EndScrollView();
        }

        private void ValidateScenes()
        {
            _messages.Clear();

            int loadedCount = SceneManager.sceneCount;
            _messages.Add($"Loaded scenes: {loadedCount}");

            for (int i = 0; i < loadedCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                _messages.Add($"Scene: {scene.name} | Root objects: {scene.rootCount}");
            }

            Object[] bootstrapObjects = Resources.FindObjectsOfTypeAll(typeof(MonoBehaviour));
            if (bootstrapObjects == null || bootstrapObjects.Length == 0)
            {
                _messages.Add("No MonoBehaviours found in open scenes.");
            }
            else
            {
                _messages.Add($"MonoBehaviours detected: {bootstrapObjects.Length}");
            }
        }
    }
}
