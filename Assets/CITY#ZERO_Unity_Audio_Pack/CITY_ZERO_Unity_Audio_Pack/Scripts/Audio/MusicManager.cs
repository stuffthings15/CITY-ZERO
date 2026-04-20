using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CityZero.Audio
{
    /// <summary>
    /// Handles playback of background music. Supports playlists, crossfading,
    /// and adjustable music volume. Attach this to a persistent GameObject and
    /// assign playlists in the inspector or via code. Music is played through
    /// two AudioSources to allow smooth crossfades.
    /// </summary>
    public class MusicManager : MonoBehaviour
    {
        [Serializable]
        public class Playlist
        {
            public string name;
            public List<AudioClip> tracks;
            public bool loop = true;
            public AudioConstants.MusicCategory category = AudioConstants.MusicCategory.Exploration;
            public AudioMixerGroup mixerGroup;
        }

        public static MusicManager Instance { get; private set; }

        [Header("Playlists")]
        public Playlist[] playlists;

        [Header("Audio Sources")]
        public AudioSource primarySource;
        public AudioSource secondarySource;

        [Header("Settings")]
        [Range(0f, 1f)]
        public float musicVolume = AudioConstants.DefaultMusicVolume;
        public float crossfadeDuration = 2f;

        private Dictionary<string, Playlist> _playlistLookup;
        private Coroutine _playlistCoroutine;
        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            BuildLookup();
            InitializeSources();
        }

        private void BuildLookup()
        {
            _playlistLookup = new Dictionary<string, Playlist>(StringComparer.OrdinalIgnoreCase);
            foreach (var pl in playlists)
            {
                if (!string.IsNullOrEmpty(pl.name) && pl.tracks != null && pl.tracks.Count > 0)
                {
                    _playlistLookup[pl.name] = pl;
                }
            }
        }

        private void InitializeSources()
        {
            // Ensure we have two audio sources for crossfading
            if (primarySource == null)
            {
                GameObject go = new GameObject("PrimaryMusicSource");
                go.transform.SetParent(transform);
                primarySource = go.AddComponent<AudioSource>();
            }
            if (secondarySource == null)
            {
                GameObject go2 = new GameObject("SecondaryMusicSource");
                go2.transform.SetParent(transform);
                secondarySource = go2.AddComponent<AudioSource>();
            }
            primarySource.loop = false;
            secondarySource.loop = false;
            primarySource.volume = musicVolume;
            secondarySource.volume = 0f;
        }

        /// <summary>
        /// Plays the given playlist by name. If another playlist is currently
        /// playing, it will be stopped and the new playlist will start with
        /// a crossfade. If the playlist is not found, a warning is logged.
        /// </summary>
        public void PlayPlaylist(string playlistName)
        {
            if (!_playlistLookup.TryGetValue(playlistName, out var pl))
            {
                Debug.LogWarning($"MusicManager: Playlist '{playlistName}' not found.");
                return;
            }
            // Stop any existing playlist coroutine
            if (_playlistCoroutine != null)
            {
                StopCoroutine(_playlistCoroutine);
                _playlistCoroutine = null;
            }
            // Start the new playlist with a crossfade
            _playlistCoroutine = StartCoroutine(PlayPlaylistRoutine(pl));
        }

        private IEnumerator PlayPlaylistRoutine(Playlist playlist)
        {
            int trackIndex = 0;
            bool usePrimary = true;
            while (playlist != null && playlist.tracks.Count > 0)
            {
                AudioClip clip = playlist.tracks[trackIndex];
                if (clip != null)
                {
                    AudioSource playSource = usePrimary ? primarySource : secondarySource;
                    AudioSource fadeOutSource = usePrimary ? secondarySource : primarySource;
                    playSource.clip = clip;
                    playSource.outputAudioMixerGroup = playlist.mixerGroup;
                    playSource.volume = 0f;
                    playSource.loop = false;
                    playSource.Play();

                    // Fade in new track and fade out old track
                    if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
                    _fadeCoroutine = StartCoroutine(Crossfade(playSource, fadeOutSource, crossfadeDuration));

                    // Wait for track to finish minus fade time
                    yield return new WaitForSeconds(Mathf.Max(0f, clip.length - crossfadeDuration));
                }

                trackIndex = (trackIndex + 1) % playlist.tracks.Count;
                usePrimary = !usePrimary;
                if (!playlist.loop && trackIndex == 0)
                {
                    break;
                }
            }
        }

        private IEnumerator Crossfade(AudioSource fadeIn, AudioSource fadeOut, float duration)
        {
            float time = 0f;
            float startInVolume = fadeIn.volume;
            float startOutVolume = fadeOut.volume;
            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(time / duration);
                fadeIn.volume = Mathf.Lerp(0f, musicVolume, t);
                fadeOut.volume = Mathf.Lerp(startOutVolume, 0f, t);
                yield return null;
            }
            fadeIn.volume = musicVolume;
            fadeOut.volume = 0f;
            fadeOut.Stop();
        }

        /// <summary>
        /// Stops any currently playing music and fades out quickly.
        /// </summary>
        public void StopMusic(float fadeOutDuration = 1f)
        {
            if (_playlistCoroutine != null)
            {
                StopCoroutine(_playlistCoroutine);
                _playlistCoroutine = null;
            }
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }
            StartCoroutine(FadeOutAndStop(primarySource, fadeOutDuration));
            StartCoroutine(FadeOutAndStop(secondarySource, fadeOutDuration));
        }

        private IEnumerator FadeOutAndStop(AudioSource source, float duration)
        {
            float startVol = source.volume;
            float time = 0f;
            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(time / duration);
                source.volume = Mathf.Lerp(startVol, 0f, t);
                yield return null;
            }
            source.volume = 0f;
            source.Stop();
        }

        /// <summary>
        /// Sets the music volume. Adjusts both audio sources immediately.
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            primarySource.volume = musicVolume;
            secondarySource.volume = 0f;
        }
    }
}