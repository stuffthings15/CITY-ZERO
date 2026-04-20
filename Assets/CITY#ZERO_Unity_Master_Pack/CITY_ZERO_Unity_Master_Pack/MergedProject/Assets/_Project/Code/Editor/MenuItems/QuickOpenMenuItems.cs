using UnityEditor;

namespace CityZero.Editor.MenuItems
{
    public static class QuickOpenMenuItems
    {
        [MenuItem("CityZero/Quick Open/Mission Authoring")]
        public static void OpenMissionAuthoring()
        {
            EditorApplication.ExecuteMenuItem("CityZero/Windows/Mission Authoring");
        }

        [MenuItem("CityZero/Quick Open/Config Stub Generator")]
        public static void OpenConfigStubGenerator()
        {
            EditorApplication.ExecuteMenuItem("CityZero/Generators/Config Stub Generator");
        }

        [MenuItem("CityZero/Quick Open/Scene Validation")]
        public static void OpenSceneValidation()
        {
            EditorApplication.ExecuteMenuItem("CityZero/Validation/Scene Validation Window");
        }
    }
}
