using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using CityZero.Core;
using CityZero.Core.Events;
using CityZero.Systems;

namespace CityZero.Tests
{
    public class FactionSystemTests : IDisposable
    {
        private readonly FactionSystem _factions;
        private readonly List<FactionRepChangedEvent> _events = new();

        public FactionSystemTests()
        {
            ServiceLocator.Clear();
            var bus = new GameBus();
            bus._Ready();

            _factions = new FactionSystem();
            _factions._Ready();

            GameBus.Instance.Subscribe<FactionRepChangedEvent>(e => _events.Add(e));
        }

        public void Dispose() => ServiceLocator.Clear();

        // ── Starting state ───────────────────────────────────────────────────────

        [Fact]
        public void InitialRep_IsZeroForAllFactions()
        {
            _factions.GetRep("ruin_syndicate").Should().BeApproximately(0f, 0.001f);
            _factions.GetRep("warden_bloc").Should().BeApproximately(0f, 0.001f);
            _factions.GetRep("hollow_kings").Should().BeApproximately(0f, 0.001f);
        }

        [Fact]
        public void InitialTier_IsNeutral()
            => _factions.GetTier("ruin_syndicate").Should().Be(FactionTier.Neutral);

        // ── ModifyRep ─────────────────────────────────────────────────────────────

        [Fact]
        public void ModifyRep_IncreasesCorrectly()
        {
            _factions.ModifyRep("ruin_syndicate", 30f, "mission_complete");
            _factions.GetRep("ruin_syndicate").Should().BeApproximately(30f, 0.001f);
        }

        [Fact]
        public void ModifyRep_ClampsAtMax()
        {
            _factions.ModifyRep("ruin_syndicate", 150f, "overdrive");
            _factions.GetRep("ruin_syndicate").Should().BeApproximately(100f, 0.001f);
        }

        [Fact]
        public void ModifyRep_ClampsAtMin()
        {
            _factions.ModifyRep("ruin_syndicate", -150f, "mass_murder");
            _factions.GetRep("ruin_syndicate").Should().BeApproximately(-100f, 0.001f);
        }

        [Fact]
        public void ModifyRep_EmitsEvent()
        {
            _factions.ModifyRep("ruin_syndicate", 15f, "heist");
            _events.Should().Contain(e => e.FactionId == "ruin_syndicate" && e.NewRep == 15f);
        }

        [Fact]
        public void ModifyRep_UnknownFaction_DoesNotThrow()
        {
            Action act = () => _factions.ModifyRep("nonexistent_faction", 10f, "test");
            act.Should().NotThrow();
        }

        // ── Cross-faction bleed ───────────────────────────────────────────────────

        [Fact]
        public void ModifyRep_WithSyndicate_BleadsNegativeToWarden()
        {
            _factions.ModifyRep("ruin_syndicate", 50f, "heist");
            // Bleed: ruin_syndicate → warden_bloc at -0.4x
            _factions.GetRep("warden_bloc").Should().BeApproximately(-20f, 0.1f);
        }

        [Fact]
        public void ModifyRep_WithWarden_BleadsNegativeToSyndicate()
        {
            _factions.ModifyRep("warden_bloc", 40f, "escort_job");
            _factions.GetRep("ruin_syndicate").Should().BeApproximately(-16f, 0.1f);
        }

        // ── Tiers ────────────────────────────────────────────────────────────────

        [Theory]
        [InlineData(-100f, FactionTier.Enemy)]
        [InlineData(-61f,  FactionTier.Enemy)]
        [InlineData(-60f,  FactionTier.Hostile)]
        [InlineData(-21f,  FactionTier.Hostile)]
        [InlineData(-20f,  FactionTier.Neutral)]
        [InlineData(0f,    FactionTier.Neutral)]
        [InlineData(20f,   FactionTier.Neutral)]
        [InlineData(21f,   FactionTier.Allied)]
        [InlineData(60f,   FactionTier.Allied)]
        [InlineData(61f,   FactionTier.Trusted)]
        [InlineData(100f,  FactionTier.Trusted)]
        public void GetTier_ReturnsCorrectTier(float rep, FactionTier expected)
        {
            _factions.ModifyRep("ruin_syndicate", rep, "test");
            _factions.GetTier("ruin_syndicate").Should().Be(expected);
        }

        // ── Price multipliers ─────────────────────────────────────────────────────

        [Fact]
        public void GetPriceMultiplier_Trusted_Is070()
        {
            _factions.ModifyRep("ruin_syndicate", 100f, "grind");
            _factions.GetPriceMultiplier("ruin_syndicate").Should().BeApproximately(0.70f, 0.001f);
        }

        [Fact]
        public void GetPriceMultiplier_Allied_Is085()
        {
            _factions.ModifyRep("ruin_syndicate", 40f, "jobs");
            _factions.GetPriceMultiplier("ruin_syndicate").Should().BeApproximately(0.85f, 0.001f);
        }

        [Fact]
        public void GetPriceMultiplier_Neutral_Is100()
            => _factions.GetPriceMultiplier("ruin_syndicate").Should().BeApproximately(1.00f, 0.001f);

        [Fact]
        public void GetPriceMultiplier_Hostile_Is130()
        {
            _factions.ModifyRep("ruin_syndicate", -40f, "betrayal");
            _factions.GetPriceMultiplier("ruin_syndicate").Should().BeApproximately(1.30f, 0.001f);
        }

        // ── RestoreReputations ───────────────────────────────────────────────────

        [Fact]
        public void RestoreReputations_LoadsCorrectValues()
        {
            var saved = new Dictionary<string, float>
            {
                ["ruin_syndicate"] = 55f,
                ["warden_bloc"]    = -30f,
                ["hollow_kings"]   = 10f
            };
            _factions.RestoreReputations(saved);

            _factions.GetRep("ruin_syndicate").Should().BeApproximately(55f, 0.001f);
            _factions.GetRep("warden_bloc").Should().BeApproximately(-30f, 0.001f);
            _factions.GetRep("hollow_kings").Should().BeApproximately(10f, 0.001f);
        }

        [Fact]
        public void RestoreReputations_ClampsOutOfRangeValues()
        {
            _factions.RestoreReputations(new Dictionary<string, float>
            {
                ["ruin_syndicate"] = 999f
            });
            _factions.GetRep("ruin_syndicate").Should().BeApproximately(100f, 0.001f);
        }
    }
}
