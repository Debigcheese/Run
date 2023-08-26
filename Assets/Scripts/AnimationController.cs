using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    private Animator anim;
    private PlayerMovement playerMovement;
    private PunchAttack punchAttack;
    private float movement;
    private float jumpingLanding;
    private float punching;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        punchAttack = GetComponentInParent<PunchAttack>();
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
        if (punchAttack.isNormalPunching)
        {
            punching = 1f;
        }
        if (punchAttack.isLongPunching)
        {
            punching = 0f;
        }

        anim.SetFloat("jumpingLanding", jumpingLanding);
        anim.SetFloat("punching", punching);
        anim.SetFloat("movement", movement);

        anim.SetBool("isJumping", playerMovement.isJumping);
        anim.SetBool("isClimbingLedge", playerMovement.isClimbingLedge);
        anim.SetBool("isWallSliding", playerMovement.isWallSliding);

        anim.SetBool("isPunching", punchAttack.isPunching);

    }


    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }
}
