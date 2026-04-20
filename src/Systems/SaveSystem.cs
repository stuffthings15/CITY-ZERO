using Godot;
using System.Collections.Generic;
using System.Text.Json;
using CityZero.Core;
using CityZero.Core.Events;
using CityZero.Systems;

namespace CityZero.Systems
{
    /// <summary>
    /// Persists and restores all game state.
    /// Uses atomic writes (write to .tmp then rename) to prevent corruption on crash.
    /// Save slots: 0, 1, 2 = manual; 3 = auto-save.
    /// </summary>
    public partial class SaveSystem : Node
    {
        public const int AUTO_SAVE_SLOT = 3;
        private const string SAVE_DIR = "user://saves/";

        public override void _Ready()
        {
            ServiceLocator.Register<SaveSystem>(this);
            DirAccess.MakeDirRecursiveAbsolute(SAVE_DIR);
        }

        // ── Public API ────────────────────────────────────────────────────────────

        public bool SaveGame(int slot, SaveData data)
        {
            string path = GetSlotPath(slot);
            string tmpPath = path + ".tmp";

            try
            {
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

                var file = FileAccess.Open(tmpPath, FileAccess.ModeFlags.Write);
                if (file == null) { GD.PrintErr($"[SaveSystem] Cannot write to {tmpPath}"); return false; }
                file.StoreString(json);
                file.Close();

                // Atomic rename — if this succeeds, the save is valid
                if (FileAccess.FileExists(path)) DirAccess.RemoveAbsolute(path);
                DirAccess.RenameAbsolute(tmpPath, path);

                GD.Print($"[SaveSystem] Game saved to slot {slot}.");
                return true;
            }
            catch (System.Exception ex)
            {
                GD.PrintErr($"[SaveSystem] Save failed: {ex.Message}");
                if (FileAccess.FileExists(tmpPath)) DirAccess.RemoveAbsolute(tmpPath);
                return false;
            }
        }

        public SaveData LoadGame(int slot)
        {
            string path = GetSlotPath(slot);
            if (!FileAccess.FileExists(path))
            {
                GD.Print($"[SaveSystem] No save in slot {slot}.");
                return null;
            }

            try
            {
                var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
                string json = file.GetAsText();
                file.Close();

                var data = JsonSerializer.Deserialize<SaveData>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (data == null) throw new System.Exception("Deserialized to null.");

                GD.Print($"[SaveSystem] Loaded slot {slot}. Version: {data.SaveVersion}");
                return data;
            }
            catch (System.Exception ex)
            {
                GD.PrintErr($"[SaveSystem] Load failed for slot {slot}: {ex.Message}. Falling back to new game.");
                return null;
            }
        }

        public bool SlotExists(int slot) => FileAccess.FileExists(GetSlotPath(slot));

        public void DeleteSlot(int slot)
        {
            string path = GetSlotPath(slot);
            if (FileAccess.FileExists(path))
            {
                DirAccess.RemoveAbsolute(path);
                GD.Print($"[SaveSystem] Slot {slot} deleted.");
            }
        }

        // ── Private ───────────────────────────────────────────────────────────────

        private static string GetSlotPath(int slot) => $"{SAVE_DIR}save_slot_{slot}.json";
    }

    // ── Save Data Structure ───────────────────────────────────────────────────────

    public class SaveData
    {
        public string SaveVersion { get; set; } = "1.0.0";
        public double Timestamp { get; set; }
        public double PlaytimeSeconds { get; set; }
        public PlayerSaveData Player { get; set; }
        public WorldSaveData World { get; set; }
        public MissionSaveData Missions { get; set; }
    }

    public class PlayerSaveData
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public string District { get; set; }
        public float Health { get; set; }
        public float Armor { get; set; }
        public float WalletVtek { get; set; }
        public Dictionary<string, int[]> UpgradeTree { get; set; }
        public Dictionary<string, float> SkillCounters { get; set; }
        public List<InventoryItem> Inventory { get; set; }
    }

    public class InventoryItem
    {
        public string ItemId { get; set; }
        public int AmmoLoaded { get; set; }
        public int AmmoReserve { get; set; }
        public int Quantity { get; set; }
    }

    public class WorldSaveData
    {
        public float TimeOfDay { get; set; }
        public string Weather { get; set; }
        public float HeatLevel { get; set; }
        public Dictionary<string, float> FactionReputations { get; set; }
        public Dictionary<string, string> DistrictControl { get; set; }
        public List<string> SafehousesOwned { get; set; }
        public HashSet<string> WorldFlags { get; set; }
        public Dictionary<string, long> EventCooldowns { get; set; }
    }

    public class MissionSaveData
    {
        public List<string> Completed { get; set; }
        public List<string> Failed { get; set; }
        public List<string> Available { get; set; }
    }
}
