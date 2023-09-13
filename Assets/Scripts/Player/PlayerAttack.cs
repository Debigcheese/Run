using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerHP playerHP;
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    public bool isAttacking = false;
    public float AttackMoveSpeed;
    public bool checkMousePositionX;
    public bool stopFlip = false;
    public bool canAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        playerHP = GetComponent<PlayerHP>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        if (Input.GetButtonDown("Fire1") && !playerMovement.isWallSliding && !playerMovement.isClimbingLedge && !isAttacking && canAttack)
        {
            isAttacking = true;
            if (mousePosition.x > transform.position.x && playerMovement.isFacingLeft || mousePosition.x < transform.position.x && !playerMovement.isFacingLeft)
            {
                stopFlip = true;
                playerMovement.Flip();
            }
        }
        //cant attack when ledgeclimb,wallslide,walljump,hurt
        if (playerMovement.isClimbingLedge || playerMovement.isWallSliding || playerMovement.isWallJumping || playerHP.isHurt)
        {
            isAttacking = false;
        }
        //if attacking and not in air, slow ms
        if (isAttacking && !playerMovement.isFalling)
        {
            rb.velocity = new Vector2(playerMovement.moveDirection * AttackMoveSpeed, rb.velocity.y);
        }
        //stop flip character when attacking
        if (!isAttacking)
        {
            stopFlip = false;
        }
        
    }

}
