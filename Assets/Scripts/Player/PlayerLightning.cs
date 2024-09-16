using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerLightning : MonoBehaviour
{
    [SerializeField] private GameObject[] PlayerLevelLight;

    // Start is called before the first frame update
    void Start()
    {
        //level 1 == 3
        //level 2 == 4
        //level 3 == 5
        //level 4 == 6

        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log(levelIndex);

        for (int i = 0; i < PlayerLevelLight.Length; i++)
        {
            PlayerLevelLight[i].SetActive(false);

            if (i == levelIndex - 3)
            {
                PlayerLevelLight[i].SetActive(true);
                Debug.Log(PlayerLevelLight[i]);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
