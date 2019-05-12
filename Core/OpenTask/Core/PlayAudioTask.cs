using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ScriptableTask
{
    [CreateAssetMenu]
    public class PlayAudioTask : BaseScriptableTask
    {
        [SerializeField] AudioClip clip;
        [SerializeField] bool loop = false;
        private static List<AudioSource> audioSourcePool = new List<AudioSource>();
        private AudioSource source;

        public override void Execute()
        {
            // get first unused audioSource
            source = GetAFreeAudioSource();
            source.clip = clip;
            source.loop = loop;
            if (!source.isPlaying) source.Play();
        }

        private static AudioSource GetAFreeAudioSource()
        {
            AudioSource source = null;
            foreach (var audioSource in audioSourcePool)
            {
                // TODO: Checking only if an audio source is playing may couse 
                // prroblems when we pause an audio and play another one while 
                // this one is paused. how to check if an audio source is idle? who knows..
                if (!audioSource.isPlaying) source = audioSource;
            }
            if (source == null)
            {
                source = new GameObject("task-audio-source").AddComponent<AudioSource>();
                DontDestroyOnLoad(source.gameObject);
                audioSourcePool.Add(source);
            }
            return source;
        }

        public override void Stop()
        {
            if (source != null)
                if (source.isPlaying) source.Stop();
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
    }
}