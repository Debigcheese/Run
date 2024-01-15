using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoPlayerCheatScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            PlayerPrefs.SetInt("TotalCrystal", (PlayerPrefs.GetInt("TotalCrystal") + 500));
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            PlayerPrefs.SetInt("mustShopAfterLevel", 1);
            PlayerPrefs.SetInt("isUnlocked", 8);
            PlayerPrefs.SetInt("LevelsUnlocked", 8);
        }
    }
}
