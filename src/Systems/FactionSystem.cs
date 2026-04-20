using Godot;
using System;
using System.Collections.Generic;
using CityZero.Core;
using CityZero.Core.Events;

namespace CityZero.Systems
{
    /// <summary>
    /// Manages all faction reputations and cross-faction reputation bleed.
    /// Faction data is loaded from JSON definitions via DataRegistry.
    /// </summary>
    public partial class FactionSystem : Node
    {
        public const float MIN_REP = -100f;
        public const float MAX_REP = 100f;

        private readonly Dictionary<string, float> _reputations = new()
        {
            ["ruin_syndicate"]       = 0f,
            ["warden_bloc"]          = 0f,
            ["hollow_kings"]         = 0f,
            ["meridian_cartel"]      = 0f,
            ["axiom_threat"]         = 0f,   // Not a reputation; a threat meter
        };

        // Cross-faction bleed: when rep with A changes, rep with B changes by bleed multiplier
        // Loaded from faction JSON; hardcoded here as fallback
        private static readonly (string from, string to, float bleed)[] REP_BLEED = {
            ("ruin_syndicate", "warden_bloc",    -0.4f),
            ("warden_bloc",    "ruin_syndicate", -0.4f),
        };

        public override void _Ready()
        {
            ServiceLocator.Register<FactionSystem>(this);
            GD.Print("[FactionSystem] Ready.");
        }

        // ── Public API ────────────────────────────────────────────────────────────

        public float GetRep(string factionId)
            => _reputations.TryGetValue(factionId, out var rep) ? rep : 0f;

        public FactionTier GetTier(string factionId)
        {
            float rep = GetRep(factionId);
            return rep switch
            {
                <= -61f => FactionTier.Enemy,
                <= -21f => FactionTier.Hostile,
                <= 20f  => FactionTier.Neutral,
                <= 60f  => FactionTier.Allied,
                _       => FactionTier.Trusted,
            };
        }

        /// <summary>Modifies reputation for a faction, applying bleed to related factions.</summary>
        public void ModifyRep(string factionId, float delta, string reason = "unspecified")
        {
            if (!_reputations.ContainsKey(factionId))
            {
                GD.PrintErr($"[FactionSystem] Unknown faction: {factionId}");
                return;
            }

            float oldRep = _reputations[factionId];
            _reputations[factionId] = Mathf.Clamp(oldRep + delta, MIN_REP, MAX_REP);
            float newRep = _reputations[factionId];

            GameBus.Instance.Emit(new FactionRepChangedEvent(factionId, oldRep, newRep, reason));
            GD.Print($"[FactionSystem] {factionId}: {oldRep:F1} → {newRep:F1} ({(delta >= 0 ? "+" : "")}{delta:F1}) | {reason}");

            // Apply bleed
            foreach (var (from, to, bleedMult) in REP_BLEED)
            {
                if (from != factionId) continue;
                float bleedDelta = delta * bleedMult;
                if (Mathf.Abs(bleedDelta) < 0.01f) continue;
                float oldBleed = _reputations[to];
                _reputations[to] = Mathf.Clamp(oldBleed + bleedDelta, MIN_REP, MAX_REP);
                GameBus.Instance.Emit(new FactionRepChangedEvent(to, oldBleed, _reputations[to],
                    $"bleed_from_{factionId}"));
            }
        }

        /// <summary>Returns the price discount multiplier for a given faction based on current rep.</summary>
        public float GetPriceMultiplier(string factionId)
        {
            return GetTier(factionId) switch
            {
                FactionTier.Trusted  => 0.70f,
                FactionTier.Allied   => 0.85f,
                FactionTier.Hostile  => 1.30f,
                FactionTier.Enemy    => float.MaxValue, // No service
                _                   => 1.00f,
            };
        }

        /// <summary>Restores all faction reps from save data.</summary>
        public void RestoreReputations(Dictionary<string, float> saved)
        {
            foreach (var kv in saved)
                if (_reputations.ContainsKey(kv.Key))
                    _reputations[kv.Key] = Mathf.Clamp(kv.Value, MIN_REP, MAX_REP);
        }

        public Dictionary<string, float> GetAllReputations()
            => new(_reputations);
    }

    public enum FactionTier { Enemy, Hostile, Neutral, Allied, Trusted }
}
