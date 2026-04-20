using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using FluentAssertions;
using CityZero.Core;
using CityZero.Systems;

namespace CityZero.Tests
{
    /// <summary>
    /// SaveSystem tests. Each test uses an isolated temp directory via the GodotStubs
    /// path mapping ("user://" → %TEMP%/cityzero_test/). Tests clean up after themselves.
    /// </summary>
    public class SaveSystemTests : IDisposable
    {
        private readonly SaveSystem _save;
        // Matches GodotStubs MapPath for "user://"
        private readonly string _saveDir = Path.Combine(
            Path.GetTempPath(), "cityzero_test", "saves");

        public SaveSystemTests()
        {
            ServiceLocator.Clear();
            var bus = new GameBus();
            bus._Ready();

            _save = new SaveSystem();
            _save._Ready();   // creates saves/ dir
        }

        public void Dispose()
        {
            // Clean up all test save files
            if (Directory.Exists(_saveDir))
                foreach (var f in Directory.GetFiles(_saveDir, "save_slot_*.json*"))
                    File.Delete(f);
            ServiceLocator.Clear();
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static SaveData MakeSaveData(string version = "1.0.0", float wallet = 1234f) => new()
        {
            SaveVersion    = version,
            Timestamp      = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            PlaytimeSeconds = 60.0,
            Player = new PlayerSaveData
            {
                PositionX    = 10f,
                PositionY    = 20f,
                District     = "the_pits",
                Health       = 100f,
                Armor        = 50f,
                WalletVtek   = wallet,
                Inventory    = new List<InventoryItem>
                {
                    new() { ItemId = "weapon_voss9_smg", AmmoLoaded = 30, AmmoReserve = 90, Quantity = 1 }
                }
            },
            World = new WorldSaveData
            {
                TimeOfDay          = 14.5f,
                Weather            = "Clear",
                HeatLevel          = 2f,
                FactionReputations = new Dictionary<string, float> { ["ruin_syndicate"] = 45f },
                DistrictControl    = new Dictionary<string, string> { ["the_pits"] = "ruin_syndicate" },
                SafehousesOwned    = new List<string> { "sh_east_block" },
                WorldFlags         = new HashSet<string> { "intro_complete" },
                EventCooldowns     = new Dictionary<string, long>()
            },
            Missions = new MissionSaveData
            {
                Completed = new List<string> { "mission_cold_iron" },
                Failed    = new List<string>(),
                Available = new List<string> { "mission_blood_tax" }
            }
        };

        // ── SaveGame ──────────────────────────────────────────────────────────────

        [Fact]
        public void SaveGame_ReturnsTrue()
            => _save.SaveGame(0, MakeSaveData()).Should().BeTrue();

        [Fact]
        public void SaveGame_CreatesFileOnDisk()
        {
            _save.SaveGame(0, MakeSaveData());
            _save.SlotExists(0).Should().BeTrue();
        }

        [Fact]
        public void SaveGame_NullData_ReturnsFalse()
            => _save.SaveGame(0, null!).Should().BeFalse();

        [Fact]
        public void SaveGame_DifferentSlots_CreatesSeparateFiles()
        {
            _save.SaveGame(0, MakeSaveData());
            _save.SaveGame(1, MakeSaveData());
            _save.SaveGame(2, MakeSaveData());

            _save.SlotExists(0).Should().BeTrue();
            _save.SlotExists(1).Should().BeTrue();
            _save.SlotExists(2).Should().BeTrue();
        }

        // ── LoadGame ──────────────────────────────────────────────────────────────

        [Fact]
        public void LoadGame_NonExistentSlot_ReturnsNull()
            => _save.LoadGame(7).Should().BeNull();

        [Fact]
        public void LoadGame_RoundTrip_PreservesVersion()
        {
            _save.SaveGame(0, MakeSaveData("2.1.0"));
            _save.LoadGame(0)!.SaveVersion.Should().Be("2.1.0");
        }

        [Fact]
        public void LoadGame_RoundTrip_PreservesWallet()
        {
            _save.SaveGame(0, MakeSaveData(wallet: 9876.54f));
            _save.LoadGame(0)!.Player.WalletVtek.Should().BeApproximately(9876.54f, 0.01f);
        }

        [Fact]
        public void LoadGame_RoundTrip_PreservesPosition()
        {
            _save.SaveGame(0, MakeSaveData());
            var loaded = _save.LoadGame(0)!;
            loaded.Player.PositionX.Should().BeApproximately(10f, 0.001f);
            loaded.Player.PositionY.Should().BeApproximately(20f, 0.001f);
        }

        [Fact]
        public void LoadGame_RoundTrip_PreservesFactionRep()
        {
            _save.SaveGame(0, MakeSaveData());
            var reps = _save.LoadGame(0)!.World.FactionReputations;
            reps.Should().ContainKey("ruin_syndicate");
            reps["ruin_syndicate"].Should().BeApproximately(45f, 0.001f);
        }

        [Fact]
        public void LoadGame_RoundTrip_PreservesInventory()
        {
            _save.SaveGame(0, MakeSaveData());
            var inv = _save.LoadGame(0)!.Player.Inventory;
            inv.Should().ContainSingle(i => i.ItemId == "weapon_voss9_smg" && i.AmmoLoaded == 30);
        }

        [Fact]
        public void LoadGame_RoundTrip_PreservesCompletedMissions()
        {
            _save.SaveGame(0, MakeSaveData());
            _save.LoadGame(0)!.Missions.Completed
                .Should().Contain("mission_cold_iron");
        }

        [Fact]
        public void LoadGame_RoundTrip_PreservesWorldFlags()
        {
            _save.SaveGame(0, MakeSaveData());
            _save.LoadGame(0)!.World.WorldFlags
                .Should().Contain("intro_complete");
        }

        [Fact]
        public void LoadGame_RoundTrip_PreservesPlaytime()
        {
            _save.SaveGame(0, MakeSaveData());
            _save.LoadGame(0)!.PlaytimeSeconds.Should().BeApproximately(60.0, 0.001);
        }

        // ── Slot isolation ────────────────────────────────────────────────────────

        [Fact]
        public void Slots_AreIndependent()
        {
            _save.SaveGame(0, MakeSaveData(wallet: 100f));
            _save.SaveGame(1, MakeSaveData(wallet: 200f));

            _save.LoadGame(0)!.Player.WalletVtek.Should().BeApproximately(100f, 0.01f);
            _save.LoadGame(1)!.Player.WalletVtek.Should().BeApproximately(200f, 0.01f);
        }

        [Fact]
        public void AutoSaveSlot_WorksLikeAnyOtherSlot()
        {
            _save.SaveGame(SaveSystem.AUTO_SAVE_SLOT, MakeSaveData(version: "auto"));
            _save.LoadGame(SaveSystem.AUTO_SAVE_SLOT)!.SaveVersion.Should().Be("auto");
        }

        // ── Overwrite ─────────────────────────────────────────────────────────────

        [Fact]
        public void SaveGame_OverwritesExistingSlot()
        {
            _save.SaveGame(0, MakeSaveData(wallet: 100f));
            _save.SaveGame(0, MakeSaveData(wallet: 999f));
            _save.LoadGame(0)!.Player.WalletVtek.Should().BeApproximately(999f, 0.01f);
        }

        // ── Corrupt / invalid JSON ─────────────────────────────────────────────────

        [Fact]
        public void LoadGame_CorruptJson_ReturnsNull()
        {
            // Write valid file then corrupt it
            _save.SaveGame(0, MakeSaveData());
            string path = Path.Combine(_saveDir, "save_slot_0.json");
            File.WriteAllText(path, "{ this is not valid json !!! }");

            _save.LoadGame(0).Should().BeNull();
        }

        [Fact]
        public void LoadGame_EmptyFile_ReturnsNull()
        {
            _save.SaveGame(0, MakeSaveData());
            string path = Path.Combine(_saveDir, "save_slot_0.json");
            File.WriteAllText(path, string.Empty);

            _save.LoadGame(0).Should().BeNull();
        }

        // ── Delete ────────────────────────────────────────────────────────────────

        [Fact]
        public void DeleteSlot_RemovesFile()
        {
            _save.SaveGame(0, MakeSaveData());
            _save.DeleteSlot(0);
            _save.SlotExists(0).Should().BeFalse();
        }

        [Fact]
        public void DeleteSlot_NonExistentSlot_DoesNotThrow()
        {
            Action act = () => _save.DeleteSlot(9);
            act.Should().NotThrow();
        }

        [Fact]
        public void DeleteSlot_DoesNotAffectOtherSlots()
        {
            _save.SaveGame(0, MakeSaveData());
            _save.SaveGame(1, MakeSaveData());
            _save.DeleteSlot(0);

            _save.SlotExists(0).Should().BeFalse();
            _save.SlotExists(1).Should().BeTrue();
        }

        // ── SlotExists ────────────────────────────────────────────────────────────

        [Fact]
        public void SlotExists_BeforeSave_ReturnsFalse()
            => _save.SlotExists(5).Should().BeFalse();

        [Fact]
        public void SlotExists_AfterSave_ReturnsTrue()
        {
            _save.SaveGame(2, MakeSaveData());
            _save.SlotExists(2).Should().BeTrue();
        }

        // ── No .tmp files left after successful save ───────────────────────────────

        [Fact]
        public void SaveGame_LeavesNoTempFile()
        {
            _save.SaveGame(0, MakeSaveData());
            string tmpPath = Path.Combine(_saveDir, "save_slot_0.json.tmp");
            File.Exists(tmpPath).Should().BeFalse("atomic rename should remove .tmp file");
        }
    }
}
