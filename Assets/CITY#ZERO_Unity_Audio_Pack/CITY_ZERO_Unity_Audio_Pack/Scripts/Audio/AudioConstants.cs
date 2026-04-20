using UnityEngine;

namespace CityZero.Audio
{
    /// <summary>
    /// Defines common enums and default values used across the audio system. You
    /// can extend these enums to include additional categories specific to your
    /// game. Modify the default volume constants to control global sound levels.
    /// </summary>
    public static class AudioConstants
    {
        /// <summary>
        /// Categories for sound effects. Use these to control volumes per category
        /// or route sounds to different mixer groups.
        /// </summary>
        public enum SFXCategory
        {
            Default,
            Weapon,
            Vehicle,
            UI,
            Footstep,
            Ambience
        }

        /// <summary>
        /// Categories for music tracks. You can use this to differentiate between
        /// exploration, combat, radio, and stinger music. Extend as needed.
        /// </summary>
        public enum MusicCategory
        {
            Exploration,
            Combat,
            Radio,
            Stinger
        }

        // Default volumes (0–1). Adjust these values or expose them in your options menu.
        public const float DefaultSFXVolume = 0.7f;
        public const float DefaultMusicVolume = 0.5f;
        public const float DefaultRadioVolume = 0.6f;
    }
}