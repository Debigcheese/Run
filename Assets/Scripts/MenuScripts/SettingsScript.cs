using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsScript : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioMixer audioMixer;
    public GameObject fullscreenActive;
    public GameObject fullscreenNotActive;


    // Start is called before the first frame update
    void Start()
    {
        SetVolume(PlayerPrefs.GetFloat("MasterVolume", 1));
        volumeSlider.onValueChanged.AddListener(SetVolume);
        volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume",1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVolume(float decimalVolume)
    {
        float dbVolume = Mathf.Log10(decimalVolume) * 20;
        if (decimalVolume == 0.0f)
        {
            dbVolume = -80.0f;
        }
        audioMixer.SetFloat("Volume", dbVolume); 
        PlayerPrefs.SetFloat("MasterVolume", decimalVolume);
        PlayerPrefs.Save();
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
