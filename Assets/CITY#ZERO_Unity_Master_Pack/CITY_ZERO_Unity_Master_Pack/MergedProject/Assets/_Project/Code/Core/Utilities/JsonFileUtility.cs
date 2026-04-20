using System;
using System.IO;
using UnityEngine;

namespace CityZero.Core.Utilities
{
    public static class JsonFileUtility
    {
        public static T LoadFromTextAsset<T>(TextAsset textAsset)
        {
            if (textAsset == null)
            {
                throw new ArgumentNullException(nameof(textAsset));
            }

            return JsonUtility.FromJson<T>(textAsset.text);
        }

        public static T LoadFromFile<T>(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                throw new ArgumentException("Path is null or empty.", nameof(fullPath));
            }

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("JSON file not found.", fullPath);
            }

            string json = File.ReadAllText(fullPath);
            return JsonUtility.FromJson<T>(json);
        }

        public static void SaveToFile<T>(string fullPath, T value, bool prettyPrint = true)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                throw new ArgumentException("Path is null or empty.", nameof(fullPath));
            }

            string directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonUtility.ToJson(value, prettyPrint);
            File.WriteAllText(fullPath, json);
        }
    }
}
