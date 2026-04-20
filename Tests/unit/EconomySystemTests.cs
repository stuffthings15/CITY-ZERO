using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using CityZero.Core;
using CityZero.Core.Events;
using CityZero.Systems;

namespace CityZero.Tests
{
    public class EconomySystemTests : IDisposable
    {
        private readonly EconomySystem _economy;
        private readonly List<EconomyTransactionEvent> _events = new();

        public EconomySystemTests()
        {
            ServiceLocator.Clear();
            var bus = new GameBus();
            bus._Ready();

            _economy = new EconomySystem();
            _economy._Ready();

            GameBus.Instance.Subscribe<EconomyTransactionEvent>(e => _events.Add(e));
        }

        public void Dispose() => ServiceLocator.Clear();

        // ── Starting state ───────────────────────────────────────────────────────

        [Fact]
        public void InitialWallet_Is500()
            => _economy.Wallet.Should().BeApproximately(500f, 0.001f);

        // ── AddCash ──────────────────────────────────────────────────────────────

        [Fact]
        public void AddCash_IncreasesWallet()
        {
            _economy.AddCash(1000f, "mission_reward");
            _economy.Wallet.Should().BeApproximately(1500f, 0.001f);
        }

        [Fact]
        public void AddCash_EmitsEvent()
        {
            _economy.AddCash(250f, "race_prize");
            _events.Should().HaveCount(1);
            _events[0].Amount.Should().BeApproximately(250f, 0.001f);
            _events[0].NewBalance.Should().BeApproximately(750f, 0.001f);
            _events[0].Reason.Should().Be("race_prize");
        }

        [Fact]
        public void AddCash_ZeroOrNegative_DoesNothing()
        {
            _economy.AddCash(0f);
            _economy.AddCash(-100f);
            _economy.Wallet.Should().BeApproximately(500f, 0.001f);
            _events.Should().BeEmpty();
        }

        // ── TrySpend ─────────────────────────────────────────────────────────────

        [Fact]
        public void TrySpend_WithSufficientFunds_DeductsAndReturnsTrue()
        {
            bool result = _economy.TrySpend(200f, "weapon_purchase");
            result.Should().BeTrue();
            _economy.Wallet.Should().BeApproximately(300f, 0.001f);
        }

        [Fact]
        public void TrySpend_WithInsufficientFunds_ReturnsFalse_DoesNotDeduct()
        {
            bool result = _economy.TrySpend(1000f, "too_expensive");
            result.Should().BeFalse();
            _economy.Wallet.Should().BeApproximately(500f, 0.001f);
            _events.Should().BeEmpty();
        }

        [Fact]
        public void TrySpend_ExactBalance_Succeeds()
        {
            bool result = _economy.TrySpend(500f, "exact");
            result.Should().BeTrue();
            _economy.Wallet.Should().BeApproximately(0f, 0.001f);
        }

        [Fact]
        public void TrySpend_ZeroAmount_AlwaysSucceeds_NoChange()
        {
            bool result = _economy.TrySpend(0f, "free");
            result.Should().BeTrue();
            _economy.Wallet.Should().BeApproximately(500f, 0.001f);
        }

        // ── Global price modifier ─────────────────────────────────────────────────

        [Fact]
        public void SetGlobalPriceModifier_AffectsSpend()
        {
            _economy.SetGlobalPriceModifier(1.5f);   // 50% price increase
            _economy.TrySpend(100f, "inflated");      // Should actually cost 150
            _economy.Wallet.Should().BeApproximately(350f, 0.001f);
        }

        [Fact]
        public void SetGlobalPriceModifier_BelowFloor_ClampsToMin()
        {
            _economy.SetGlobalPriceModifier(0f);
            // After clamp, modifier is 0.1 — spending 100 costs 10, leaving 490
            _economy.TrySpend(100f, "nearly_free");
            _economy.Wallet.Should().BeApproximately(490f, 0.001f);
        }

        // ── GetFinalPrice ────────────────────────────────────────────────────────

        [Fact]
        public void GetFinalPrice_WithNoModifiers_ReturnsBasePrice()
            => _economy.GetFinalPrice(1000f).Should().BeApproximately(1000f, 0.001f);

        [Fact]
        public void GetFinalPrice_WithFactionDiscount_AppliesMultiplier()
            => _economy.GetFinalPrice(1000f, factionMultiplier: 0.85f)
                       .Should().BeApproximately(850f, 0.001f);

        [Fact]
        public void GetFinalPrice_WithBothModifiers_Stacks()
        {
            _economy.SetGlobalPriceModifier(1.15f);
            _economy.GetFinalPrice(1000f, 0.85f).Should().BeApproximately(977.5f, 0.5f);
        }

        // ── RestoreWallet ────────────────────────────────────────────────────────

        [Fact]
        public void RestoreWallet_SetsExactValue()
        {
            _economy.RestoreWallet(9999.99f);
            _economy.Wallet.Should().BeApproximately(9999.99f, 0.01f);
        }

        [Fact]
        public void RestoreWallet_NegativeValue_ClampsToZero()
        {
            _economy.RestoreWallet(-500f);
            _economy.Wallet.Should().BeApproximately(0f, 0.001f);
        }
    }
}
