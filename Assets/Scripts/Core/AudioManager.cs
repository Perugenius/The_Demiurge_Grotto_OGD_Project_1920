using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;

        [Range(0f,1f)]
        public float volume = 0.7f;
        [Range(.5f,1.5f)]
        public float pitch = 1f;

        [Range(0f,0.5f)]
        public float randomVolume = 0.1f;
        [Range(0f,0.5f)]
        public float randomPitch = 0.1f;

        private AudioSource _source;

        public void SetSource(AudioSource source)
        {
            _source = source;
            _source.clip = clip;
        }

        public void Play()
        {
            _source.volume = volume * (1 + Random.Range(-randomVolume/2, randomVolume/2));
            _source.pitch = pitch * (1 + Random.Range(-randomPitch/2, randomPitch/2));
            _source.Play();
        }

        public void Stop()
        {
            _source.Stop();
        }

    }

    public class AudioManager: Singleton<AudioManager>
    {
        [SerializeField] private Sound[] sounds;

        private void Start()
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                GameObject x = new GameObject("Sound_" + i + "_" + sounds[i].name);
                sounds[i].SetSource(x.AddComponent<AudioSource>());
                DontDestroyOnLoad(x);
            }
        }

        public void PlaySound(string soundName)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i].name == soundName)
                {
                    sounds[i].Play();
                    return;
                }
            }
            Debug.Log("No sound found with name: " + soundName);
        }

        public void StopSound(string soundName)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i].name == soundName)
                {
                    sounds[i].Stop();
                    return;
                }
            }
            Debug.Log("No sound found with name: " + soundName);
        }
    }
}