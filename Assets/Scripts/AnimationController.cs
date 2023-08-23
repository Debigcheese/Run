using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    private Animator anim;
    private PlayerMovement playerMovement;
    private Collision coll;
    private float movement;
    private float jumpingLanding;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponentInParent<Collision>();
        playerMovement = GetComponentInParent<PlayerMovement>();
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
        anim.SetBool("canClimbLedge", playerMovement.canClimbLedge);
        anim.SetBool("isJumping", playerMovement.isJumping);
        anim.SetBool("isWallSliding", playerMovement.isWallSliding);
    }


    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }
}
