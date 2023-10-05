using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickupWeapon : MonoBehaviour
{
    private PlayerAttack playerAttack;
    private WeaponHolder weaponholder;
    public int identifier;
    public bool inRange = false;
    public GameObject EKeyCap;

    // Start is called before the first frame update
    void Start()
    {
        playerAttack = FindAnyObjectByType<PlayerAttack>();
        weaponholder = FindAnyObjectByType<WeaponHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && inRange && !playerAttack.isAttacking)
        {
            weaponholder.currentWeapon.SetActive(false);
            weaponholder.currentWeapon = weaponholder.weapons[identifier];
            weaponholder.currentWeapon.SetActive(true);
            Destroy(gameObject);
        }

        if (inRange)
        {
            EKeyCap.SetActive(true);
        }
        else
        {
            EKeyCap.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = false;
        }
    }




}
