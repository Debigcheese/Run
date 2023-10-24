using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatScript : MonoBehaviour
{
    private DialogueManager dialogueManager;
    private WeaponHolder weaponHolder;
    public int count;

    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        weaponHolder = GetComponent<WeaponHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            count++;
            weaponHolder.secondWeapon = weaponHolder.weapons[count];

        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerPrefs.SetInt("EnableDash", 1);
            PlayerPrefs.Save();
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            PlayerPrefs.SetInt("ShowSecondWeaponUI",1 );
            PlayerPrefs.Save();
        }
    }
}
