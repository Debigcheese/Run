using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioSource source;

    public float pitch;
    public float randomPitchValue = 0.1f;

}

