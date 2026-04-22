#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Runs automatically every time Unity loads or scripts recompile.
/// Ensures scenes exist, build settings are correct, and the GameWorld
/// scene is open and ready to Play — zero manual steps required.
/// </summary>
[InitializeOnLoad]
internal static class AutoSetup
{
    private const string GameWorldPath  = "Assets/Scenes/GameWorld.unity";
    private const string BootstrapPath  = "Assets/Scenes/Bootstrap.unity";
    private const string ScenesFolder   = "Assets/Scenes";
    private const string DoneKey        = "AutoSetup_WorldBuilt";

    static AutoSetup()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanging;
        EditorApplication.delayCall += RunOnce;
    }

    /// <summary>Called by DebuggerWindow to force a full scene rebuild.</summary>
    public static void ForceRebuild()
    {
        EnsureScenesFolder();
        BuildGameWorld();
        EnsureBuildSettings();
        if (File.Exists(GameWorldPath))
            EditorSceneManager.OpenScene(GameWorldPath);
    }

    private static void OnPlayModeChanging(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode) return;

        // If the game world scene doesn't exist yet, build it before play starts
        if (!File.Exists(GameWorldPath))
        {
            EditorApplication.isPlaying = false; // cancel play
            Debug.LogWarning("[AutoSetup] Building GameWorld.unity first — press Play again in a moment.");
            EditorApplication.delayCall += () =>
            {
                EnsureScenesFolder();
                BuildGameWorld();
                EnsureBuildSettings();
            };
        }
    }

    private static void RunOnce()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        EnsureScenesFolder();
        EnsureBuildSettings();
        EnsureGameWorldScene();
        ForceRecompileIfStale();
    }

    // ── 1. Scenes folder ────────────────────────────────────────────────────
    private static void EnsureScenesFolder()
    {
        if (!Directory.Exists(ScenesFolder))
        {
            Directory.CreateDirectory(ScenesFolder);
            AssetDatabase.Refresh();
        }
    }

    // ── 2. Build settings ───────────────────────────────────────────────────
    private static void EnsureBuildSettings()
    {
        var list    = EditorBuildSettings.scenes.ToList();
        bool dirty  = false;

        // GameWorld first (index 0 = first scene Unity loads)
        if (File.Exists(GameWorldPath) && !list.Any(s => s.path == GameWorldPath))
        {
            list.Insert(0, new EditorBuildSettingsScene(GameWorldPath, true));
            dirty = true;
        }

        if (File.Exists(BootstrapPath) && !list.Any(s => s.path == BootstrapPath))
        {
            list.Add(new EditorBuildSettingsScene(BootstrapPath, true));
            dirty = true;
        }

        if (dirty)
        {
            EditorBuildSettings.scenes = list.ToArray();
            Debug.Log("[AutoSetup] Build settings updated.");
        }
    }

    // ── 3. Create & open GameWorld scene ────────────────────────────────────
    private static void EnsureGameWorldScene()
    {
        if (!File.Exists(GameWorldPath))
        {
            Debug.Log("[AutoSetup] GameWorld.unity not found — building now...");
            BuildGameWorld();
            EnsureBuildSettings();
            return;
        }

        // Open the scene if something else (or nothing) is currently open
        var active = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (active.path != GameWorldPath)
        {
            EditorSceneManager.OpenScene(GameWorldPath);
            Debug.Log("[AutoSetup] Opened GameWorld.unity — ready to Play.");
        }
    }

    // ── 4. Force recompile if any script is newer than last import ──────────
    private static void ForceRecompileIfStale()
    {
        // Touch a temp file to trigger reimport only when scripts have changed
        string stampPath = Path.Combine("Temp", "AutoSetup_stamp.txt");
        string latestScript = GetLatestScriptWriteTime();

        bool needsRecompile = true;
        if (File.Exists(stampPath))
        {
            string stamp = File.ReadAllText(stampPath).Trim();
            needsRecompile = stamp != latestScript;
        }

        if (needsRecompile)
        {
            File.WriteAllText(stampPath, latestScript);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            Debug.Log("[AutoSetup] Scripts refreshed and recompiled.");
        }
    }

    private static string GetLatestScriptWriteTime()
    {
        long ticks = 0;
        foreach (var file in Directory.GetFiles("Assets", "*.cs", SearchOption.AllDirectories))
        {
            long t = File.GetLastWriteTimeUtc(file).Ticks;
            if (t > ticks) ticks = t;
        }
        return ticks.ToString();
    }

    // ── World builder ────────────────────────────────────────────────────────
    private static void BuildGameWorld()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var root = new GameObject("World");
        BuildDistricts(root);
        BuildRoads(root);
        BuildBuildings(root);
        BuildLighting();

        var player = BuildPlayer();
        BuildCamera(player.transform);

        // GameBootstrap
        var gbType = Type.GetType("CityZero.Core.Bootstrap.GameBootstrap, Assembly-CSharp");
        if (gbType != null)
            new GameObject("GameBootstrap").AddComponent(gbType);

        EditorSceneManager.SaveScene(scene, GameWorldPath);
        AssetDatabase.Refresh();
        EnsureBuildSettings();

        Debug.Log("[AutoSetup] GameWorld.unity built successfully. Press Play!");
    }

    // ── Districts ─────────────────────────────────────────────────────────────
    private static readonly (string Name, Color Ground, Color Accent, Vector2 Grid)[] Districts =
    {
        ("The Pits",       new Color(0.545f,0.271f,0.075f), new Color(1f,0.4f,0f),      new Vector2(0,0)),
        ("The Grid",       new Color(0.290f,0.435f,0.647f), new Color(0.91f,0.92f,0.94f),new Vector2(1,0)),
        ("Neon Flats",     new Color(0.051f,0.051f,0.102f), new Color(0.545f,0f,1f),     new Vector2(2,0)),
        ("The Waterfront", new Color(0.373f,0.478f,0.478f), new Color(1f,0.549f,0f),     new Vector2(0,1)),
        ("The Spire",      new Color(0.290f,0.290f,0.353f), new Color(0.769f,0.071f,0.188f),new Vector2(1,1)),
    };

    private const float DistrictSize = 120f;
    private const float RoadSpacing  =  30f;
    private const float RoadWidth    =   8f;

    private static Vector3 Center(Vector2 g) => new Vector3(g.x * DistrictSize, 0f, g.y * DistrictSize);

    private static void BuildDistricts(GameObject root)
    {
        var dr = new GameObject("Districts");
        dr.transform.parent = root.transform;

        foreach (var d in Districts)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            go.name = d.Name;
            go.transform.parent       = dr.transform;
            go.transform.localPosition= Center(d.Grid);
            go.transform.localScale   = new Vector3(DistrictSize / 10f, 1f, DistrictSize / 10f);
            var mat = new Material(Shader.Find("Standard")) { color = d.Ground };
            go.GetComponent<Renderer>().sharedMaterial = mat;
            UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());

            var label = new GameObject("Label");
            label.transform.parent        = go.transform;
            label.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            label.transform.localScale    = Vector3.one * 0.05f;
            var tm = label.AddComponent<TextMesh>();
            tm.text = d.Name.ToUpper(); tm.fontSize = 96;
            tm.color = d.Accent; tm.anchor = TextAnchor.MiddleCenter;
        }
    }

    private static void BuildRoads(GameObject root)
    {
        var rr  = new GameObject("Roads");
        rr.transform.parent = root.transform;
        var mat = new Material(Shader.Find("Standard")) { color = new Color(0.15f, 0.15f, 0.15f) };
        int i   = 0;
        float half = DistrictSize * 0.5f;

        foreach (var d in Districts)
        {
            var c = Center(d.Grid);
            for (float z = -half + RoadSpacing; z < half; z += RoadSpacing)
                MakeRoad(rr, mat, i++, c + new Vector3(0, 0.01f, z),
                    new Vector3(DistrictSize / 10f, 1f, RoadWidth / 10f));
            for (float x = -half + RoadSpacing; x < half; x += RoadSpacing)
                MakeRoad(rr, mat, i++, c + new Vector3(x, 0.01f, 0),
                    new Vector3(RoadWidth / 10f, 1f, DistrictSize / 10f));
        }
    }

    private static void MakeRoad(GameObject parent, Material mat, int idx, Vector3 pos, Vector3 scale)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        go.name = $"Road_{idx}";
        go.transform.parent = parent.transform;
        go.transform.position = pos;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
    }

    private static void BuildBuildings(GameObject root)
    {
        var br = new GameObject("Buildings");
        br.transform.parent = root.transform;
        var rng = new System.Random(42);
        int i   = 0;

        foreach (var d in Districts)
        {
            var mat = new Material(Shader.Find("Standard"))
                { color = Color.Lerp(d.Ground, Color.black, 0.35f) };
            var c   = Center(d.Grid);
            for (int row = 0; row < 3; row++)
            for (int col = 0; col < 3; col++)
            {
                float h = 4f + (float)rng.NextDouble() * 16f;
                var go  = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = $"Building_{i++}";
                go.transform.parent   = br.transform;
                go.transform.position = c + new Vector3(
                    -DistrictSize * 0.3f + col * RoadSpacing, h * 0.5f,
                    -DistrictSize * 0.3f + row * RoadSpacing);
                go.transform.localScale = new Vector3(10f, h, 10f);
                go.GetComponent<Renderer>().sharedMaterial = mat;
            }
        }
    }

    private static GameObject BuildPlayer()
    {
        var player  = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag  = "Player";
        player.transform.position = Center(Districts[0].Grid) + new Vector3(0f, 1f, 0f);
        player.GetComponent<Renderer>().sharedMaterial =
            new Material(Shader.Find("Standard")) { color = Color.yellow };

        var rb = player.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.drag        = 8f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        player.AddComponent<FallbackPlayerMover>();

        return player;
    }

    private static void BuildCamera(Transform target)
    {
        var go  = new GameObject("Main Camera");
        go.tag  = "MainCamera";
        go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        go.transform.position = target.position + new Vector3(0f, 80f, 0f);

        var cam = go.AddComponent<Camera>();
        cam.orthographic     = true;
        cam.orthographicSize = 40f;
        cam.clearFlags       = CameraClearFlags.SolidColor;
        cam.backgroundColor  = new Color(0.05f, 0.05f, 0.05f);
        cam.farClipPlane     = 200f;

        go.AddComponent<AudioListener>();
        var follow = go.AddComponent<TopDownCameraFollow>();
        follow.SetTarget(target);
    }

    private static void BuildLighting()
    {
        var go  = new GameObject("Sun");
        var l   = go.AddComponent<Light>();
        l.type  = LightType.Directional;
        l.intensity = 1.2f;
        l.color = new Color(1f, 0.95f, 0.8f);
        go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        RenderSettings.ambientMode  = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.2f, 0.2f, 0.25f);
        RenderSettings.fog          = false;
    }
}
#endif
