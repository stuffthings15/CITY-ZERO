#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
internal static class ProjectInitializer
{
    static ProjectInitializer()
    {
        // Delay call to allow domain reload
        EditorApplication.delayCall += EnsureBootstrapSceneExists;
    }

    private static void EnsureBootstrapSceneExists()
    {
        // Only run in editor (not in builds)
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        // Check for any scenes in Assets/Scenes
        var scenesFolder = "Assets/Scenes";
        if (!System.IO.Directory.Exists(scenesFolder))
        {
            System.IO.Directory.CreateDirectory(scenesFolder);
            AssetDatabase.Refresh();
        }

        var scenePath = System.IO.Path.Combine(scenesFolder, "Bootstrap.unity");
        if (!System.IO.File.Exists(scenePath))
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Create GameBootstrap gameobject
            var go = new GameObject("GameBootstrap");
            // Try to find the GameBootstrap type
            var gbType = System.Type.GetType("CityZero.Core.Bootstrap.GameBootstrap, Assembly-CSharp");
            if (gbType != null)
            {
                go.AddComponent(gbType);
            }
            else
            {
                // Fallback: try to add by name (assembly will compile when Unity loads)
                go.AddComponent("GameBootstrap");
            }

            // Create a persistent scene object
            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.Refresh();
            Debug.Log("ProjectInitializer: Created Bootstrap.unity and GameBootstrap GameObject.");

            // Add scene to build settings
            var list = EditorBuildSettings.scenes.ToList();
            list.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = list.ToArray();
        }
        else
        {
            // Ensure build settings include the bootstrap scene
            var list = EditorBuildSettings.scenes.ToList();
            if (!list.Exists(s => s.path == scenePath))
            {
                list.Add(new EditorBuildSettingsScene(scenePath, true));
                EditorBuildSettings.scenes = list.ToArray();
            }
        }
    }
}
#endif
