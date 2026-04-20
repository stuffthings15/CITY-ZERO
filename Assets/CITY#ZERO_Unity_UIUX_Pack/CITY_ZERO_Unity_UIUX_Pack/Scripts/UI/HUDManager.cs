using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CityZero.UI
{
    /// <summary>
    /// Manages updates to the Heads‑Up Display. Attach this component to
    /// your HUD canvas and assign the required references in the inspector.
    /// The HUDManager exposes methods to update health, armor, ammo, heat,
    /// cash, and mission objective text. Call these methods whenever the
    /// underlying game values change.
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        [Header("Bars")]
        [Tooltip("Image with Filled type representing the player's health.")]
        public Image healthBar;

        [Tooltip("Image with Filled type representing the player's armor.")]
        public Image armorBar;

        [Tooltip("Image with Filled type representing the player's heat or wanted level.")]
        public Image heatMeter;

        [Header("Ammo")]
        [Tooltip("Text component showing ammo in the current clip.")]
        public TMP_Text ammoCurrentText;

        [Tooltip("Text component showing reserve ammo.")]
        public TMP_Text ammoReserveText;

        [Header("Cash / Objective")]
        [Tooltip("Text component displaying the player's cash or influence.")]
        public TMP_Text cashText;

        [Tooltip("Text component for the current mission objective.")]
        public TMP_Text objectiveText;

        private void Awake()
        {
            // Apply default colors if assigned. These can be overridden in the inspector.
            if (healthBar != null)
                healthBar.color = UIConstants.HealthColor;
            if (armorBar != null)
                armorBar.color = UIConstants.ArmorColor;
            if (heatMeter != null)
                heatMeter.color = UIConstants.HeatColor;
        }

        #region Update Methods

        /// <summary>
        /// Updates the health bar fill value (0–1).
        /// </summary>
        public void UpdateHealth(float normalizedValue)
        {
            if (healthBar != null)
            {
                healthBar.fillAmount = Mathf.Clamp01(normalizedValue);
            }
        }

        /// <summary>
        /// Updates the armor bar fill value (0–1).
        /// </summary>
        public void UpdateArmor(float normalizedValue)
        {
            if (armorBar != null)
            {
                armorBar.fillAmount = Mathf.Clamp01(normalizedValue);
            }
        }

        /// <summary>
        /// Updates the heat meter fill value (0–1).
        /// </summary>
        public void UpdateHeat(float normalizedValue)
        {
            if (heatMeter != null)
            {
                heatMeter.fillAmount = Mathf.Clamp01(normalizedValue);
            }
        }

        /// <summary>
        /// Updates the ammo display. Provide current clip count and reserve ammo.
        /// </summary>
        public void UpdateAmmo(int currentClip, int reserve)
        {
            if (ammoCurrentText != null)
            {
                ammoCurrentText.text = currentClip.ToString();
            }
            if (ammoReserveText != null)
            {
                ammoReserveText.text = reserve.ToString();
            }
        }

        /// <summary>
        /// Updates the cash display. Accepts an integer or long for large values.
        /// </summary>
        public void UpdateCash(long cash)
        {
            if (cashText != null)
            {
                cashText.text = string.Format(UIConstants.CashFormat, cash);
            }
        }

        /// <summary>
        /// Updates the objective text. Pass null or empty string to clear.
        /// </summary>
        public void UpdateObjective(string objective)
        {
            if (objectiveText != null)
            {
                objectiveText.text = string.IsNullOrEmpty(objective) ? string.Empty : objective;
            }
        }

        #endregion
    }
}