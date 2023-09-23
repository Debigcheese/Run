using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    private Animator anim;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private PlayerHP playerHp;
    private float movement;
    private float jumpingLanding;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerAttack = GetComponentInParent<PlayerAttack>();
        playerHp = GetComponentInParent<PlayerHP>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement.isMoving)
        {
            movement = 1f;
        }
        else
        {
            movement = 0f;
        }
        if (playerMovement.isJumping)
        {
            jumpingLanding = 1f;
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
        anim.SetBool("isHurt", playerHp.isHurt);
        
    }


    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }
}
