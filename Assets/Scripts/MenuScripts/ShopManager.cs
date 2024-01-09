using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public GameObject[] panels;
    public TextMeshProUGUI crystalText;
    public int equippedAbility;
    public int[] abilityIndex;
    public GameObject[] AbilitiesUI;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("mustShopAfterLevel", 1);
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        panels[0].SetActive(true);

        equippedAbility = PlayerPrefs.GetInt("Ability", 0);

        for (int i = 0; i < AbilitiesUI.Length; i++)
        {
            if (equippedAbility == abilityIndex[i])
            {
                AbilitiesUI[equippedAbility].SetActive(true);
            }
            else
            {
                AbilitiesUI[i].SetActive(false);
            }
        }

        PlayerPrefs.Save();
    }

    // Update is called once per frame
    void Update()
    {
        crystalText.text = PlayerPrefs.GetInt("TotalCrystal", 0).ToString();
    }

    public void LeaveShop()
    {
        AudioManager.Instance.PlaySound("uibutton");
        FindObjectOfType<TransitionScript>().TransitionStart();
        StartCoroutine(LoadLevelSelect());
    }

    private IEnumerator LoadLevelSelect()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
    }

    public void OpenWeaponsPanel()
    {
        OpenPanel(0);
    }

    public void OpenAbilityPanel()
    {
        OpenPanel(1);
    }

    public void OpenStatsPanel()
    {
        OpenPanel(2);
    }

    private void OpenPanel(int panel)
    {
        AudioManager.Instance.PlaySound("uibutton");
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] == panels[panel])
            {
                panels[i].SetActive(true);
            }
            else
            {
                panels[i].SetActive(false);
            }
        }
    }
}
