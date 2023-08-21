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
    public float maxWallClimbTime;
    public float wallGrabCooldown;
    [SerializeField] public Vector2 wallJumpingPower = new Vector2(8f, 16f);

    [Space]
    [Header("Booleans")]
    public bool isWallGrabbing;
    public bool isJumping;
    public bool isWallJumping;

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
        isJumping = coll.onGround && Input.GetButtonDown("Jump");

        if (coll.onGround)
        {
            isWallJumping = false;
        }
        
        if (isJumping)
        {
            Jump(Vector2.up);
        }
        Vector2 dir = new Vector2(moveDirection, y);
        Walk(dir);
        WallGrab();
        WallSlide();
        WallJump();
        GetComponent<BetterJump>().enabled = true;
     }

    private void WallJump()
    {
        
        if (coll.onWall && !coll.onGround && Input.GetButtonDown("Jump"))
        {
            Jump(new Vector2(10,1));
            isWallJumping = true;
            Flip();
        }
    }

    private void WallGrab()
    {
        isWallGrabbing = coll.onWall && Input.GetKey(KeyCode.LeftShift);
        if (isWallGrabbing && !isWallGrabCooldown)
        {
            wallClimbTime += Time.deltaTime;
            rb.gravityScale = 0;
            float speedModifier = y > 0 ? .5f : 1;
            rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
        }
        
        if (wallClimbTime >= maxWallClimbTime || isWallGrabbing == false)
        {
            rb.gravityScale = 4;
            isWallGrabCooldown = true;
            StartCoroutine(WallGrabCooldown());
        }
    }

    private IEnumerator WallGrabCooldown()
    {
        yield return new WaitForSeconds(wallGrabCooldown);
        isWallGrabCooldown = false;
        wallClimbTime = 0f;
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

    private void Jump(Vector2 dir)
    {
        isJumping = true;
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
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
    }

}
