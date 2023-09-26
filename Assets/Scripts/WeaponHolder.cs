using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    // References
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    public GameObject[] weapons;
    public GameObject currentWeapon;
    public GameObject secondWeapon;
    private GameObject previousWeapon;


    public bool isSwappingWeapons;
    public bool meleeEquipped;
    public bool magicEquipped;
    public bool justSwitchedWeapon;
    public bool canSwitchWeapons = true;

    // Start is called before the first frame update
    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        previousWeapon = currentWeapon;

        for (int i = 0; i < weapons.Length; i++)
        {
            
            if(weapons[i].gameObject == null || weapons[i] == null)
            {
                weapons[i] = weapons[0].gameObject;
            }
            weapons[i].SetActive(false);
        }
        currentWeapon = weapons[0];
        secondWeapon = weapons[1];
        currentWeapon.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isSwappingWeapons)
        {
            isSwappingWeapons = true;
            StartCoroutine(SwitchWeapon());
        }

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
        if (currentWeapon == weapons[10] || currentWeapon == weapons[11])
        {
            meleeEquipped = false;
            magicEquipped = true;
        }
        else
        {
            meleeEquipped = true;
            magicEquipped = false;
        }
    }

    public IEnumerator SwitchWeapon()
    {
        yield return new WaitForSeconds(.1f);
        playerAttack.isAttacking = false;
        playerAttack.canAttack = true;

        GameObject temp = currentWeapon;
        currentWeapon.SetActive(false);
        currentWeapon = secondWeapon;
        currentWeapon.SetActive(true);
        secondWeapon = temp;

        isSwappingWeapons = false;
    }

}