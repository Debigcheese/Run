using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    public bool isAttacking = false;
    public float AttackMoveSpeed;
    public bool checkMousePositionX;
    public bool stopFlip = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        if (Input.GetButtonDown("Fire1") && !playerMovement.isWallSliding && !playerMovement.isClimbingLedge && !isAttacking)
        {
            isAttacking = true;
            if (mousePosition.x > transform.position.x && playerMovement.isFacingLeft || mousePosition.x < transform.position.x && !playerMovement.isFacingLeft)
            {
                stopFlip = true;
                playerMovement.Flip();
            }
        }
        //cant ledgeclimb or wallslide when attacking
        if ((playerMovement.isClimbingLedge || playerMovement.isWallSliding) && isAttacking)
        {
            isAttacking = true;
        }
        //if attacking and not jumping, slow ms
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
