/*
File Name:    AudioSystem.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace RollbackSupport
{
    public class AudioSystem : MonoBehaviour
    {
        public static AudioSystem Instance { get; private set; }

        [Header("Pool Settings")]
        [SerializeField] private int _poolSize = 32;
        [SerializeField] private bool _dontDestroyOnLoad = true;

        [Header("Mixer Groups (Optional)")]
        public AudioMixerGroup sfxMixer;
        public AudioMixerGroup voiceMixer;
        public AudioMixerGroup bgmMixer;
        public AudioMixerGroup uiMixer;

        private readonly List<AudioSource> _pooledSources = new List<AudioSource>();
        private readonly List<float> _lastPlayTimes = new List<float>();

        // Dedicated BGM source so we can loop and control music separately
        private AudioSource _bgmSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            InitializePool();
            InitializeBGMSource();
        }

        private void InitializePool()
        {
            for (int i = 0; i < _poolSize; ++i)
            {
                var go = new GameObject($"PooledAudioSource_{i}");
                go.transform.SetParent(transform);
                var source = go.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 1.0f; // 3D by default for SFX/VO
                _pooledSources.Add(source);
                _lastPlayTimes.Add(0f);
            }
        }

        private void InitializeBGMSource()
        {
            var go = new GameObject("BGMSource");
            go.transform.SetParent(transform);
            _bgmSource = go.AddComponent<AudioSource>();
            _bgmSource.playOnAwake = false;
            _bgmSource.loop = true;
            _bgmSource.spatialBlend = 0.0f; // 2D
            if (bgmMixer != null)
            {
                _bgmSource.outputAudioMixerGroup = bgmMixer;
            }
        }

        private AudioSource GetFreeSource(AudioBus bus, Vector3 position)
        {
            int bestIndex = -1;
            float bestTime = float.MaxValue;

            for (int i = 0; i < _pooledSources.Count; ++i)
            {
                var src = _pooledSources[i];
                if (!src.isPlaying)
                {
                    ConfigureSourceForBus(src, bus);
                    src.transform.position = position;
                    _lastPlayTimes[i] = Time.time;
                    return src;
                }

                // If all are playing, we will steal the oldest one
                if (_lastPlayTimes[i] < bestTime)
                {
                    bestTime = _lastPlayTimes[i];
                    bestIndex = i;
                }
            }

            // Steal the oldest source
            var stolen = _pooledSources[bestIndex];
            ConfigureSourceForBus(stolen, bus);
            stolen.transform.position = position;
            _lastPlayTimes[bestIndex] = Time.time;
            return stolen;
        }

        private void ConfigureSourceForBus(AudioSource src, AudioBus bus)
        {
            switch (bus)
            {
                case AudioBus.SFX:
                    src.spatialBlend = 1.0f;
                    if (sfxMixer != null)
                        src.outputAudioMixerGroup = sfxMixer;
                    break;

                case AudioBus.Voice:
                    src.spatialBlend = 1.0f;
                    if (voiceMixer != null)
                        src.outputAudioMixerGroup = voiceMixer;
                    break;

                case AudioBus.UI:
                    src.spatialBlend = 0.0f;
                    if (uiMixer != null)
                        src.outputAudioMixerGroup = uiMixer;
                    break;

                default:
                    src.spatialBlend = 1.0f;
                    break;
            }
        }

        /// <summary>
        /// Low-level play function. Use the convenience wrappers when possible.
        /// </summary>
        public void Play(AudioClip clip, Vector3 position, AudioBus bus,
                         float volume = 1f, float pitch = 1f)
        {
            if (clip == null)
                return;

            if (bus == AudioBus.BGM)
            {
                PlayBGM(clip, volume);
                return;
            }

            var src = GetFreeSource(bus, position);
            src.clip = clip;
            src.volume = volume;
            src.pitch = pitch;
            src.Stop();
            src.Play();
        }

        #region Convenience Wrappers

        public void PlaySFX(AudioClip clip, Vector3 position,
                            float volume = 1f, float pitch = 1f)
        {
            Play(clip, position, AudioBus.SFX, volume, pitch);
        }

        public void PlayVoice(AudioClip clip, Vector3 position,
                              float volume = 1f, float pitch = 1f)
        {
            Play(clip, position, AudioBus.Voice, volume, pitch);
        }

        public void PlayUI(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            // UI is 2D, position doesn’t matter
            Play(clip, Vector3.zero, AudioBus.UI, volume, pitch);
        }

        public void PlayBGM(AudioClip clip, float volume = 1f)
        {
            if (clip == null)
                return;

            _bgmSource.clip = clip;
            _bgmSource.volume = volume;
            _bgmSource.loop = true;
            if (!_bgmSource.isPlaying)
            {
                _bgmSource.Play();
            }
            else
            {
                // Simple restart; you can add crossfade later if you want
                _bgmSource.Stop();
                _bgmSource.Play();
            }
        }

        public void StopBGM()
        {
            _bgmSource.Stop();
            _bgmSource.clip = null;
        }

        #endregion
    }
}
