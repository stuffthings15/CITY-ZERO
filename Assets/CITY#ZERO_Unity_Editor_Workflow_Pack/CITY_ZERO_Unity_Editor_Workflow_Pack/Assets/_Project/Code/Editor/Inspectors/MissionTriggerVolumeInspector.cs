using UnityEditor;
using UnityEngine;

namespace CityZero.Editor.Inspectors
{
    [CustomEditor(typeof(CityZero.Gameplay.Missions.MissionTriggerVolume), true)]
    public sealed class MissionTriggerVolumeInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("This trigger starts a mission when the player enters the collider.", MessageType.Info);
        }
    }
}
