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

        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerPrefs.SetInt("EnableDash", 1);
            playerMovement.Dash(new Vector2(1f, 1f));
            playerMovement.enableDashUponCollision = true;
            PlayerPrefs.Save();
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            PlayerPrefs.SetInt("ShowSecondWeaponUI",1 );
            PlayerPrefs.Save();
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            abilityCount2++;
            PlayerPrefs.SetInt("Ability", abilityCount2);
            PlayerPrefs.Save();

        }
    }
}
