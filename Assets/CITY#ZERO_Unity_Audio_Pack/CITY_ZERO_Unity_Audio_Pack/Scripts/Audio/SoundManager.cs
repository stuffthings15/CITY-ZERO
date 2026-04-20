using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CityZero.Audio
{
    /// <summary>
    /// Manages playback of sound effects (SFX). Supports playing clips by name,
    /// pooling AudioSource components for efficiency, and setting volume per
    /// category or globally. Assign this component to a singleton GameObject.
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        /// <summary>
        /// A serializable definition of a sound effect. Assign these in the
        /// inspector to populate the sound library. Use the name field to
        /// reference clips at runtime.
        /// </summary>
        [Serializable]
        public class SFXItem
        {
            public string name;
            public AudioClip clip;
            public AudioConstants.SFXCategory category = AudioConstants.SFXCategory.Default;
            public AudioMixerGroup mixerGroup;
        }

        public static SoundManager Instance { get; private set; }

        [Header("Sound Library")]
        [Tooltip("List of sound effects available for playback. Fill this list in the inspector.")]
        public SFXItem[] sfxItems;

        [Header("Pool Settings")]
        [Tooltip("Initial number of pooled AudioSources for 3D sounds.")]
        public int initialPoolSize = 16;

        [Tooltip("Whether to expand the pool automatically when needed.")]
        public bool allowPoolExpansion = true;

        [Header("Volume Settings")]
        [Range(0f, 1f)]
        public float sfxVolume = AudioConstants.DefaultSFXVolume;

        private Dictionary<string, SFXItem> _sfxLookup;
        private List<AudioSource> _audioSources;
        private int _poolIndex;

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
            InitializePool();
        }

        private void BuildLookup()
        {
            _sfxLookup = new Dictionary<string, SFXItem>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in sfxItems)
            {
                if (!string.IsNullOrEmpty(item.name) && item.clip != null)
                {
                    _sfxLookup[item.name] = item;
                }
            }
        }

        private void InitializePool()
        {
            _audioSources = new List<AudioSource>(initialPoolSize);
            for (int i = 0; i < initialPoolSize; i++)
            {
                var src = CreateNewSource();
                _audioSources.Add(src);
            }
            _poolIndex = 0;
        }

        private AudioSource CreateNewSource()
        {
            GameObject go = new GameObject("SFXSource");
            go.transform.SetParent(transform);
            AudioSource src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 1f; // default to 3D sound
            src.volume = sfxVolume;
            return src;
        }

        /// <summary>
        /// Plays a sound effect at a given world position. If the sound is not found,
        /// no action is taken. Volume is multiplied by the global SFX volume.
        /// </summary>
        public void PlaySFX(string name, Vector3 position, float volume = 1f)
        {
            if (!_sfxLookup.TryGetValue(name, out var item) || item.clip == null)
            {
                Debug.LogWarning($"SoundManager: SFX '{name}' not found.");
                return;
            }
            AudioSource src = GetAvailableSource();
            src.transform.position = position;
            src.clip = item.clip;
            src.outputAudioMixerGroup = item.mixerGroup;
            src.volume = sfxVolume * volume;
            src.spatialBlend = 1f;
            src.Stop();
            src.Play();
        }

        /// <summary>
        /// Plays a non-positional (2D) sound effect. Useful for UI clicks or stingers.
        /// </summary>
        public void PlaySFX2D(string name, float volume = 1f)
        {
            if (!_sfxLookup.TryGetValue(name, out var item) || item.clip == null)
            {
                Debug.LogWarning($"SoundManager: SFX '{name}' not found.");
                return;
            }
            AudioSource src = GetAvailableSource();
            src.transform.position = Vector3.zero;
            src.clip = item.clip;
            src.outputAudioMixerGroup = item.mixerGroup;
            src.volume = sfxVolume * volume;
            src.spatialBlend = 0f;
            src.Stop();
            src.Play();
        }

        /// <summary>
        /// Sets the global SFX volume and updates all pooled sources.
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            foreach (var src in _audioSources)
            {
                src.volume = sfxVolume;
            }
        }

        private AudioSource GetAvailableSource()
        {
            for (int i = 0; i < _audioSources.Count; i++)
            {
                _poolIndex = (_poolIndex + 1) % _audioSources.Count;
                if (!_audioSources[_poolIndex].isPlaying)
                {
                    return _audioSources[_poolIndex];
                }
            }
            // If all sources are playing and expansion allowed, create a new one
            if (allowPoolExpansion)
            {
                var newSource = CreateNewSource();
                _audioSources.Add(newSource);
                return newSource;
            }
            // Otherwise return the first source (will interrupt if necessary)
            return _audioSources[0];
        }
    }
}