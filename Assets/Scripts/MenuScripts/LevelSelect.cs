using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelect : MonoBehaviour
{
    public Animator transitionAnim;
    public Animator[] letterAnim;
    public Animator[] LockAnim;
    public GameObject transitionImage;
    public float transitionDuration = 1.5f;
    public int levelsUnlocked;
    private int keyUnlocked;

    // Start is called before the first frame update
    void Start()
    {
        transitionImage.SetActive(true);
        transitionAnim.SetBool("TransitionEnd", true);
        StartCoroutine(EndTransition());
        levelsUnlocked = PlayerPrefs.GetInt("LevelsUnlocked", 3);
        keyUnlocked = PlayerPrefs.GetInt("isUnlocked", 3);

        for (int i = 0; i < letterAnim.Length; i++)
        {
            if (i+2 < levelsUnlocked)
            {
                letterAnim[i].SetBool("LetterLocked", false);
                if (i + 2 < keyUnlocked)
                {
                    LockAnim[i].SetBool("isUnlocked", true);
                }
                else if(i + 2 <= keyUnlocked)
                {
                    LockAnim[i].SetBool("Unlock", true);
                    StartCoroutine(LockAnimIsUnlocked(levelsUnlocked));
                }
            }
            else
            {
                letterAnim[i].SetBool("LetterLocked", true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
   


    }

    public IEnumerator LoadScene(int level)
    {
        if(levelsUnlocked >= level)
        {
            if(level != 2)
            {
                letterAnim[level - 3].SetBool("LetterOpen", true);
            }
            transitionImage.SetActive(true);
            transitionAnim.SetBool("TransitionStart", true);
            yield return new WaitForSeconds(transitionDuration);
            SceneManager.LoadScene(level);
        }
    }

    private IEnumerator LockAnimIsUnlocked(int levelsUnlocked)
    {
        PlayerPrefs.SetInt("isUnlocked", levelsUnlocked);
        PlayerPrefs.Save();
        yield return new WaitForSeconds(3f);
        LockAnim[levelsUnlocked].SetBool("Unlock", false);
    }

    private IEnumerator EndTransition()
    {
        yield return new WaitForSeconds(1f);
        transitionAnim.SetBool("TransitionEnd", false);
        transitionImage.SetActive(false);
    }

    public void OpenShop()
    {
        StartCoroutine(LoadScene(2));
    }

    public void SelectLevel1()
    {
        StartCoroutine(LoadScene(3));
    }

    public void SelectLevel2()
    {
        StartCoroutine(LoadScene(4));
    }

    public void SelectLevel3()
    {
        StartCoroutine(LoadScene(5));
    }

    public void SelectLevel4()
    {
        StartCoroutine(LoadScene(6));
    }

    public void SelectLevel5()
    {
        StartCoroutine(LoadScene(7));
    }
}
