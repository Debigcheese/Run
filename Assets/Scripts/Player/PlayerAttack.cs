using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !playerMovement.isWallSliding && !playerMovement.isClimbingLedge && !isAttacking)
        {
            isAttacking = true;
        }
        if((playerMovement.isClimbingLedge || playerMovement.isWallSliding) && isAttacking)
        {
            isAttacking = true;
        }
    }

}
