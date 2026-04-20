using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CityZero.UI
{
    /// <summary>
    /// Controls the main menu and pause menu. This script handles showing and
    /// hiding menu panels, pausing the game, and responding to button clicks.
    /// It is intentionally simple; expand it to integrate with your save/load
    /// system, options panel, and other menus. Attach one instance of this
    /// script to each root menu canvas.
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        [Header("Panels")]
        [Tooltip("The root GameObject for the main menu (e.g. the start menu)")]
        public GameObject mainMenuPanel;

        [Tooltip("The root GameObject for the pause menu")]
        public GameObject pauseMenuPanel;

        [Tooltip("The root GameObject for the options panel")]
        public GameObject optionsPanel;

        [Header("Audio")]
        [Tooltip("Optional audio source to play a click sound when buttons are pressed")]
        public AudioSource uiAudioSource;

        [Tooltip("Clip to play when a button is pressed")]
        public AudioClip buttonClickClip;

        public bool IsPaused { get; private set; }

        private void Start()
        {
            // Ensure only main menu is active by default (if assigned)
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            if (optionsPanel != null) optionsPanel.SetActive(false);
        }

        #region Public API

        /// <summary>
        /// Called by UI buttons to start a new game. Replace with your own logic.
        /// </summary>
        public void OnNewGame()
        {
            PlayClickSound();
            Debug.Log("New Game button pressed");
            // TODO: Initialize game state and load first level.
        }

        /// <summary>
        /// Called by UI buttons to load an existing game. Replace with your load logic.
        /// </summary>
        public void OnLoadGame()
        {
            PlayClickSound();
            Debug.Log("Load Game button pressed");
            // TODO: Display load game UI or load last save.
        }

        /// <summary>
        /// Opens the options panel.
        /// </summary>
        public void OnOptions()
        {
            PlayClickSound();
            if (optionsPanel != null)
            {
                bool isActive = optionsPanel.activeSelf;
                optionsPanel.SetActive(!isActive);
            }
        }

        /// <summary>
        /// Quits the game. Replace with platform-specific quit behaviour if needed.
        /// </summary>
        public void OnQuit()
        {
            PlayClickSound();
            Debug.Log("Quit button pressed");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// Resume the game when pause menu is active.
        /// </summary>
        public void OnResume()
        {
            PlayClickSound();
            TogglePauseMenu();
        }

        /// <summary>
        /// Toggle the pause menu. Call when escape key or pause button is pressed.
        /// </summary>
        public void TogglePauseMenu()
        {
            if (pauseMenuPanel == null) return;
            IsPaused = !IsPaused;
            pauseMenuPanel.SetActive(IsPaused);
            if (optionsPanel != null && optionsPanel.activeSelf) optionsPanel.SetActive(false);
            Time.timeScale = IsPaused ? 0f : 1f;
        }

        #endregion

        private void PlayClickSound()
        {
            if (uiAudioSource != null && buttonClickClip != null)
            {
                uiAudioSource.PlayOneShot(buttonClickClip);
            }
        }
    }
}