using UnityEditor;
using UnityEngine;

namespace CityZero.Editor.Windows
{
    public sealed class JsonPathQuickReferenceWindow : EditorWindow
    {
        [MenuItem("CityZero/Windows/JSON Path Quick Reference")]
        public static void Open()
        {
            GetWindow<JsonPathQuickReferenceWindow>("JSON Paths");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Common Config Paths", EditorStyles.boldLabel);
            EditorGUILayout.TextField("Districts", "Assets/_Project/Data/Config/districts");
            EditorGUILayout.TextField("Factions", "Assets/_Project/Data/Config/factions");
            EditorGUILayout.TextField("Vehicles", "Assets/_Project/Data/Config/vehicles");
            EditorGUILayout.TextField("Weapons", "Assets/_Project/Data/Config/weapons");
            EditorGUILayout.TextField("Shops", "Assets/_Project/Data/Config/shops");
            EditorGUILayout.TextField("Events", "Assets/_Project/Data/Config/events");
            EditorGUILayout.TextField("Missions", "Assets/_Project/Data/Config/missions");
        }
    }
}
