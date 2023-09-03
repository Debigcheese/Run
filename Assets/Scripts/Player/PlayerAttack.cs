using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    public bool isAttacking = false;
    public float AttackMoveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && !playerMovement.isWallSliding && !playerMovement.isClimbingLedge && !isAttacking)
        {
            isAttacking = true;
        }
        if ((playerMovement.isClimbingLedge || playerMovement.isWallSliding) && isAttacking)
        {
            isAttacking = true;
        }
        if (isAttacking && !playerMovement.isJumping)
        {
            playerMovement.cantMove = true;
            rb.velocity = new Vector2(playerMovement.moveDirection * AttackMoveSpeed, rb.velocity.y);
        }
        else
        {
            playerMovement.cantMove = false;
        }
    }

}
