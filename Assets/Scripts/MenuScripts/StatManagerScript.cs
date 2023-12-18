using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatManagerScript : MonoBehaviour
{

    public int statSelected = 0;
    public int totalCrystalAmount;

    [Space]
    [Header("Upgrade")]
    public int currenthealthLevel = 0;
    public int currentregenLevel = 0;
    public int currentcritLevel = 0;
    public int[] UpgradeCost;

    public Image[] hpBar;
    public Image[] regenBar;
    public Image[] critBar;

    [Space]
    [Header("Popup")]
    public GameObject popUpWindow;
    public GameObject[] BuyButton;
    public GameObject[] cantBuyButton;
    public TextMeshProUGUI statDescription;
    public TextMeshProUGUI statCostText;

    [Space]
    [Header("StatsText")]
    public TextMeshProUGUI[] StatsDisplayText;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < BuyButton.Length; i++)
        {
            BuyButton[i].SetActive(false);
            cantBuyButton[i].SetActive(false);
        }

        for (int i = 0; i < hpBar.Length; i++)
        {
            hpBar[i].enabled = false;
            regenBar[i].enabled = false;
            critBar[i].enabled = false;
        }

        currenthealthLevel = PlayerPrefs.GetInt("CurrentHealthLevel", 0);
        currentregenLevel = PlayerPrefs.GetInt("CurrentRegenLevel", 0);
        currentcritLevel = PlayerPrefs.GetInt("CurrentCritLevel", 0);

        popUpWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        StatsDisplayText[0].text = "HealthPoints: " + PlayerPrefs.GetInt("TotalHealth", 37).ToString();
        StatsDisplayText[3].text = "Crit chance: " + PlayerPrefs.GetFloat("CritChance", 4).ToString() + "%";

        for (int i = 0; i< StatsDisplayText.Length; i++)
        {
            int manaRegen = 25;
            int staminaRegen = 25;
            StatsDisplayText[1].text = "Mana regen: ";
            StatsDisplayText[2].text = "Stamina regen: ";

            if (currentregenLevel == 0)
            {
                StatsDisplayText[1].text += manaRegen + "%";
                StatsDisplayText[2].text += staminaRegen + "%";
            }
            if (currentregenLevel == 1)
            {
                StatsDisplayText[1].text += manaRegen + 16 + "%";
                StatsDisplayText[2].text += staminaRegen + 16 + "%";
            }
            if (currentregenLevel == 2)
            {
                StatsDisplayText[1].text += manaRegen + 32 + "%";
                StatsDisplayText[2].text += staminaRegen + 32 + "%";
            }
            if (currentregenLevel == 3)
            {
                StatsDisplayText[1].text += manaRegen + 48 + "%";
                StatsDisplayText[2].text += staminaRegen + 48 + "%";
            }
            if (currentregenLevel == 4)
            {
                StatsDisplayText[1].text += manaRegen + 64 + "%";
                StatsDisplayText[2].text += staminaRegen + 64 + "%";
            }
            if (currentregenLevel == 5)
            {
                StatsDisplayText[1].text += manaRegen + 80 + "%";
                StatsDisplayText[2].text += staminaRegen + 80 + "%";
            }
        }

        for (int i = 0; i < StatsDisplayText.Length; i++)
        {
            int critDamage = 100;
            StatsDisplayText[4].text = "Crit damage: ";


            if (currentcritLevel == 0)
            {
                StatsDisplayText[4].text += critDamage + "%";
            }
            if (currentcritLevel == 1)
            {
                StatsDisplayText[4].text += critDamage + 16 + "%";
            }
            if (currentcritLevel == 2)
            {
                StatsDisplayText[4].text += critDamage + 32 + "%";
            }
            if (currentcritLevel == 3)
            {
                StatsDisplayText[4].text += critDamage + 48 + "%";
            }
            if (currentcritLevel == 4)
            {
                StatsDisplayText[4].text += critDamage + 64 + "%";
            }
            if (currentcritLevel == 5)
            {
                StatsDisplayText[4].text += critDamage + 80 + "%";
            }

        }

        totalCrystalAmount = PlayerPrefs.GetInt("TotalCrystal", 0);

        for (int i = 0; i< hpBar.Length; i++)
        {
            hpBar[currenthealthLevel].enabled = true;
            regenBar[currentregenLevel].enabled = true;
            critBar[currentcritLevel].enabled = true;
        }

        if(statSelected == 0)
        {
            statCostText.text = UpgradeCost[currenthealthLevel].ToString();
        }
        if (statSelected == 1)
        {
            statCostText.text = UpgradeCost[currentregenLevel].ToString();
        }
        if (statSelected == 2)
        {
            statCostText.text = UpgradeCost[currentcritLevel].ToString();
        }
        PressStatButton();
    }

    private void TurnOffButtons(int btnIndex, int currentLevel)
    {
        for (int i = 0; i < BuyButton.Length; i++)
        {
            if(btnIndex != i)
            {
                BuyButton[i].SetActive(false);
                cantBuyButton[i].SetActive(false);
            }
            if (btnIndex == statSelected)
            {
                if (totalCrystalAmount >= UpgradeCost[currentLevel] && currentLevel != 5)
                {

                    BuyButton[statSelected].SetActive(true);
                    cantBuyButton[statSelected].SetActive(false);
                }
                else
                {
                    BuyButton[statSelected].SetActive(false);
                    cantBuyButton[statSelected].SetActive(true);
                }
            }
        }
    }

    private void PressStatButton()
    {
        if (statSelected == 0)
        {
            statDescription.text = "Increases max health by +50%";
            TurnOffButtons(0, currenthealthLevel);
        }

        if (statSelected == 1)

        {
            statDescription.text = "Increases stamina and mana regeneration by +16%";
            TurnOffButtons(1, currentregenLevel);
        }

        if (statSelected == 2)
        {
            statDescription.text = "increases crit chance by +3% and crit damage by +16%";
            TurnOffButtons(2, currentcritLevel);
        }
    }

    public void PressHealthButton()
    {
        statSelected = 0;
        popUpWindow.SetActive(true);
        AudioManager.Instance.PlaySound("uibutton");
    }

    public void PressRegenButton()
    {
        statSelected = 1;
        popUpWindow.SetActive(true);
        AudioManager.Instance.PlaySound("uibutton");
    }

    public void PressCritButton()
    {
        statSelected = 2;
        popUpWindow.SetActive(true);
        AudioManager.Instance.PlaySound("uibutton");
    }

    public void UpgradeHealth()
    {
        if (totalCrystalAmount >= UpgradeCost[currenthealthLevel] && currenthealthLevel != 5)
        {
            int totalHealth = PlayerPrefs.GetInt("TotalHealth", 37);
            float newTotalHealth = totalHealth * 1.5f;
            int roundedTotalHealth = Mathf.RoundToInt(newTotalHealth);
            PlayerPrefs.SetInt("TotalHealth", roundedTotalHealth);

            PlayerPrefs.SetInt("TotalCrystal", totalCrystalAmount - UpgradeCost[currenthealthLevel]);

            currenthealthLevel++;
            PlayerPrefs.SetInt("CurrentHealthLevel", currenthealthLevel);
            PlayerPrefs.Save();

            AudioManager.Instance.PlaySound("uibuttonbuy");
        }
        else
        {
            AudioManager.Instance.PlaySound("uibuttonwrong");
        }

    }

    public void UpgradeRegen()
    {
        if (totalCrystalAmount >= UpgradeCost[currentregenLevel] && currentregenLevel != 5)
        {

            float manaRegen = PlayerPrefs.GetFloat("ManaRegen", 0.07f);
            float newManaRegen = manaRegen / 1.16f;
            PlayerPrefs.SetFloat("ManaRegen", newManaRegen);

            float staminaRegen = PlayerPrefs.GetFloat("StaminaRegen", 0.07f);
            float newStaminaRegen = staminaRegen / 1.16f;
            PlayerPrefs.SetFloat("StaminaRegen", newStaminaRegen);

            PlayerPrefs.SetInt("TotalCrystal", totalCrystalAmount - UpgradeCost[currentregenLevel]);

            currentregenLevel++;
            PlayerPrefs.SetInt("CurrentRegenLevel", currentregenLevel);
            PlayerPrefs.Save();

            AudioManager.Instance.PlaySound("uibuttonbuy");
        }
        else
        {
            AudioManager.Instance.PlaySound("uibuttonwrong");
        }
    }

    public void UpgradeCrit()
    {
        if (totalCrystalAmount >= UpgradeCost[currentcritLevel] && currentcritLevel != 5)
        {
            float CritChance = PlayerPrefs.GetFloat("CritChance", 4);
            float newCritChance = CritChance + 3f;
            PlayerPrefs.SetFloat("CritChance", newCritChance);

            float critDamage = PlayerPrefs.GetFloat("CritDamage", 2);
            float newCritDamage = critDamage + .16f;
            PlayerPrefs.SetFloat("CritDamage", newCritDamage);

            PlayerPrefs.SetInt("TotalCrystal", totalCrystalAmount - UpgradeCost[currentcritLevel]);

            currentcritLevel++;
            PlayerPrefs.SetInt("CurrentCritLevel", currentcritLevel);
            PlayerPrefs.Save();
            AudioManager.Instance.PlaySound("uibuttonbuy");
        }
        else
        {
            AudioManager.Instance.PlaySound("uibuttonwrong");
        }
    }

    public void ReturnButton()
    {
        AudioManager.Instance.PlaySound("uibutton");
        popUpWindow.SetActive(false);
        BuyButton[statSelected].SetActive(false);
    }
}
