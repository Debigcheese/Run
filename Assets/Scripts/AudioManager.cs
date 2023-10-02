using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

 // If you're using Audio Mixer

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; 

    public AudioSource musicSource;
    public AudioSource sfxSource;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Ensure that the AudioManager GameObject persists between scenes
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        musicSource.clip = musicClip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        sfxSource.PlayOneShot(sfxClip);
    }


    // Add audio-related functions (e.g., PlayMusic, PlaySFX, etc.) below
}
