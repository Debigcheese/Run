using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] sounds;

    private int SceneIndex = 0;
    public bool waveMusicSwitch;
    public string[] musicTheme;

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
        else
        {
            Destroy(gameObject);
        }

        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1));
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1);
        ManageMusicTheme();

        /*        PlayerPrefs.SetInt("SceneManager", SceneIndex);
                PlayerPrefs.Save();*/
    }

    private void Start()
    {

    }

    private void Update()
    {
        SceneIndex = SceneManager.GetActiveScene().buildIndex;
        ManageMusicTheme();
    }

    private void ManageMusicTheme()
    {
        if(SceneIndex < 3)
        {
            PlayLoopingSound(musicTheme[0]);
        }
        else if(SceneIndex >= 3)
        {
            DisableSound(musicTheme[0]);
        }

        if (SceneIndex == 3 && !waveMusicSwitch)
        {
            DisableAllExceptOneMusic(musicTheme[1]);
        }
        else if(SceneIndex == 3 && waveMusicSwitch)
        {
            DisableAllExceptOneMusic(musicTheme[2]);
        }
        if (SceneIndex == 4 && !waveMusicSwitch)
        {
            DisableAllExceptOneMusic(musicTheme[3]);
        }
        else if (SceneIndex == 4 && waveMusicSwitch)
        {
            DisableAllExceptOneMusic(musicTheme[4]);
        }
    }

    public void DisableAllExceptOneMusic(string name)
    {
        for(int i = 0; i<musicTheme.Length; i++)
        {
            if(musicTheme[i] == name)
            {
                PlayLoopingSound(name);
            }
            else
            {
                DisableSoundNoFade(musicTheme[i]);
            }
        }
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
        s.source.pitch = s.pitch;
        float rng = UnityEngine.Random.Range(-s.randomPitchValue, s.randomPitchValue);
        s.source.pitch = s.pitch + rng;
        s.source.volume = s.volume;
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
            StartCoroutine(FadeOut(s));
        }
    }

    public void DisableSoundNoFade(string name)
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

    public IEnumerator FadeOut(Sound s)
    {
        float timer = 0;
        float duration = .8f;
        s.source.volume = s.volume;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            s.source.volume = Mathf.Lerp(s.volume, 0, timer / duration);
            yield return null;
        }
        s.source.Stop();
        s.source.volume = s.volume;
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
        AudioManager.Instance.PlaySound("uibutton");

    }
}
