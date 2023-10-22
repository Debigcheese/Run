using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    public int totalCrystalAmount;
    public int pageNumber;
    public int pagesUnlocked = 0;
    public int buyButtonIndex;

    public List<int> ownedWeapons = new List<int>();
    private string ownedWeaponString;
    public Page[] pages;

    //buttons
    public GameObject[] buyButton;
    public GameObject[] cantBuyButton;
    public GameObject[] EquipButton;
    public GameObject[] EquipAsSecondary;
    public GameObject nextPageButton;
    public GameObject previousPageButton;

    //text
    public TextMeshProUGUI[] buyText;
    public TextMeshProUGUI[] equipText;
    public TextMeshProUGUI[] equippedText;
    public TextMeshProUGUI[] crystalCostText;

    //images
    public GameObject[] currentWeaponImages;
    public GameObject[] secondWeaponImages;

    // Start is called before the first frame update
    void Start()
    {

        ownedWeaponString = PlayerPrefs.GetString("OwnedWeapons", "0,1");
        string[] indexStrings = ownedWeaponString.Split(",");
        ownedWeapons = new List<int>();

        foreach(string indexString in indexStrings)
        {
            int index;
            if(int.TryParse(indexString, out index))
            {
                ownedWeapons.Add(index);
            }
        }

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
        if(pageNumber == 0)
        {
            previousPageButton.SetActive(false);
        }
        else
        {
            previousPageButton.SetActive(true);
        }
        if(pageNumber == pages.Length-1)
        {
            nextPageButton.SetActive(false);
        }
        else
        {
            nextPageButton.SetActive(true);
        }

        for (int pageIndex = 0; pageIndex < pages.Length; pageIndex++)
        {
            int[] weaponIndexes = pages[pageIndex].weaponIndex;
            int count = 0;

            for (int i = 0; i < weaponIndexes.Length; i++)
            {
                int nextIndex = (i + 1) % weaponIndexes.Length;
                int afterNextIndex = (i + 2) % weaponIndexes.Length;

                if (ownedWeapons.Contains(weaponIndexes[i]) && ownedWeapons.Contains(weaponIndexes[nextIndex]) ||
                    ownedWeapons.Contains(weaponIndexes[nextIndex]) && ownedWeapons.Contains(weaponIndexes[afterNextIndex]) ||
                    ownedWeapons.Contains(weaponIndexes[afterNextIndex]) && ownedWeapons.Contains(weaponIndexes[i]))
                {
                    count++;
                    pagesUnlocked = pageIndex + count;
                    break;
                }
            }
        }


        int currentWeaponIndex = PlayerPrefs.GetInt("CurrentWeapon");
        int secondWeaponIndex = PlayerPrefs.GetInt("SecondWeapon");

        for (int i = 0; i < currentWeaponImages.Length; i++)
        {
            if(currentWeaponIndex == i)
            {
                currentWeaponImages[i].SetActive(true);
            }
            else
            {
                currentWeaponImages[i].SetActive(false);
            }
            if (secondWeaponIndex == i)
            {
                secondWeaponImages[i].SetActive(true);
            }
            else
            {
                secondWeaponImages[i].SetActive(false);
            }
        }

        pageNumber = PlayerPrefs.GetInt("CurrentPage", 0);
        totalCrystalAmount = PlayerPrefs.GetInt("TotalCrystal", 0);

        for (int i = 0; i < buyButton.Length; i++)
        {

            crystalCostText[i].text = pages[pageNumber].weaponCost[i].ToString();

            if (totalCrystalAmount >= pages[pageNumber].weaponCost[i] && !ownedWeapons.Contains(pages[pageNumber].weaponIndex[i]))
            {
                buyButton[i].SetActive(true);
                cantBuyButton[i].SetActive(false);
                EquipButton[i].SetActive(false);
                EquipAsSecondary[i].SetActive(false);
                buyText[i].enabled = true;
                equipText[i].enabled = false;
                equippedText[i].enabled = false;
            }
            else if (ownedWeapons.Contains(pages[pageNumber].weaponIndex[i]) )
            {
                if (currentWeaponIndex == pages[pageNumber].weaponIndex[i])
                {
                    buyButton[i].SetActive(false);
                    cantBuyButton[i].SetActive(false);
                    EquipButton[i].SetActive(false);
                    EquipAsSecondary[i].SetActive(true);
                    buyText[i].enabled = false;
                    equipText[i].enabled = false;
                    equippedText[i].enabled = true;
                }
                else if(secondWeaponIndex == pages[pageNumber].weaponIndex[i])
                {
                    buyButton[i].SetActive(false);
                    cantBuyButton[i].SetActive(false);
                    EquipButton[i].SetActive(false);
                    EquipAsSecondary[i].SetActive(true);
                    buyText[i].enabled = false;
                    equipText[i].enabled = true;
                    equippedText[i].enabled = false;
                }
                else
                {
                    buyButton[i].SetActive(false);
                    cantBuyButton[i].SetActive(false);
                    EquipButton[i].SetActive(true);
                    EquipAsSecondary[i].SetActive(false);
                    buyText[i].enabled = false;
                    equipText[i].enabled = true;
                    equippedText[i].enabled = false;
                }
            }
            else
            {
                buyButton[i].SetActive(false);
                EquipButton[i].SetActive(false);
                cantBuyButton[i].SetActive(true);
                EquipAsSecondary[i].SetActive(false);
                buyText[i].enabled = true;
                equipText[i].enabled = false;
                equippedText[i].enabled = false;
            }
        }
    }

    public void EquipWeapon(int btnIndex)
    {
        for (int i = 0; i < buyButton.Length; i++)
        {
            int weaponIndex = pages[pageNumber].weaponIndex[btnIndex];
            PlayerPrefs.SetInt("CurrentWeapon", weaponIndex);
            PlayerPrefs.Save();
        }
    }

    public void SwapSecondary(int btnIndex)
    {
        int secondWeaponIndex = PlayerPrefs.GetInt("SecondWeapon");
        PlayerPrefs.SetInt("SecondWeapon", PlayerPrefs.GetInt("CurrentWeapon"));
        PlayerPrefs.SetInt("CurrentWeapon", secondWeaponIndex);
        PlayerPrefs.Save();
    }

    public void BuyWeapon(int btnIndex)
    {
        if(totalCrystalAmount >= pages[pageNumber].weaponCost[btnIndex])
        {
            if(ownedWeapons == null)
            {
                ownedWeapons = new List<int>();
            }
            if (!ownedWeapons.Contains(pages[pageNumber].weaponIndex[btnIndex]))
            {
                int weaponCost = pages[pageNumber].weaponCost[btnIndex];
                PlayerPrefs.SetInt("TotalCrystal", totalCrystalAmount - weaponCost);

                ownedWeapons.Add(pages[pageNumber].weaponIndex[btnIndex]);

                ownedWeaponString = string.Join(",", ownedWeapons);
                PlayerPrefs.SetString("OwnedWeapons", ownedWeaponString);
                PlayerPrefs.Save();

                Debug.Log(PlayerPrefs.GetString("OwnedWeapons"));
            }
        }
    }

    public void NextPage()
    {
        if(pageNumber < pages.Length-1 && pageNumber < pagesUnlocked )
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
    public int[] weaponIndex;
    public int[] weaponCost;

}
