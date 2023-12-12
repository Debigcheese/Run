using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatScript : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private DialogueManager dialogueManager;
    private WeaponHolder weaponHolder;
    public int weaponCount;
    public int abilityCount2 = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        weaponHolder = GetComponent<WeaponHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            weaponCount++;
            weaponHolder.secondWeapon = weaponHolder.weapons[weaponCount];
            if(weaponCount == 16) 
            {
                weaponCount = 0;
            }

        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerPrefs.SetInt("ShowSecondWeaponUI", 1);
            GetComponent<WeaponHolder>().showSecondWeaponUI.SetActive(true);
            PlayerPrefs.Save();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            PlayerPrefs.SetInt("EnableDash", 1);
            PlayerPrefs.SetInt("Ability", 0);
            playerMovement.Dash(new Vector2(1f, 1f));
            playerMovement.enableDashUponCollision = true;
            PlayerPrefs.Save();
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            PlayerPrefs.SetInt("Ability", abilityCount2);
            abilityCount2++;
            if(abilityCount2 == 2)
            {
                abilityCount2 = 0;
            }
            PlayerPrefs.Save();

        }
    }
}
