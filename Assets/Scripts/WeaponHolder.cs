using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    PlayerMovement playerMovement;

    public Transform weaponHolder;
    public GameObject[] weapons;
    public GameObject currentWeapon;

    private PunchAttack punchAttack;

    // Start is called before the first frame update
    void Start()
    {
        punchAttack = GetComponentInParent<PunchAttack>();
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !playerMovement.isWallSliding && !playerMovement.isClimbingLedge)
        {
            Attack();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            
        }
        if (currentWeapon == null)
        {
            currentWeapon = Instantiate(weapons[0], weaponHolder);
            punchAttack = currentWeapon.GetComponent<PunchAttack>();
        }
    }

    public void Attack()
    {
        if(currentWeapon == weapons[0])
        {
           punchAttack.Punch();
        }
    }

    public void EquipWeapon(GameObject newWeapon)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }
        currentWeapon = newWeapon;
    }

}
