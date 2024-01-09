using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private PlayerState playerState;
    private float movement;
    private float jumpingLanding;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerAttack = GetComponentInParent<PlayerAttack>();
        playerState = GetComponentInParent<PlayerState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement.isMoving && !playerState.isRegeningHp)
        {
            if (playerState.guardianEnabled || playerMovement.isInWater)
            {
                movement = 2f;
            }
            else
            {
                movement = 1f;
            }
        }
        else
        {
            movement = 0f;
        }
        if (playerMovement.isJumping && !playerState.guardianEnabled)
        {
            jumpingLanding = 1f;
        }
        else if(playerMovement.isJumping && playerState.guardianEnabled)
        {
            jumpingLanding = 2f;
        }
        if (playerMovement.isFalling)
        {
            jumpingLanding = 0f;
        }

        anim.SetFloat("jumpingLanding", jumpingLanding);
        anim.SetFloat("movement", movement);
        anim.SetBool("isAttacking", playerAttack.isAttacking);
        anim.SetBool("isJumping", playerMovement.isJumping);
        anim.SetBool("isClimbingLedge", playerMovement.isClimbingLedge);
        anim.SetBool("isWallSliding", playerMovement.isWallSliding);
        anim.SetBool("isHurt", playerState.isHurt);
        
    }


    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }
}
