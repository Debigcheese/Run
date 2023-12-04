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

    //settings
    public Slider musicSlider;
    public Slider SFXSlider;
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup SFXMixerGroup;
    public GameObject fullscreenActive;
    public GameObject fullscreenNotActive;

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

        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1));
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1);
    }

    private void Start()
    {

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
            RandomPitch(s);
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
                RandomPitch(s);
            }
        }
    }

    public void RandomPitch(Sound s)
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


    //settings
    public void SetVolume(AudioMixerGroup mixerGroup, float decimalVolume, string exposedParam)
    {
        float dbVolume = Mathf.Log10(decimalVolume) * 20;
        if (decimalVolume == 0.0f)
        {
            dbVolume = -80.0f;
        }

        mixerGroup.audioMixer.SetFloat(exposedParam, dbVolume);
        PlayerPrefs.SetFloat(exposedParam, decimalVolume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float decimalVolume)
    {
        SetVolume(musicMixerGroup, decimalVolume, "MusicVolume");
    }

    public void SetSFXVolume(float decimalVolume)
    {
        SetVolume(SFXMixerGroup, decimalVolume, "SFXVolume");
    }


    public void ToggleFullScreen(int check)
    {
        Screen.fullScreen = !Screen.fullScreen;
        if (check == 0)
        {
            fullscreenActive.SetActive(false);
            fullscreenNotActive.SetActive(true);
        }
        if (check == 1)
        {
            fullscreenActive.SetActive(true);
            fullscreenNotActive.SetActive(false);
        }

    }
}
