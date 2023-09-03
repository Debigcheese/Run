using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PickupWeapon pickupWeapon;

    public Transform weaponHolder;
    public GameObject[] weapons;
    public GameObject currentWeapon;

    public Transform pickUpWeaponCheck;
    public bool circleRange = false;
    public LayerMask layer;
    public float radius;

    // Start is called before the first frame update
    void Start()
    {

        pickupWeapon = FindAnyObjectByType<PickupWeapon>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        
        for(int i = 0; i< weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }
        currentWeapon = weapons[0];
        currentWeapon.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        circleRange = Physics2D.OverlapCircle(pickUpWeaponCheck.position, radius, layer);

        if(Input.GetKeyDown(KeyCode.E) && circleRange)
        {
            PickupWeapon();
        }
    }

    public void PickupWeapon()
    {
        foreach(GameObject weapon in weapons)
        {
            if(weapon == pickupWeapon.weaponPrefab)
            {
                weapon.SetActive(true);
            }
            else
            {
                weapon.SetActive(false);
            }
        }
        currentWeapon = pickupWeapon.weaponPrefab;
        currentWeapon.SetActive(true);
        pickupWeapon.pickedUp = true;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    //Gizmos.DrawWireSphere(PickupPoint.position, PickupRange);
    //}

}
