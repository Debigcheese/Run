 using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;


public class LevelSelectBackgroundMoving : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] private int level = 0;
    [SerializeField] public bool isSwitchingLevelBackground = false;
    [SerializeField] private int levelsUnlocked;

    [SerializeField] GameObject[] backgrounds;

    // Start is called before the first frame update
    void Start()
    {
        level = PlayerPrefs.GetInt("LevelBackground", 3);
        levelsUnlocked = PlayerPrefs.GetInt("LevelsUnlocked", 3);

        for (int i = 0; i < backgrounds.Length; i++)
        {

            backgrounds[i].SetActive(false);

            if (backgrounds[i] != null && backgrounds[level - 3] == backgrounds[i])
            {
                backgrounds[level - 3].SetActive(true);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SetIsHoveringLevel(int getLevel)
    {

        if (level != getLevel  && !isSwitchingLevelBackground && levelsUnlocked >= level)
        {
            level = getLevel;

            if(level <= levelsUnlocked)
            {
                PlayerPrefs.SetInt("LevelBackground", level);
                StartCoroutine(SetBackground());
            }
        }
    }

    private IEnumerator SetBackground()
    {
        isSwitchingLevelBackground = true;
        anim.SetBool("isStartingTransition", true);

        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < backgrounds.Length; i++)
        {

            backgrounds[i].SetActive(false);

            if (backgrounds[i] != null && backgrounds[level - 3] == backgrounds[i])
            {
                backgrounds[level - 3].SetActive(true);
            }
        }

        yield return new WaitForSeconds(.6f);
        anim.SetBool("isStartingTransition", false);
        isSwitchingLevelBackground = false;
    }
}
