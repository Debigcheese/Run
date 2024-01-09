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
    private int mustShopAfterLevel;
    public int crystalAmount;
    public TextMeshProUGUI crystalText;
    public GameObject[] shopObject;
    public TextMeshProUGUI shopText;
    public Color originalColor;
    public Color unavailableColor;

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
        crystalText.text = PlayerPrefs.GetInt("TotalCrystal", 0).ToString();
        mustShopAfterLevel = PlayerPrefs.GetInt("mustShopAfterLevel", 3);
        if (mustShopAfterLevel == 3)
        {
            foreach(GameObject shop in shopObject) 
            {
                shop.GetComponent<Image>().color = unavailableColor;
            }
            shopText.color = unavailableColor;
        }
        else
        {
            foreach (GameObject shop in shopObject)
            {

                shop.GetComponent<Image>().color = originalColor;
            }
            shopText.color = originalColor;
        }
    }

    public IEnumerator LoadLevel(int level)
    {
        if (levelsUnlocked >= level && (mustShopAfterLevel == 1 || mustShopAfterLevel == 3))
        {
            letterAnim[level - 3].SetBool("LetterOpen", true);
            AudioManager.Instance.PlaySound("uibuttonturnpage");

            transitionImage.SetActive(true);
            transitionAnim.SetBool("TransitionStart", true);
            yield return new WaitForSeconds(transitionDuration);
            SceneManager.LoadScene(level);
        }
        else
        {
            AudioManager.Instance.PlaySound("uibuttonwrong");
        }
    }

    public IEnumerator LoadShop(int level)
    {
        if (levelsUnlocked >= level && (mustShopAfterLevel == 1 || mustShopAfterLevel == 2))
        {
            AudioManager.Instance.PlaySound("uibutton");
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
        AudioManager.Instance.PlaySound("lockopen");
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
        StartCoroutine(LoadShop(2));
    }

    public void SelectLevel1()
    {
        StartCoroutine(LoadLevel(3));
    }

    public void SelectLevel2()
    {
        StartCoroutine(LoadLevel(4));
    }

    public void SelectLevel3()
    {
        StartCoroutine(LoadLevel(5));
    }

    public void SelectLevel4()
    {
        StartCoroutine(LoadLevel(6));
    }

    public void SelectLevel5()
    {
        StartCoroutine(LoadLevel(7));
    }
}
