using System.IO;
using UnityEditor;
using UnityEngine;

namespace CityZero.Editor.Tools
{
    public static class ConfigAuditTool
    {
        [MenuItem("CityZero/Tools/Run Config Audit")]
        public static void RunAudit()
        {
            string root = Path.Combine(Application.dataPath, "_Project/Data/Config");
            if (!Directory.Exists(root))
            {
                Debug.LogWarning($"Config root missing: {root}");
                return;
            }

            string[] files = Directory.GetFiles(root, "*.json", SearchOption.AllDirectories);
            Debug.Log($"Config audit: found {files.Length} JSON config files.");
        }
    }
}
