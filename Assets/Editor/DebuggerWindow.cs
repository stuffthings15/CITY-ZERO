#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Visual debugger window. Open via CITY#ZERO > Debugger.
/// Shows all Unity compiler errors, scene state, and provides one-click fixes.
/// </summary>
public class DebuggerWindow : EditorWindow
{
    private Vector2 _scroll;
    private List<LogEntry> _errors   = new();
    private List<LogEntry> _warnings = new();
    private string _logPath;
    private double _lastRefresh;
    private bool _showWarnings;

    private static readonly Color ErrorColor   = new Color(1f, 0.4f, 0.4f);
    private static readonly Color WarnColor    = new Color(1f, 0.85f, 0.3f);
    private static readonly Color OkColor      = new Color(0.4f, 1f, 0.5f);
    private static readonly Color HeaderColor  = new Color(0.15f, 0.15f, 0.18f);

    [MenuItem("CITY#ZERO/Debugger %#d")] // Ctrl+Shift+D
    public static void Open()
    {
        var w = GetWindow<DebuggerWindow>("CITY#ZERO Debugger");
        w.minSize = new Vector2(520, 400);
        w.Refresh();
    }

    private void OnEnable()
    {
        _logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Unity", "Editor", "Editor.log");
        Refresh();
    }

    private void OnFocus()  => Refresh();

    private void OnGUI()
    {
        // Auto-refresh every 2 seconds
        if (EditorApplication.timeSinceStartup - _lastRefresh > 2.0)
            Refresh();

        DrawHeader();
        DrawStatus();
        DrawActions();
        DrawLogs();
    }

    // ── Header ────────────────────────────────────────────────────────────────
    private void DrawHeader()
    {
        EditorGUI.DrawRect(new Rect(0, 0, position.width, 36), HeaderColor);
        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        GUILayout.Space(8);
        GUILayout.Label("CITY#ZERO  //  Debugger", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("↻ Refresh", GUILayout.Width(80))) Refresh();
        GUILayout.Space(8);
        GUILayout.EndHorizontal();
        GUILayout.Space(4);
    }

    // ── Status panel ─────────────────────────────────────────────────────────
    private void DrawStatus()
    {
        GUILayout.Label("── Project Status ──────────────────────", EditorStyles.miniLabel);
        GUILayout.BeginVertical("box");

        StatusRow("Compiler errors",    _errors.Count,   _errors.Count   == 0);
        StatusRow("Compiler warnings",  _warnings.Count, _warnings.Count == 0);
        StatusRow("GameWorld.unity",     SceneExists("GameWorld"), SceneExists("GameWorld"));
        StatusRow("Bootstrap.unity",     SceneExists("Bootstrap"), SceneExists("Bootstrap"));
        StatusRow("Build settings",      BuildSettingsCount(), BuildSettingsCount() > 0);
        StatusRow("Active scene",        ActiveSceneName(), ActiveSceneName() == "GameWorld");
        StatusRow("Player in scene",     PlayerInScene() ? "YES" : "NO", PlayerInScene());
        StatusRow("Camera in scene",     CameraInScene() ? "YES" : "NO", CameraInScene());

        GUILayout.EndVertical();
    }

    private void StatusRow(string label, object value, bool ok)
    {
        GUILayout.BeginHorizontal();
        var prev = GUI.color;
        GUI.color = ok ? OkColor : ErrorColor;
        GUILayout.Label(ok ? "✔" : "✘", GUILayout.Width(18));
        GUI.color = prev;
        GUILayout.Label(label, GUILayout.Width(160));
        GUILayout.Label(value?.ToString() ?? "—", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();
    }

    // ── Action buttons ────────────────────────────────────────────────────────
    private void DrawActions()
    {
        GUILayout.Space(4);
        GUILayout.Label("── Actions ─────────────────────────────", EditorStyles.miniLabel);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("⚡ Fix Everything", GUILayout.Height(32)))
            FixEverything();

        if (GUILayout.Button("🔨 Rebuild World Scene", GUILayout.Height(32)))
            RebuildWorld();

        if (GUILayout.Button("▶ Open & Play", GUILayout.Height(32)))
            OpenAndPlay();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Force Recompile"))
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }
        if (GUILayout.Button("Clear Missing Scripts"))
            ClearMissingScripts();
        if (GUILayout.Button(_showWarnings ? "Hide Warnings" : "Show Warnings"))
            _showWarnings = !_showWarnings;
        GUILayout.EndHorizontal();
        GUILayout.Space(4);
    }

    // ── Log display ───────────────────────────────────────────────────────────
    private void DrawLogs()
    {
        var toShow = new List<LogEntry>(_errors);
        if (_showWarnings) toShow.AddRange(_warnings);

        int count = toShow.Count;
        GUILayout.Label($"── Errors / Warnings ({count}) ─────────────────",
            EditorStyles.miniLabel);

        _scroll = GUILayout.BeginScrollView(_scroll);

        if (count == 0)
        {
            var prev = GUI.color;
            GUI.color = OkColor;
            GUILayout.Label("  No errors found  ✔", EditorStyles.boldLabel);
            GUI.color = prev;
        }
        else
        {
            foreach (var e in toShow)
            {
                var prev = GUI.color;
                GUI.color = e.IsError ? ErrorColor : WarnColor;
                GUILayout.BeginVertical("box");
                GUI.color = prev;
                GUILayout.Label(e.File, EditorStyles.miniLabel);
                GUILayout.Label(e.Message, EditorStyles.wordWrappedLabel);
                GUILayout.EndVertical();
                GUILayout.Space(2);
            }
        }

        GUILayout.EndScrollView();
    }

    // ── Fix Everything ────────────────────────────────────────────────────────
    private static void FixEverything()
    {
        // 1. Force recompile
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

        // 2. Ensure scenes folder
        if (!Directory.Exists("Assets/Scenes"))
        {
            Directory.CreateDirectory("Assets/Scenes");
            AssetDatabase.Refresh();
        }

        // 3. Rebuild world if missing
        if (!File.Exists("Assets/Scenes/GameWorld.unity"))
            RebuildWorld();

        // 4. Fix build settings
        FixBuildSettings();

        // 5. Open the scene
        if (File.Exists("Assets/Scenes/GameWorld.unity"))
            EditorSceneManager.OpenScene("Assets/Scenes/GameWorld.unity");

        Debug.Log("[Debugger] Fix Everything complete.");
    }

    private static void RebuildWorld()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("[Debugger] Stop Play mode before rebuilding.");
            return;
        }

        // Delete stale scene
        if (File.Exists("Assets/Scenes/GameWorld.unity"))
        {
            File.Delete("Assets/Scenes/GameWorld.unity");
            string meta = "Assets/Scenes/GameWorld.unity.meta";
            if (File.Exists(meta)) File.Delete(meta);
            AssetDatabase.Refresh();
        }

        // Trigger AutoSetup to rebuild
        AutoSetup.ForceRebuild();
    }

    private static void OpenAndPlay()
    {
        if (!File.Exists("Assets/Scenes/GameWorld.unity"))
        {
            RebuildWorld();
            return;
        }
        EditorSceneManager.OpenScene("Assets/Scenes/GameWorld.unity");
        EditorApplication.isPlaying = true;
    }

    private static void FixBuildSettings()
    {
        var list  = EditorBuildSettings.scenes.ToList();
        bool dirty = false;

        string gw = "Assets/Scenes/GameWorld.unity";
        string bs = "Assets/Scenes/Bootstrap.unity";

        if (File.Exists(gw) && !list.Any(s => s.path == gw))
        { list.Insert(0, new EditorBuildSettingsScene(gw, true)); dirty = true; }

        if (File.Exists(bs) && !list.Any(s => s.path == bs))
        { list.Add(new EditorBuildSettingsScene(bs, true)); dirty = true; }

        if (dirty) EditorBuildSettings.scenes = list.ToArray();
    }

    private static void ClearMissingScripts()
    {
        int count = 0;
        var gos   = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in gos)
        {
            var components = go.GetComponents<Component>();
            foreach (var c in components)
                if (c == null) { count++; }

            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
        }
        Debug.Log($"[Debugger] Removed missing scripts from {count} components.");
    }

    // ── Log parsing ───────────────────────────────────────────────────────────
    private void Refresh()
    {
        _lastRefresh = EditorApplication.timeSinceStartup;
        _errors.Clear();
        _warnings.Clear();

        if (!File.Exists(_logPath)) return;

        try
        {
            string[] lines;
            // Read with shared access so Unity doesn't block us
            using (var fs = new FileStream(_logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs))
            {
                lines = sr.ReadToEnd()
                          .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            }

            // Parse only the most recent compilation block
            int lastCompile = 0;
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                if (lines[i].Contains("CompileScripts") || lines[i].Contains("Begin MonoManager ReloadAssembly"))
                { lastCompile = i; break; }
            }

            var errorRx   = new Regex(@"(.+\.cs)\((\d+),(\d+)\): (error \w+): (.+)");
            var warningRx = new Regex(@"(.+\.cs)\((\d+),(\d+)\): (warning \w+): (.+)");

            for (int i = lastCompile; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                var em   = errorRx.Match(line);
                if (em.Success)
                {
                    _errors.Add(new LogEntry
                    {
                        IsError  = true,
                        File     = $"{Path.GetFileName(em.Groups[1].Value)}  line {em.Groups[2].Value}",
                        Message  = $"{em.Groups[4].Value}: {em.Groups[5].Value}"
                    });
                    continue;
                }
                var wm = warningRx.Match(line);
                if (wm.Success)
                {
                    _warnings.Add(new LogEntry
                    {
                        IsError  = false,
                        File     = $"{Path.GetFileName(wm.Groups[1].Value)}  line {wm.Groups[2].Value}",
                        Message  = $"{wm.Groups[4].Value}: {wm.Groups[5].Value}"
                    });
                }
            }
        }
        catch { /* log locked — skip this refresh */ }

        Repaint();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private static bool   SceneExists(string name)  => File.Exists($"Assets/Scenes/{name}.unity");
    private static int    BuildSettingsCount()       => EditorBuildSettings.scenes.Length;
    private static string ActiveSceneName()          => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    private static bool   PlayerInScene()            => GameObject.FindWithTag("Player") != null;
    private static bool   CameraInScene()            => Camera.main != null;

    private class LogEntry
    {
        public bool   IsError;
        public string File;
        public string Message;
    }
}
#endif
