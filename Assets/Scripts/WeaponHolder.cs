using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
  

    private PlayerMovement playerMovement;
    public Transform weaponHolder;
    public GameObject[] weapons;
    public GameObject currentWeapon;

    private PunchAttack punchAttack;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        punchAttack = GetComponentInParent<PunchAttack>();

        currentWeapon = weapons[0];
            
    }

    // Update is called once per frame
    void Update()
    {
        
       
    }

    //public void Attack()
    //{
    //    if (currentWeapon == weapons[0])
    //    {
    //        punchAttack.Attack();
    //    }
    //    if (currentWeapon == weapons[1])
    //    {
    //        swordWeapon.Attack();
    //    }
    //}

    //private void OnDrawGizmosSelected()
    //{
    //    //Gizmos.DrawWireSphere(PickupPoint.position, PickupRange);
    //}

}
