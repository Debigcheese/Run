using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityManagerMenu : MonoBehaviour
{
    public int totalCrystalAmount;
    public int[] crystalCost;
    public int[] abilityIndex;

    public List<int> ownedAbilities = new List<int>();
    private string ownedAbilitiesString;
    public int equippedAbility;

    public Color originalColor;
    public Color boughtColor;
    public GameObject[] itemPanels;

    public GameObject[] AbilitiesUI;
    public GameObject[] buyButton;
    public GameObject[] cantBuyButton;
    public GameObject[] equipButton;
    public GameObject[] buyText;
    public GameObject[] equipText;
    public GameObject[] equippedText;
    public TextMeshProUGUI[] costText;

    // Start is called before the first frame update
    void Start()
    {
        
        equippedAbility = PlayerPrefs.GetInt("Ability", 0);
        ownedAbilitiesString = PlayerPrefs.GetString("OwnedAbilities", "0");
        string[] indexStrings = ownedAbilitiesString.Split(",");
        ownedAbilities = new List<int>();

        foreach (string indexString in indexStrings)
        {
            int index;
            if (int.TryParse(indexString, out index))
            {
                ownedAbilities.Add(index);
            }
        }

        for (int i = 0; i < crystalCost.Length; i++)
        {
            costText[i].text = crystalCost[i].ToString();
        }

        UpdateButtonStates();
    }

    // Update is called once per frame
    void Update()
    {
        totalCrystalAmount = PlayerPrefs.GetInt("TotalCrystal", 0);
        UpdateButtonStates();
        ChangePanelColor();

    }

    private void ChangePanelColor()
    {
        for (int i = 0; i < itemPanels.Length; i++)
        {
            if (ownedAbilities.Contains(abilityIndex[i]))
            {
                itemPanels[i].GetComponent<Image>().color = boughtColor;
            }
            else
            {
                itemPanels[i].GetComponent<Image>().color = originalColor;
            }
        }
    }

    private void UpdateButtonStates()
    {
        for (int i = 0; i < abilityIndex.Length; i++)
        {
            bool isOwned = ownedAbilities.Contains(abilityIndex[i]);
            bool isEquipped = equippedAbility == abilityIndex[i];
            bool canAfford = totalCrystalAmount >= crystalCost[i];
            bool previousOwned = i == 0 || ownedAbilities.Contains(abilityIndex[i - 1]);

            buyButton[i].SetActive(canAfford && !isOwned);
            cantBuyButton[i].SetActive(!canAfford && !isOwned || !previousOwned);
            equipButton[i].SetActive(isOwned);
            buyText[i].SetActive(!isOwned);
            equipText[i].SetActive(isOwned && !isEquipped);
            equippedText[i].SetActive(isEquipped);
        }
    }

    public void EquipAbility(int btnIndex)
    {
        for (int i = 0; i < equipButton.Length; i++)
        {
            if (ownedAbilities.Contains(btnIndex))
            {
                equippedAbility = btnIndex;
                PlayerPrefs.SetInt("Ability", equippedAbility);
                PlayerPrefs.Save();

                UpdateButtonStates();
            }
            if (equippedAbility == abilityIndex[i])
            {
                AbilitiesUI[equippedAbility].SetActive(true);
            }
            else
            {
                AbilitiesUI[i].SetActive(false);
            }
        }
       
    }

    public void BuyButton(int btnIndex)
    {
        for (int i = 0; i < buyButton.Length; i++)
        {
            if(totalCrystalAmount >= crystalCost[btnIndex])
            {
                if (ownedAbilities == null)
                {
                    ownedAbilities = new List<int>();
                }
                if (!ownedAbilities.Contains(btnIndex))
                {

                    int abilityCost = crystalCost[btnIndex];
                    PlayerPrefs.SetInt("TotalCrystal", totalCrystalAmount - abilityCost);

                    ownedAbilities.Add(btnIndex);

                    ownedAbilitiesString = string.Join(",", ownedAbilities);
                    PlayerPrefs.SetString("OwnedAbilities", ownedAbilitiesString);
                    PlayerPrefs.Save();

                    UpdateButtonStates();
                }

            }
        }
    }
}
