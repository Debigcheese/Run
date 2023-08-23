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
    private float originalSpeed;

    [Header("Stats")]
    public float speed = 10;
    public float JumpForce = 50;
    public float slideSpeed = 5;
    public float dashSpeed = 5f;

    [Space]
    [Header("Booleans")]
    public bool isJumping;
    public bool isWallGrabbing;
    public bool isWallJumping;
    public bool isDashing;


    [Space]
    [Header("WallGrabbing")]
    public float maxWallClimbTime;
    public float wallGrabCooldown;
    private float wallClimbTime = 0f;
    private bool isWallGrabCooldown;


    [Space]
    [Header("WallJumping")]
    [SerializeField] public Vector2 wallJumpingPower = new Vector2(1f, 1f);
    private float wallJumpingDirection;
    public float wallJumpingSlowMo;

    [Space]
    [Header("Dash")]
    public bool hasDashed;

    void Start()
    {
        originalSpeed = speed;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
    }

     void Update()
     {
        moveDirection = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
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

        if (Input.GetKeyDown(KeyCode.LeftShift) && !hasDashed)
        {
            if (xRaw != 0 || yRaw != 0)
            {
                Dash(xRaw, yRaw);
            }
        }

        GetComponent<BetterJump>().enabled = true;
     }

    private void Dash(float x, float y)
    {
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(3*x, y);

        hasDashed = true;

        rb.velocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        StartCoroutine(GroundDash());

        rb.gravityScale = 0;
        GetComponent<BetterJump>().enabled = false;
        isDashing = true;

        yield return new WaitForSeconds(1f);

        rb.gravityScale = 4;
        GetComponent<BetterJump>().enabled = true;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
        
            hasDashed = false;
        

    }

    private void WallJump()
    {

        if (coll.onWall && !coll.onGround && Input.GetButtonDown("Jump"))
        {
            StopCoroutine(DisableMovement(0));
            StartCoroutine(DisableMovement(.15f));
            isWallJumping = true;
            wallJumpingDirection = -transform.localScale.x;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            Flip();
        }
    }

    IEnumerator DisableMovement(float time)
    {
        speed = 0;
        yield return new WaitForSeconds(time);
        speed = originalSpeed;
    }

    private void WallGrab()
    {
        
        isWallGrabbing = coll.onWall && Input.GetKey(KeyCode.W);
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

    IEnumerator WallGrabCooldown()
    {
        yield return new WaitForSeconds(wallGrabCooldown);
        isWallGrabCooldown = false;
        wallClimbTime = 0f;
    }

    private void Walk(Vector2 dir)
    {
        
        if (rb.velocity.y < 0 && isWallJumping)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpingSlowMo * Time.deltaTime);
        }
        if(!isWallJumping)
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
            MoveDirectionFlip();
        }
        else
        {
            StartCoroutine(EnableFlip(.5f));
        }
    }

    IEnumerator EnableFlip(float time)
    {
        yield return new WaitForSeconds(time);
        MoveDirectionFlip();
        StopCoroutine(EnableFlip(0));
    }

    private void Jump(Vector2 dir)
    {
        isJumping = true;
        rb.velocity += dir * JumpForce;
    }

    private void WallSlide()
    {
        if (coll.onWall && !coll.onGround && rb.velocity.y <0 && moveDirection != 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
        }
    }

    private void MoveDirectionFlip()
    {
        if (moveDirection > 0f && isFacingRight  )
        {
            Flip();
        }
        if(moveDirection < 0f && isFacingRight == false)
        {
            Flip();
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

