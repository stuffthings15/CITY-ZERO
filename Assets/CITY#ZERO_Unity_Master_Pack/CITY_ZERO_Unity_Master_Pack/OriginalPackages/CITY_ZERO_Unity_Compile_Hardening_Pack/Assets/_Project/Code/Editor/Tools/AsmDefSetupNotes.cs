using UnityEditor;
using UnityEngine;

namespace CityZero.Editor.Tools
{
    public static class AsmDefSetupNotes
    {
        [MenuItem("CityZero/Tools/Print AsmDef Notes")]
        public static void PrintNotes()
        {
            Debug.Log("Place asmdefs from Assets/_Project/AsmDef into matching code folders or duplicate per folder as needed. Update references if files are moved.");
        }
    }
}
