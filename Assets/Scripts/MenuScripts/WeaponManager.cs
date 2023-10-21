using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    public int totalCrystalAmount;
    public int pageNumber;
    public int buyButtonIndex;
    public Page[] pages;

    public GameObject[] buyButton;
    public GameObject[] cantBuyButton;
    public TextMeshProUGUI[] crystalCost;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < buyButton.Length; i++)
        {
            buyButton[i].SetActive(false);
            cantBuyButton[i].SetActive(false);
        }


        totalCrystalAmount = PlayerPrefs.GetInt("TotalCrystal", 0);
        pageNumber = PlayerPrefs.GetInt("CurrentPage", 0);
        PlayerPrefs.Save();

        for (int i = 0; i < pages.Length; i++)
        {
            foreach (GameObject itemPanel in pages[i].WeaponTierPanels)
            {
                itemPanel.SetActive(false);

                if (pages[pageNumber] == pages[i])
                {
                    itemPanel.SetActive(true);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        for (int i = 0; i < crystalCost.Length; i++)
        {
            if (pages[pageNumber] == pages[i])
            {
                crystalCost[0].text = pages[pageNumber].weaponCost[0].ToString();
                crystalCost[1].text = pages[pageNumber].weaponCost[1].ToString();
                crystalCost[2].text = pages[pageNumber].weaponCost[2].ToString();

                if(totalCrystalAmount >= pages[pageNumber].weaponCost[0])
                {
                    buyButton[0].SetActive(true);
                    cantBuyButton[0].SetActive(false);
                }
                else
                {
                    buyButton[0].SetActive(false);
                    cantBuyButton[0].SetActive(true);
                }

                if (totalCrystalAmount >= pages[pageNumber].weaponCost[1])
                {
                    buyButton[1].SetActive(true);
                    cantBuyButton[1].SetActive(false);
                }
                else
                {
                    buyButton[1].SetActive(false);
                    cantBuyButton[1].SetActive(true);
                }

                if (totalCrystalAmount >= pages[pageNumber].weaponCost[2])
                {
                    buyButton[2].SetActive(true);
                    cantBuyButton[2].SetActive(false);
                }
                else
                {
                    buyButton[2].SetActive(false);
                    cantBuyButton[2].SetActive(true);
                }
            }
        }

    }

    public void BuyButtonPressed1()
    {
        for(int i = 0; i < pages.Length; i++)
        {
            if(pages[pageNumber] == pages[i])
            {
                


            }
        }
    }

    public void BuyButtonPressed2()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            buyButtonIndex = 5 + pageNumber;
        }
    }

    public void BuyButtonPressed3()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            buyButtonIndex = 10 + pageNumber;
        }
    }

    public void NextPage()
    {
        if(pageNumber != 5)
        {
            pageNumber++;
            SwitchPage(pageNumber);
            PlayerPrefs.SetInt("CurrentPage", pageNumber);
            PlayerPrefs.Save();
        }
    }

    public void PreviousPage()
    {
        if(pageNumber != 0)
        {
            pageNumber--;
            SwitchPage(pageNumber);
            PlayerPrefs.SetInt("CurrentPage", pageNumber);
            PlayerPrefs.Save();
        }
    }

    private void SwitchPage(int pageNumber)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            foreach (GameObject itemPanel in pages[i].WeaponTierPanels)
            {
                itemPanel.SetActive(false);

                if (pages[pageNumber] == pages[i])
                {
                    itemPanel.SetActive(true);
                }
            }
        }
    }

}

[System.Serializable]
public class Page
{
    public GameObject[] WeaponTierPanels;

    public int[] weaponCost;
}
