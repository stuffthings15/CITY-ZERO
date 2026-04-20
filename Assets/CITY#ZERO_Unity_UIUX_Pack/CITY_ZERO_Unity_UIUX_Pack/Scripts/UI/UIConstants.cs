using UnityEngine;

namespace CityZero.UI
{
    /// <summary>
    /// A collection of static constants used throughout the UI system.
    /// Centralizing common strings and values makes it easy to change defaults
    /// without searching through multiple files. Feel free to expand this as
    /// your project grows.
    /// </summary>
    public static class UIConstants
    {
        // Reference resolution for canvas scaler. Adjust to match your UI design.
        public static readonly Vector2 ReferenceResolution = new Vector2(1920, 1080);

        // Duration for menu fade in/out animations (in seconds).
        public const float MenuFadeDuration = 0.2f;

        // Default colors for HUD bars. These can be overridden via inspector.
        public static readonly Color HealthColor = new Color(0.8f, 0.1f, 0.1f);
        public static readonly Color ArmorColor = new Color(0.2f, 0.4f, 0.8f);
        public static readonly Color HeatColor = new Color(0.8f, 0.4f, 0.0f);

        // Format strings for cash and progress display.
        public const string CashFormat = "$ {0:N0}";
        public const string ProgressFormat = "{0}/{1}";
    }
}