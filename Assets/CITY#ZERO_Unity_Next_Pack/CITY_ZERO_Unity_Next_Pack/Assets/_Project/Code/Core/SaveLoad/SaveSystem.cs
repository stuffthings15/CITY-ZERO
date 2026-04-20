using System;
using System.IO;
using UnityEngine;
using CityZero.Core.Utilities;

namespace CityZero.Core.SaveLoad
{
    public sealed class SaveSystem
    {
        private readonly string _saveDirectory;

        public SaveSystem(string saveDirectory = null)
        {
            _saveDirectory = string.IsNullOrWhiteSpace(saveDirectory)
                ? Path.Combine(Application.persistentDataPath, "Saves")
                : saveDirectory;
        }

        public void Save(string slotId, SaveGameData data)
        {
            string path = GetPath(slotId);
            JsonFileUtility.SaveToFile(path, data, true);
        }

        public SaveGameData Load(string slotId)
        {
            string path = GetPath(slotId);
            if (!File.Exists(path))
            {
                return null;
            }

            return JsonFileUtility.LoadFromFile<SaveGameData>(path);
        }

        public bool Exists(string slotId)
        {
            return File.Exists(GetPath(slotId));
        }

        private string GetPath(string slotId)
        {
            if (string.IsNullOrWhiteSpace(slotId))
            {
                throw new ArgumentException("Slot id is null or empty.", nameof(slotId));
            }

            return Path.Combine(_saveDirectory, $"{slotId}.json");
        }
    }
}
