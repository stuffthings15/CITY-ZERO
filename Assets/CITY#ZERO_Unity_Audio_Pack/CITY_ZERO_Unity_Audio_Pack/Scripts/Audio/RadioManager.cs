using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CityZero.Audio
{
    /// <summary>
    /// Manages radio channels for in‑game vehicles or ambient music. Channels
    /// consist of playlists of tracks that loop sequentially. The radio
    /// supports channel switching with optional crossfading. Attach this
    /// component to a persistent GameObject and populate channels in the
    /// inspector.
    /// </summary>
    public class RadioManager : MonoBehaviour
    {
        [Serializable]
        public class RadioChannel
        {
            public string name;
            public List<AudioClip> tracks;
            public bool loop = true;
            public AudioMixerGroup mixerGroup;
        }

        public static RadioManager Instance { get; private set; }

        [Header("Radio Channels")]
        public RadioChannel[] channels;

        [Header("Audio Sources")]
        public AudioSource radioSourceA;
        public AudioSource radioSourceB;

        [Header("Settings")]
        [Range(0f, 1f)]
        public float radioVolume = AudioConstants.DefaultRadioVolume;
        public float crossfadeDuration = 1.5f;

        private Dictionary<string, RadioChannel> _channelLookup;
        private int _currentChannelIndex = -1;
        private Coroutine _channelRoutine;
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
            _channelLookup = new Dictionary<string, RadioChannel>(StringComparer.OrdinalIgnoreCase);
            foreach (var ch in channels)
            {
                if (!string.IsNullOrEmpty(ch.name) && ch.tracks != null && ch.tracks.Count > 0)
                {
                    _channelLookup[ch.name] = ch;
                }
            }
        }

        private void InitializeSources()
        {
            if (radioSourceA == null)
            {
                GameObject go = new GameObject("RadioSourceA");
                go.transform.SetParent(transform);
                radioSourceA = go.AddComponent<AudioSource>();
            }
            if (radioSourceB == null)
            {
                GameObject go2 = new GameObject("RadioSourceB");
                go2.transform.SetParent(transform);
                radioSourceB = go2.AddComponent<AudioSource>();
            }
            radioSourceA.loop = false;
            radioSourceB.loop = false;
            radioSourceA.volume = radioVolume;
            radioSourceB.volume = 0f;
        }

        /// <summary>
        /// Tunes to a specific channel by name. If the channel does not exist, a
        /// warning is logged. Passing null or empty string will stop the radio.
        /// </summary>
        public void TuneToChannel(string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                StopRadio();
                return;
            }
            if (!_channelLookup.TryGetValue(channelName, out var ch))
            {
                Debug.LogWarning($"RadioManager: Channel '{channelName}' not found.");
                return;
            }
            int newIndex = Array.IndexOf(channels, ch);
            if (newIndex == _currentChannelIndex)
            {
                return; // already playing
            }
            _currentChannelIndex = newIndex;
            StartChannel(ch);
        }

        /// <summary>
        /// Tunes to the next channel in the list. Wraps around when reaching the end.
        /// </summary>
        public void NextChannel()
        {
            if (channels == null || channels.Length == 0) return;
            int nextIndex = (_currentChannelIndex + 1) % channels.Length;
            _currentChannelIndex = nextIndex;
            StartChannel(channels[_currentChannelIndex]);
        }

        /// <summary>
        /// Tunes to the previous channel in the list. Wraps around when at the beginning.
        /// </summary>
        public void PreviousChannel()
        {
            if (channels == null || channels.Length == 0) return;
            int prevIndex = (_currentChannelIndex - 1 + channels.Length) % channels.Length;
            _currentChannelIndex = prevIndex;
            StartChannel(channels[_currentChannelIndex]);
        }

        private void StartChannel(RadioChannel channel)
        {
            if (_channelRoutine != null)
            {
                StopCoroutine(_channelRoutine);
                _channelRoutine = null;
            }
            _channelRoutine = StartCoroutine(PlayChannelRoutine(channel));
        }

        private IEnumerator PlayChannelRoutine(RadioChannel channel)
        {
            int trackIndex = 0;
            bool useA = true;
            while (channel != null && channel.tracks.Count > 0)
            {
                AudioClip clip = channel.tracks[trackIndex];
                if (clip != null)
                {
                    AudioSource playSource = useA ? radioSourceA : radioSourceB;
                    AudioSource fadeOutSource = useA ? radioSourceB : radioSourceA;
                    playSource.clip = clip;
                    playSource.outputAudioMixerGroup = channel.mixerGroup;
                    playSource.volume = 0f;
                    playSource.loop = false;
                    playSource.Play();

                    if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
                    _fadeCoroutine = StartCoroutine(RadioCrossfade(playSource, fadeOutSource, crossfadeDuration));

                    // Wait for track end minus crossfade
                    yield return new WaitForSeconds(Mathf.Max(0f, clip.length - crossfadeDuration));
                }
                trackIndex = (trackIndex + 1) % channel.tracks.Count;
                useA = !useA;
                if (!channel.loop && trackIndex == 0)
                {
                    break;
                }
            }
        }

        private IEnumerator RadioCrossfade(AudioSource fadeIn, AudioSource fadeOut, float duration)
        {
            float time = 0f;
            float startOutVol = fadeOut.volume;
            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(time / duration);
                fadeIn.volume = Mathf.Lerp(0f, radioVolume, t);
                fadeOut.volume = Mathf.Lerp(startOutVol, 0f, t);
                yield return null;
            }
            fadeIn.volume = radioVolume;
            fadeOut.volume = 0f;
            fadeOut.Stop();
        }

        /// <summary>
        /// Stops radio playback and fades out quickly.
        /// </summary>
        public void StopRadio(float fadeOutDuration = 0.5f)
        {
            if (_channelRoutine != null)
            {
                StopCoroutine(_channelRoutine);
                _channelRoutine = null;
            }
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }
            StartCoroutine(FadeOutAndStop(radioSourceA, fadeOutDuration));
            StartCoroutine(FadeOutAndStop(radioSourceB, fadeOutDuration));
            _currentChannelIndex = -1;
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
        /// Sets the radio volume and updates both audio sources.
        /// </summary>
        public void SetRadioVolume(float volume)
        {
            radioVolume = Mathf.Clamp01(volume);
            // Immediately adjust currently playing source volumes proportionally
            if (_currentChannelIndex >= 0)
            {
                AudioSource currentPlaying = radioSourceA.isPlaying ? radioSourceA : radioSourceB;
                currentPlaying.volume = radioVolume;
            }
        }
    }
}