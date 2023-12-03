using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] sounds;

    //Keys needed to show PlayerPrefs where to save the set sound from VolumSettings
    public const string MASTER_KEY = "masterVolume";
    public const string MUSIC_KEY = "musicVolume";
    public const string SFX_KEY = "sfxVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        //PlaySound("Theme");
    }

    public void PlaySound(string name)
    {

        Sound s = Array.Find(sounds, mSound => mSound.name == name);
        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            ChangePitch(s);
        }
    }

    public void PlayLoopingSound(string name)
    {

        Sound s = Array.Find(sounds, mSound => mSound.name == name);
        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            if (!s.source.isPlaying)
            {
                ChangePitch(s);
            }
        }
    }

    public void ChangePitch(Sound s)
    {
        float rng = UnityEngine.Random.Range(-s.randomPitchValue, s.randomPitchValue);
        s.source.pitch = s.pitch + rng;
        s.source.Play();
    }

    public void DisableSound(string name)
    {
        Sound s = Array.Find(sounds, mSound => mSound.name == name);
        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            s.source.Stop();
        }
    }
}
