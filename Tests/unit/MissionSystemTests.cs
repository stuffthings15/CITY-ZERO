using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using CityZero.Core;
using CityZero.Core.Events;
using CityZero.Systems;

namespace CityZero.Tests
{
    public class MissionSystemTests : IDisposable
    {
        private readonly MissionSystem _missions;
        private readonly List<MissionStateChangedEvent> _stateEvents  = new();
        private readonly List<ObjectiveCompletedEvent>  _objEvents     = new();

        public MissionSystemTests()
        {
            ServiceLocator.Clear();
            var bus = new GameBus();
            bus._Ready();

            _missions = new MissionSystem();
            _missions._Ready();

            GameBus.Instance.Subscribe<MissionStateChangedEvent>(e => _stateEvents.Add(e));
            GameBus.Instance.Subscribe<ObjectiveCompletedEvent>(e  => _objEvents.Add(e));
        }

        public void Dispose() => ServiceLocator.Clear();

        // ── ActivateMission ───────────────────────────────────────────────────────

        [Fact]
        public void ActivateMission_NewMission_ReturnsTrue()
            => _missions.ActivateMission("mission_a").Should().BeTrue();

        [Fact]
        public void ActivateMission_NewMission_IsActive()
        {
            _missions.ActivateMission("mission_a");
            _missions.IsMissionActive("mission_a").Should().BeTrue();
        }

        [Fact]
        public void ActivateMission_EmitsActiveStateEvent()
        {
            _missions.ActivateMission("mission_a");
            _stateEvents.Should().ContainSingle(e =>
                e.MissionId == "mission_a" &&
                e.NewState  == MissionState.Active);
        }

        [Fact]
        public void ActivateMission_Duplicate_ReturnsFalse()
        {
            _missions.ActivateMission("mission_a");
            _missions.ActivateMission("mission_a").Should().BeFalse();
        }

        [Fact]
        public void ActivateMission_Duplicate_DoesNotDoubleCount()
        {
            _missions.ActivateMission("mission_a");
            _missions.ActivateMission("mission_a");
            _missions.ActiveCount.Should().Be(1);
        }

        [Fact]
        public void ActivateMission_AtCap_ReturnsFalse()
        {
            _missions.ActivateMission("m1");
            _missions.ActivateMission("m2");
            _missions.ActivateMission("m3");
            _missions.ActivateMission("m4").Should().BeFalse();
        }

        [Fact]
        public void ActivateMission_AtCap_DoesNotExceedThree()
        {
            _missions.ActivateMission("m1");
            _missions.ActivateMission("m2");
            _missions.ActivateMission("m3");
            _missions.ActivateMission("m4");
            _missions.ActiveCount.Should().Be(3);
        }

        [Fact]
        public void ActivateMission_AfterComplete_CanReactivate()
        {
            _missions.ActivateMission("mission_a");
            // Force complete via objective
            _missions.TryGetActive("mission_a", out var inst);
            inst.AddObjective("obj_1");
            inst.CompleteObjective("obj_1");

            _missions.IsMissionCompleted("mission_a").Should().BeTrue();
            // A completed mission can be re-used (replay / alternative path)
            _missions.ActivateMission("mission_a").Should().BeTrue();
        }

        // ── Objective completion → mission completion ──────────────────────────

        [Fact]
        public void CompleteAllRequired_CompletesTheMission()
        {
            _missions.ActivateMission("mission_a");
            _missions.TryGetActive("mission_a", out var inst);
            inst.AddObjective("reach_marker");
            inst.AddObjective("eliminate_target");

            inst.CompleteObjective("reach_marker");
            _missions.IsMissionCompleted("mission_a").Should().BeFalse("one objective remains");

            inst.CompleteObjective("eliminate_target");
            _missions.IsMissionCompleted("mission_a").Should().BeTrue();
        }

        [Fact]
        public void MissionCompletion_EmitsCompletedStateEvent()
        {
            _missions.ActivateMission("mission_a");
            _missions.TryGetActive("mission_a", out var inst);
            inst.AddObjective("obj");
            inst.CompleteObjective("obj");

            _stateEvents.Should().Contain(e =>
                e.MissionId == "mission_a" &&
                e.NewState  == MissionState.Completed);
        }

        [Fact]
        public void MissionCompletion_RemovedFromActive()
        {
            _missions.ActivateMission("mission_a");
            _missions.TryGetActive("mission_a", out var inst);
            inst.AddObjective("obj");
            inst.CompleteObjective("obj");

            _missions.IsMissionActive("mission_a").Should().BeFalse();
        }

        [Fact]
        public void MissionCompletion_FreesSlotForNewMission()
        {
            _missions.ActivateMission("m1"); _missions.ActivateMission("m2"); _missions.ActivateMission("m3");

            _missions.TryGetActive("m1", out var inst);
            inst.AddObjective("done");
            inst.CompleteObjective("done");

            // Slot freed — m4 should now activate
            _missions.ActivateMission("m4").Should().BeTrue();
        }

        // ── Optional objectives ───────────────────────────────────────────────────

        [Fact]
        public void OptionalObjective_DoesNotBlockCompletion()
        {
            _missions.ActivateMission("mission_a");
            _missions.TryGetActive("mission_a", out var inst);
            inst.AddObjective("required_obj",  isOptional: false);
            inst.AddObjective("optional_bonus", isOptional: true);

            inst.CompleteObjective("required_obj");
            // Should complete even though optional wasn't done
            _missions.IsMissionCompleted("mission_a").Should().BeTrue();
        }

        [Fact]
        public void OptionalObjective_EmitsEventWithOptionalFlag()
        {
            _missions.ActivateMission("mission_a");
            _missions.TryGetActive("mission_a", out var inst);
            inst.AddObjective("bonus", isOptional: true);
            inst.AddObjective("main");
            inst.CompleteObjective("bonus");

            _objEvents.Should().Contain(e =>
                e.ObjectiveId == "bonus" && e.IsOptional);
        }

        [Fact]
        public void RequiredObjective_EmitsEventWithNotOptionalFlag()
        {
            _missions.ActivateMission("mission_a");
            _missions.TryGetActive("mission_a", out var inst);
            inst.AddObjective("main");
            inst.CompleteObjective("main");

            _objEvents.Should().Contain(e =>
                e.ObjectiveId == "main" && !e.IsOptional);
        }

        // ── Duplicate objective completion ────────────────────────────────────────

        [Fact]
        public void CompleteObjective_Twice_OnlyFiresOneEvent()
        {
            _missions.ActivateMission("mission_a");
            _missions.TryGetActive("mission_a", out var inst);
            inst.AddObjective("obj");
            inst.CompleteObjective("obj");
            inst.CompleteObjective("obj");   // idempotent

            _objEvents.Count(e => e.ObjectiveId == "obj").Should().Be(1);
        }

        // ── PlayerDied cascade ────────────────────────────────────────────────────

        [Fact]
        public void PlayerDied_FailsAllActiveMissions()
        {
            _missions.ActivateMission("m1");
            _missions.ActivateMission("m2");

            GameBus.Instance.Emit(new PlayerDiedEvent(100f));

            _missions.IsMissionFailed("m1").Should().BeTrue();
            _missions.IsMissionFailed("m2").Should().BeTrue();
        }

        [Fact]
        public void PlayerDied_ClearsActiveList()
        {
            _missions.ActivateMission("m1");
            _missions.ActivateMission("m2");
            GameBus.Instance.Emit(new PlayerDiedEvent(100f));

            _missions.ActiveCount.Should().Be(0);
        }

        [Fact]
        public void PlayerDied_EmitsFailedStateEventsForEachMission()
        {
            _missions.ActivateMission("m1");
            _missions.ActivateMission("m2");
            GameBus.Instance.Emit(new PlayerDiedEvent(100f));

            _stateEvents.Should().Contain(e => e.MissionId == "m1" && e.NewState == MissionState.Failed);
            _stateEvents.Should().Contain(e => e.MissionId == "m2" && e.NewState == MissionState.Failed);
        }

        [Fact]
        public void PlayerDied_WithNoActiveMissions_DoesNotThrow()
        {
            Action act = () => GameBus.Instance.Emit(new PlayerDiedEvent(100f));
            act.Should().NotThrow();
        }

        // ── ReportEvent ───────────────────────────────────────────────────────────

        [Fact]
        public void ReportEvent_CompletesMatchingObjective()
        {
            _missions.ActivateMission("mission_a");
            _missions.TryGetActive("mission_a", out var inst);
            inst.AddObjective("kill_target");

            _missions.ReportEvent("kill_target", "npc_01");

            _objEvents.Should().Contain(e => e.ObjectiveId == "kill_target");
        }

        // ── Query helpers ─────────────────────────────────────────────────────────

        [Fact]
        public void IsCompleted_NotStarted_ReturnsFalse()
            => _missions.IsMissionCompleted("unknown").Should().BeFalse();

        [Fact]
        public void IsFailed_NotStarted_ReturnsFalse()
            => _missions.IsMissionFailed("unknown").Should().BeFalse();

        [Fact]
        public void IsActive_NotStarted_ReturnsFalse()
            => _missions.IsMissionActive("unknown").Should().BeFalse();

        [Fact]
        public void TryGetActive_NonExistent_ReturnsFalse()
            => _missions.TryGetActive("ghost", out _).Should().BeFalse();
    }
}
