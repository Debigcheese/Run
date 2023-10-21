using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public GameObject[] panels;
    public TextMeshProUGUI crystalText;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        panels[0].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        crystalText.text = PlayerPrefs.GetInt("TotalCrystal", 0).ToString();
    }

    public void LeaveShop()
    {
        FindObjectOfType<TransitionScript>().TransitionStart();
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
