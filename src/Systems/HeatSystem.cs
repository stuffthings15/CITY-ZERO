using Godot;
using System;
using System.Collections.Generic;
using CityZero.Core;
using CityZero.Core.Events;

namespace CityZero.Systems
{
    /// <summary>
    /// Manages the player's wanted level (0.0–5.0).
    /// Heat is a float for smooth UI transitions; HeatLevel is the integer tier for dispatch logic.
    /// Communicates exclusively via GameBus — no direct system references.
    /// </summary>
    public partial class HeatSystem : Node
    {
        // ── Public State ─────────────────────────────────────────────────────────
        public float CurrentHeat { get; private set; } = 0f;
        public int HeatLevel => Mathf.FloorToInt(CurrentHeat);
        public bool IsWanted => CurrentHeat >= 1f;

        // ── Config (loaded from data/config/heat_config.json via DataRegistry) ──
        [Export] private float _decayRate = 0.5f;       // HP per second when cooling
        [Export] private float[] _cooldownDelays = { 0f, 15f, 30f, 45f, 60f, 90f };

        // ── Internal State ────────────────────────────────────────────────────────
        private float _cooldownTimer = 0f;
        private bool _playerHidden = false;
        private bool _inSafehouse = false;

        private const float MAX_HEAT = 5f;
        private const float MIN_HEAT = 0f;

        // ── Godot Lifecycle ───────────────────────────────────────────────────────
        public override void _Ready()
        {
            ServiceLocator.Register<HeatSystem>(this);
            GD.Print("[HeatSystem] Ready.");
        }

        public override void _Process(double delta)
        {
            if (_inSafehouse)
            {
                // Immediate decay inside safehouse
                SetHeat(0f, "safehouse");
                return;
            }

            if (_playerHidden && CurrentHeat > MIN_HEAT)
            {
                int level = HeatLevel;
                float delay = level < _cooldownDelays.Length ? _cooldownDelays[level] : 90f;

                _cooldownTimer += (float)delta;
                if (_cooldownTimer >= delay)
                    SetHeat(Mathf.Max(MIN_HEAT, CurrentHeat - _decayRate * (float)delta), "decay");
            }
            else
            {
                _cooldownTimer = 0f;
            }
        }

        // ── Public API ────────────────────────────────────────────────────────────

        /// <summary>Increases heat by the given amount. Pass reason for analytics/debug.</summary>
        public void AddHeat(float amount, string reason = "unspecified")
        {
            if (amount <= 0f) return;
            SetHeat(Mathf.Min(MAX_HEAT, CurrentHeat + amount), reason);
        }

        /// <summary>Reduces heat by amount. Cannot go below 0.</summary>
        public void ReduceHeat(float amount, string reason = "unspecified")
        {
            if (amount <= 0f) return;
            SetHeat(Mathf.Max(MIN_HEAT, CurrentHeat - amount), reason);
        }

        /// <summary>Fully resets heat to 0 (safehouse, bribe, disguise).</summary>
        public void ClearHeat(string reason = "cleared")
            => SetHeat(MIN_HEAT, reason);

        /// <summary>Called by player visibility system when player breaks/regains LOS.</summary>
        public void SetPlayerHidden(bool hidden)
        {
            if (_playerHidden == hidden) return;
            _playerHidden = hidden;
            if (!hidden) _cooldownTimer = 0f;
        }

        /// <summary>Called when player enters or exits a registered safehouse.</summary>
        public void SetInSafehouse(bool inSafehouse)
            => _inSafehouse = inSafehouse;

        // ── Private ───────────────────────────────────────────────────────────────

        private void SetHeat(float newHeat, string reason)
        {
            newHeat = Mathf.Clamp(newHeat, MIN_HEAT, MAX_HEAT);
            if (Mathf.IsEqualApprox(newHeat, CurrentHeat)) return;

            float oldHeat = CurrentHeat;
            CurrentHeat = newHeat;

            GameBus.Instance.Emit(new HeatChangedEvent(CurrentHeat, HeatLevel, reason));
            GD.Print($"[HeatSystem] Heat: {oldHeat:F2} → {CurrentHeat:F2} (level {HeatLevel}) | {reason}");
        }
    }
}
