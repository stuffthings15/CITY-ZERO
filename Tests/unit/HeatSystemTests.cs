using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using CityZero.Core;
using CityZero.Core.Events;
using CityZero.Systems;

namespace CityZero.Tests
{
    /// <summary>
    /// Unit tests for HeatSystem.
    /// Each test uses a fresh GameBus + HeatSystem to avoid shared state.
    /// </summary>
    public class HeatSystemTests : IDisposable
    {
        private readonly HeatSystem _heat;
        private readonly List<HeatChangedEvent> _events = new();

        public HeatSystemTests()
        {
            // Fresh GameBus per test
            ServiceLocator.Clear();
            var bus = new GameBus();
            bus._Ready();

            _heat = new HeatSystem();
            _heat._Ready();

            GameBus.Instance.Subscribe<HeatChangedEvent>(e => _events.Add(e));
        }

        public void Dispose() => ServiceLocator.Clear();

        // ── AddHeat ──────────────────────────────────────────────────────────────

        [Fact]
        public void AddHeat_IncreasesCurrentHeat()
        {
            _heat.AddHeat(1.5f, "test");
            _heat.CurrentHeat.Should().BeApproximately(1.5f, 0.001f);
        }

        [Fact]
        public void AddHeat_DoesNotExceedMaximum()
        {
            _heat.AddHeat(10f, "overdrive");
            _heat.CurrentHeat.Should().BeApproximately(5.0f, 0.001f);
        }

        [Fact]
        public void AddHeat_ZeroOrNegative_DoesNothing()
        {
            _heat.AddHeat(0f, "zero");
            _heat.AddHeat(-1f, "negative");
            _heat.CurrentHeat.Should().BeApproximately(0f, 0.001f);
        }

        [Fact]
        public void AddHeat_EmitsHeatChangedEvent()
        {
            _heat.AddHeat(2f, "shooting");
            _events.Should().HaveCount(1);
            _events[0].NewHeat.Should().BeApproximately(2f, 0.001f);
            _events[0].Reason.Should().Be("shooting");
        }

        // ── HeatLevel ────────────────────────────────────────────────────────────

        [Theory]
        [InlineData(0.0f, 0)]
        [InlineData(0.9f, 0)]
        [InlineData(1.0f, 1)]
        [InlineData(1.99f, 1)]
        [InlineData(2.0f, 2)]
        [InlineData(3.5f, 3)]
        [InlineData(4.1f, 4)]
        [InlineData(5.0f, 5)]
        public void HeatLevel_ReturnsCorrectTier(float heat, int expectedLevel)
        {
            _heat.AddHeat(heat, "test");
            _heat.HeatLevel.Should().Be(expectedLevel);
        }

        // ── ReduceHeat ───────────────────────────────────────────────────────────

        [Fact]
        public void ReduceHeat_DecreasesCurrentHeat()
        {
            _heat.AddHeat(3f, "setup");
            _heat.ReduceHeat(1f, "bribe");
            _heat.CurrentHeat.Should().BeApproximately(2f, 0.001f);
        }

        [Fact]
        public void ReduceHeat_DoesNotGoBelowZero()
        {
            _heat.AddHeat(0.5f, "setup");
            _heat.ReduceHeat(5f, "overdrop");
            _heat.CurrentHeat.Should().BeApproximately(0f, 0.001f);
        }

        // ── ClearHeat ────────────────────────────────────────────────────────────

        [Fact]
        public void ClearHeat_ResetsToZero()
        {
            _heat.AddHeat(4f, "setup");
            _heat.ClearHeat("safehouse");
            _heat.CurrentHeat.Should().BeApproximately(0f, 0.001f);
        }

        [Fact]
        public void ClearHeat_EmitsEventWithZero()
        {
            _heat.AddHeat(3f, "setup");
            _events.Clear();
            _heat.ClearHeat("safehouse");
            _events.Should().HaveCount(1);
            _events[0].NewHeat.Should().BeApproximately(0f, 0.001f);
        }

        // ── IsWanted ─────────────────────────────────────────────────────────────

        [Fact]
        public void IsWanted_FalseAtZero()
            => _heat.IsWanted.Should().BeFalse();

        [Fact]
        public void IsWanted_TrueAboveOne()
        {
            _heat.AddHeat(1f, "crime");
            _heat.IsWanted.Should().BeTrue();
        }

        // ── Safehouse ────────────────────────────────────────────────────────────

        [Fact]
        public void SetInSafehouse_ImmediatelyClearsHeatOnNextProcess()
        {
            _heat.AddHeat(5f, "setup");
            _heat.SetInSafehouse(true);
            _heat._Process(0.016); // one frame
            _heat.CurrentHeat.Should().BeApproximately(0f, 0.001f);
        }

        // ── Hidden / Decay ───────────────────────────────────────────────────────

        [Fact]
        public void SetPlayerHidden_False_ResetsTimer()
        {
            _heat.AddHeat(2f, "setup");
            _heat.SetPlayerHidden(true);
            _heat.SetPlayerHidden(false);
            // Process enough time to pass cooldown — should NOT decay because hidden=false
            _heat._Process(100.0);
            _heat.CurrentHeat.Should().BeApproximately(2f, 0.001f);
        }

        // ── No duplicate events ──────────────────────────────────────────────────

        [Fact]
        public void AddHeat_SameValue_DoesNotEmitDuplicateEvent()
        {
            _heat.AddHeat(1f, "first");
            int countAfterFirst = _events.Count;
            // Adding 0 should not emit
            _heat.AddHeat(0f, "zero");
            _events.Should().HaveCount(countAfterFirst);
        }
    }
}
