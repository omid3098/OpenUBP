using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace ScriptableTask
{
    /// <summary>
    /// We create a game object with an audiosource and if the current audio  was not looped we will disable AudioSource component after play.
    /// next time we check for deactivated audiosources in pool and if we did not find any, we make a new one
    /// </summary>

    [CreateAssetMenu(menuName = "Task/Audio", fileName = "AudioTask")]
    public class AudioTask : BaseScriptableTask
    {
        [SerializeField] AudioClip clip;
        [SerializeField] bool loop = false;
        private static List<AudioSource> audioSourcePool = new List<AudioSource>();
        private AudioSource source
        {
            get
            {
                if (_source == null)
                {
                    // get the first unused audioSource
                    foreach (var audioSource in audioSourcePool)
                    {
                        if (!audioSource.enabled)
                        {
                            _source = audioSource;
                            _source.enabled = true;
                        }
                    }
                    // if no usused audio detected, create a new one
                    if (_source == null)
                    {
                        _source = new GameObject("task-audio-source").AddComponent<AudioSource>();
                        DontDestroyOnLoad(_source.gameObject);
                        audioSourcePool.Add(_source);
                    }
                }
                return _source;
            }
            set { _source = value; }
        }
        private AudioSource _source;

        public override void Start()
        {
            source.clip = clip;
            source.loop = loop;
            if (!source.isPlaying) source.Play();
            if (OnStart != null) OnStart.Invoke();
            if (!loop) FreeSource(source.clip.length);
        }

        private async void FreeSource(float delay)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(delay));
                source.enabled = false;
                _source = null;
            }
            catch { throw; }
        }

        public void SetVolume(float value)
        {
            if (source != null) source.volume = value;
        }

        public void SetMixerGroup(AudioMixerGroup mixerGroup)
        {
            if (source != null)
            {
                source.outputAudioMixerGroup = mixerGroup;
            }
        }

        public void Mute()
        {
            if (source != null) source.mute = true;
        }

        public void UnMute()
        {
            if (source != null) source.mute = false;
        }

        internal void SetPitch(float _pitch)
        {
            if (source != null) source.pitch = _pitch;
        }

        public override void Pause()
        {
            if (source != null) source.Pause();
        }

        public override void Stop()
        {
            if (source != null)
            {
                if (source.isPlaying) source.Stop();
                if (OnStop != null) OnStop.Invoke();
            }
        }

        public override void Update() { }
    }
}