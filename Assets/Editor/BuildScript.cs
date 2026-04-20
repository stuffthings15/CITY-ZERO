#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

/// <summary>
/// BuildScript.PerformBuild is the entrypoint expected by the unity-builder GitHub Action used in CI.
/// It collects enabled scenes from EditorBuildSettings and performs a StandaloneWindows64 build to the build/ folder.
/// This file is safe to keep in source control and will be ignored by the WinUI build (Assets/*.cs are excluded).
/// </summary>
public static class BuildScript
{
    public static void PerformBuild()
    {
        var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        if (scenes.Length == 0)
        {
            UnityEngine.Debug.LogError("No enabled scenes found in EditorBuildSettings. Add at least one scene to the build settings.");
            throw new Exception("No enabled scenes found for build.");
        }

        var buildPath = System.IO.Path.Combine("build", "CityZero_Windows.exe");
        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if (report == null)
        {
            UnityEngine.Debug.LogError("BuildPipeline returned null report.");
            throw new Exception("Build failed: null report");
        }

        if (report.summary.result != BuildResult.Succeeded)
        {
            UnityEngine.Debug.LogError($"Build failed: {report.summary.result}. See Editor log for details.");
            throw new Exception($"Build failed: {report.summary.result}");
        }

        UnityEngine.Debug.Log($"Build succeeded: {report.summary.totalSize} bytes written to {buildPath}");
    }
}
#endif
