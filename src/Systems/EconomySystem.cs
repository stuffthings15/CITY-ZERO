using Godot;
using System;
using System.Collections.Generic;
using CityZero.Core;
using CityZero.Core.Events;

namespace CityZero.Systems
{
    /// <summary>
    /// Manages the player's wallet and all price/transaction logic.
    /// All faction discount lookups delegated to FactionSystem.
    /// Communicates balance changes via GameBus.
    /// </summary>
    public partial class EconomySystem : Node
    {
        public float Wallet { get; private set; } = 500f;   // Starting cash

        // Price modifier applied globally (e.g., from Cartel territory events)
        private float _globalPriceModifier = 1.0f;

        private readonly List<TransactionRecord> _transactionLog = new(capacity: 50);

        public override void _Ready()
        {
            ServiceLocator.Register<EconomySystem>(this);
            GD.Print("[EconomySystem] Ready. Starting wallet: " + Wallet);
        }

        // ── Public API ────────────────────────────────────────────────────────────

        /// <summary>
        /// Attempts to spend cash. Returns true and deducts if funds available.
        /// </summary>
        public bool TrySpend(float amount, string reason = "purchase")
        {
            if (amount <= 0f) return true;
            float finalAmount = amount * _globalPriceModifier;
            if (Wallet < finalAmount)
            {
                GD.Print($"[EconomySystem] Insufficient funds. Need {finalAmount:F2}, have {Wallet:F2}");
                return false;
            }

            Wallet -= finalAmount;
            LogAndEmit(-finalAmount, reason);
            return true;
        }

        /// <summary>Adds cash to the wallet (mission reward, loot, race prize, etc.).</summary>
        public void AddCash(float amount, string reason = "reward")
        {
            if (amount <= 0f) return;
            Wallet += amount;
            LogAndEmit(amount, reason);
        }

        /// <summary>
        /// Returns the final price for an item after faction discount and global modifier.
        /// factionMultiplier: pass 1.0 if no faction discount applies.
        /// </summary>
        public float GetFinalPrice(float basePrice, float factionMultiplier = 1.0f)
            => basePrice * factionMultiplier * _globalPriceModifier;

        /// <summary>Sets the city-wide price modifier (e.g., from Cartel territory events).</summary>
        public void SetGlobalPriceModifier(float modifier)
        {
            _globalPriceModifier = Mathf.Max(0.1f, modifier);
            GD.Print($"[EconomySystem] Global price modifier updated: {_globalPriceModifier:F2}x");
        }

        /// <summary>Directly sets wallet value (use only for save/load restoration).</summary>
        public void RestoreWallet(float amount)
        {
            Wallet = Mathf.Max(0f, amount);
            GD.Print($"[EconomySystem] Wallet restored to {Wallet:F2}");
        }

        // ── Private ───────────────────────────────────────────────────────────────

        private void LogAndEmit(float amount, string reason)
        {
            if (_transactionLog.Count >= 50)
                _transactionLog.RemoveAt(0);

            _transactionLog.Add(new TransactionRecord(amount, Wallet, reason, Time.GetUnixTimeFromSystem()));
            GameBus.Instance.Emit(new EconomyTransactionEvent(amount, Wallet, reason));
            GD.Print($"[EconomySystem] {(amount >= 0 ? "+" : "")}{amount:F2} | Balance: {Wallet:F2} | {reason}");
        }
    }

    public readonly struct TransactionRecord
    {
        public float Amount { get; }
        public float BalanceAfter { get; }
        public string Reason { get; }
        public double Timestamp { get; }

        public TransactionRecord(float amount, float balance, string reason, double timestamp)
        {
            Amount = amount;
            BalanceAfter = balance;
            Reason = reason;
            Timestamp = timestamp;
        }
    }
}
