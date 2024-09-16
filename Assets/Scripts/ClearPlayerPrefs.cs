using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearPlayerPrefs : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("DeletePlayerPrefs") == 0)
        {
            // This will delete all PlayerPrefs at the start of the game
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        PlayerPrefs.SetInt("DeletePlayerPrefs", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
