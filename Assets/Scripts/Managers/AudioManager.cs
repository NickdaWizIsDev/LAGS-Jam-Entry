using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        [SerializeField] private AudioSource breakSFXSource;
        [SerializeField] private AudioSource pickaxeSFXSource;
        [SerializeField] private AudioSource musicSource;
        

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }

            Instance = this;
        }

        private void Start()
        {
            musicSource.Play();
        }

        public void PlayPickSFX()
        {
            pickaxeSFXSource.Play();
        }

        public void PlayBreakSFX()
        {
            breakSFXSource.Play();
        }
        
        public void ChangeMusicVolume(float vol)
        {
            musicSource.volume = vol;
        }
    
    }
}