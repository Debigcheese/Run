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

    // Start is called before the first frame update
    void Start()
    {
        pickupWeapons = FindObjectsOfType<PickupWeapon>();
        playerMovement = GetComponentInParent<PlayerMovement>();

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
        
    }

}