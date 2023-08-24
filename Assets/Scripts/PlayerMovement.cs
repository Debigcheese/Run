using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collision coll;

    [HideInInspector]
    public float moveDirection = 0f;
    private float y = 0f;
    
    private float originalSpeed;
    private float originalGravity;
    

    [Header("Stats")]
    public float speed = 10;
    public float JumpForce = 50;
    public float slideSpeed = 5;
    public float dashSpeed = 5f;

    [Space]
    [Header("Booleans")]
    public bool cantMove;
    public bool isMoving;
    public bool isJumping;
    public bool isFalling;
    public bool isWallSliding;
    public bool isWallJumping;
    public bool isDashing;
    public bool ledgeDetected;
    public bool isFacingLeft;

    [Space]
    [Header("WallJumping")]
    [SerializeField] public Vector2 wallJumpingPower = new Vector2(1f, 1f);
    private float wallJumpingDirection;
    public float wallJumpingSlowMo;

    [Space]
    [Header("Dash")]
    public bool hasDashed;

    [Space]
    [Header("LedgeClimbing")]
    public Vector2 offset1;
    public Vector2 offset2;
    public Vector2 offsetLeft1;
    public Vector2 offsetLeft2;
    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;
    public bool canGrabLedge = true;
    public bool isClimbingLedge = false;
    public float ledgeClimbFinish;

    void Start()
    {
        originalGravity = 4;
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
        isMoving = moveDirection != 0;

        //Animation
        if (coll.onGround)
        {
            isJumping = false;
            isWallJumping = false;
            isFalling = false;
            isWallSliding = false;
        }

        if (rb.velocity.y > 0)
        {
            isJumping = true;
            isWallSliding = false;
        }
        else if (rb.velocity.y < 0 && !isWallSliding)
        {
            isWallSliding = false;
            isFalling = true;
        }


        //Methods

        if (!cantMove)
        {
            Vector2 dir = new Vector2(moveDirection, y);
            Walk(dir);

            if (coll.onGround && Input.GetButtonDown("Jump"))
            {
                Jump(Vector2.up);
            }
            if (coll.onWall && !coll.onGround && Input.GetButtonDown("Jump"))
            {
                WallJump();
            }
            if (coll.onWall && !coll.onGround && rb.velocity.y < 0 && moveDirection != 0f)
            {
                WallSlide();
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && !hasDashed)
            {
                if (xRaw != 0 || yRaw != 0)
                {
                    Dash(xRaw, yRaw);
                }
            }
        }
        
        CheckForLedge();
        GetComponent<BetterJump>().enabled = true;
     }

    private void CheckForLedge()
    {
        if (ledgeDetected && canGrabLedge  )
        {
            canGrabLedge = false;
            isClimbingLedge = true;
            cantMove = true;
            Vector2 ledgePosition = GetComponentInChildren<LedgeDetection>().transform.position;


            if (!isFacingLeft)
            {
                climbBegunPosition = ledgePosition + offset1;
                climbOverPosition = ledgePosition + offset2;
            }
            if(isFacingLeft)
            {
                climbBegunPosition = ledgePosition + offsetLeft1;
                climbOverPosition = ledgePosition + offsetLeft2;
            }

            StartCoroutine(LedgeClimbOver());
        }

        if (isClimbingLedge)
        {
            rb.gravityScale = -1.7f;
            transform.position = climbBegunPosition;
        }
    }
    IEnumerator LedgeClimbOver()
    {
        yield return new WaitForSeconds(ledgeClimbFinish);
        rb.gravityScale = 4;
        isClimbingLedge = false;
        transform.position = climbOverPosition;
        Invoke("AllowLedgeGrab", .1f);
    }
    private void AllowLedgeGrab() 
    {
        canGrabLedge = true;
        cantMove = false;
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

        rb.gravityScale = originalGravity;
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
         isJumping = true;
         StopCoroutine(DisableMovement(0));
         StartCoroutine(DisableMovement(.15f));
         isWallJumping = true;
         wallJumpingDirection = -transform.localScale.x;
         rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
         Flip();
    }

    IEnumerator DisableMovement(float time)
    {
        speed = 0;
        yield return new WaitForSeconds(time);
        speed = originalSpeed;
    }

    private void Walk(Vector2 dir)
    {
        
        if (rb.velocity.y < 0 && isWallJumping)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpingSlowMo * Time.deltaTime);
        }
        if(!isWallJumping )
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
            MoveDirectionFlip();
        }
        else
        {
            StartCoroutine(EnableFlip(.4f));
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
        isJumping = false;
        isFalling = false;
        isWallSliding = true;
        rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
    }

    private void MoveDirectionFlip()
    {
        if (!cantMove)
        {
            if (moveDirection > 0f && isFacingLeft)
            {
                isFacingLeft = true;
                Flip();
            }
            if (moveDirection < 0f && isFacingLeft == false)
            {
                isFacingLeft = false;
                Flip();
            }
        }
    }

    private void Flip()
    {
            isFacingLeft = !isFacingLeft;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
    }
}

