using UnityEditor;

namespace CityZero.Editor.Inspectors
{
    [CustomEditor(typeof(CityZero.Core.Bootstrap.VerticalSliceGameFlow), true)]
    public sealed class VerticalSliceGameFlowInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("This controller can auto-start the first mission and expose save/test actions.", MessageType.Info);
        }
    }
}
