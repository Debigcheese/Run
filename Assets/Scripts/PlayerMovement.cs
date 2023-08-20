using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collision coll;
    private bool isFacingRight;
    private float moveDirection = 0f;
    private float y = 0f;

    [Header("Stats")]
    public float speed = 10;
    public float JumpForce = 50;
    public float slideSpeed = 5;
    public float maxWallClimbTime = 2f;
    public float wallGrabCooldown = 2f;

    [Space]
    [Header("Booleans")]
    public bool wallGrab;
    public bool isJumping;
    private bool canWallGrab = true;

    [Space]
    private float wallClimbTime = 0f;
    private bool isWallGrabCooldown;

    void Start()
     {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
    }

     void Update()
     {
        moveDirection = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        Vector2 dir = new Vector2(moveDirection, y);

        Walk(dir);

        wallGrab = coll.onWall && Input.GetKey(KeyCode.LeftShift);

        if (Input.GetButtonDown("Jump"))
        {
            if (coll.onGround)
            {
                isJumping = true;
                Jump(Vector2.up, false);
            }
            if(coll.onWall && !coll.onGround)
            {
                WallJump();
            }
          
        }
 
        WallGrab();
        WallSlide();
        GetComponent<BetterJump>().enabled = true;

     }
    private void WallJump()
    {

    }

    private void WallGrab()
    {
        if (wallGrab && wallClimbTime < maxWallClimbTime && !isWallGrabCooldown && canWallGrab)
        {
            wallClimbTime += Time.deltaTime;
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            float speedModifier = y > 0 ? .5f : 1;
            rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
            canWallGrab = false;
        }
        
        if (wallClimbTime >= maxWallClimbTime || wallGrab == false)
        {
            wallClimbTime = 0f;
            canWallGrab = false;
            rb.gravityScale = 4;
            StartCoroutine(WallGrabCooldown());
        }
    }

    private IEnumerator WallGrabCooldown()
    {
        yield return new WaitForSeconds(wallGrabCooldown);
        isWallGrabCooldown = false;
        wallClimbTime = 0f;
        wallGrab = true;
        rb.gravityScale = 4;
        canWallGrab = true;
    }

    private void Walk(Vector2 dir)
    {

        rb.velocity = (new Vector2(dir.x * speed, rb.velocity.y));
        if (moveDirection > 0f && isFacingRight)
        {
            Flip();
        }
        if (moveDirection < 0f && isFacingRight == false)
        {
            Flip();
        }

    }

    private void Jump(Vector2 dir, bool wall)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * JumpForce;
    }

    private void WallSlide()
    {
        if (coll.onWall && !coll.onGround && rb.velocity.y <0)
        {
            rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
        }

    }

    private void Flip()
    {

        if(moveDirection < 0 || moveDirection > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;

        }
    }

}
