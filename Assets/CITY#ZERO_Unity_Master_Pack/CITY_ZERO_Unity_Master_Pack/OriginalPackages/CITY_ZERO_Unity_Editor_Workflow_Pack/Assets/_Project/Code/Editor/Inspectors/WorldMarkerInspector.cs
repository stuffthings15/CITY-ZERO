using UnityEditor;

namespace CityZero.Editor.Inspectors
{
    [CustomEditor(typeof(CityZero.Gameplay.Interaction.WorldMarker), true)]
    public sealed class WorldMarkerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Use world markers for safehouses, garages, shops, and mission points.", MessageType.Info);
        }
    }
}
