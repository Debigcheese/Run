using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    // References
    private PlayerMovement playerMovement;
    private PickupWeapon[] pickupWeapons;

    public GameObject[] weapons;
    public GameObject currentWeapon;
    private GameObject previousWeapon;
    public bool justSwitchedWeapon;

    public bool meleeEquipped;
    public bool magicEquipped;

    // Start is called before the first frame update
    void Start()
    {
        pickupWeapons = FindObjectsOfType<PickupWeapon>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        previousWeapon = currentWeapon;

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }
        currentWeapon = weapons[0];
        currentWeapon.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentWeapon == null)
        {
            currentWeapon = weapons[0];
        }
        if(currentWeapon != previousWeapon)
        {
            justSwitchedWeapon = true;
            previousWeapon = currentWeapon;
        }
        else
        {
            justSwitchedWeapon = false;
        }

        //check which weapon is equipped;
        if (currentWeapon == weapons[0] || currentWeapon == weapons[1] || currentWeapon == weapons[2] || currentWeapon == weapons[3] || currentWeapon == weapons[4])
        {
            meleeEquipped = true;
            magicEquipped = false;
        }
        else
        {
            meleeEquipped = false;
            magicEquipped = true;
        }

    }

}