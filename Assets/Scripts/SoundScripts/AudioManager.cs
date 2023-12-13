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
        SceneIndex = SceneManager.GetActiveScene().buildIndex;
        ChangeMusicTheme();

        /*        PlayerPrefs.SetInt("SceneManager", SceneIndex);
                PlayerPrefs.Save();*/
    }

    private void Start()
    {

    }

    private void Update()
    {
        ChangeMusicTheme();
    }

    private void ChangeMusicTheme()
    {
        if (SceneIndex == 3 && !waveMusicSwitch)
        {
            PlayLoopingSound(musicTheme[0]);
            DisableSoundNoFade(musicTheme[1]);
        }
        else if(SceneIndex == 3 && waveMusicSwitch)
        {
            DisableSoundNoFade(musicTheme[0]);
            PlayLoopingSound(musicTheme[1]);
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
        float startVolume = s.source.volume;
        float timer = 0;
        float duration = .8f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            s.source.volume = Mathf.Lerp(startVolume, 0, timer / duration);
            yield return null;
        }
        s.source.Stop();
        s.source.volume = startVolume;
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
