using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collision coll;
    private PlayerAttack playerAttack;
    private EnemyAttack enemyAttack;

    [HideInInspector]
    public float moveDirection = 0f;
    private float y = 0f;
    private float originalSpeed;
    private float originalGravity;

    [Header("BaseStats")]
    public float speed = 10;
    public float slideSpeed = 5;
    public float dashSpeed = 5f;
    public float JumpForce = 50;
    [SerializeField] public Vector2 wallJumpingForce = new Vector2(6f, 15f);

    [Space]
    [Header("Booleans")]
    public bool isMoving;
    public bool isJumping;
    public bool isFalling;
    public bool isWallSliding;
    public bool isWallJumping;
    public bool isClimbingLedge;

    //Booleans that dont need to be public for animations
    private bool cantMove;
    public bool isFacingLeft;

    [Space]
    [Header("KnockBack")]
    public float KBForce;
    public float KBCounter;
    public float KBTotalTime;
    public bool KnockFromRight;

    [Space]
    [Header("WallJumping")]
    private float wallJumpingDirection;
    private float wallJumpingSlowMo = 16f;

    [Space]
    [Header("Dash")]
    public bool hasDashed;

    [Space]
    [Header("LedgeClimbing")]
    public bool ledgeDetected;
    private bool canGrabLedge = true;
    private float ledgeClimbFinish = 0.5f;
    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;
    public Vector2 offset1;
    public Vector2 offset2;
    public Vector2 offsetLeft1;
    public Vector2 offsetLeft2;

    [Space]
    [Header("Particles")]
    public ParticleSystem jumpParticle;
    public ParticleSystem walljumprightParticle;
    public ParticleSystem walljumpleftParticle;

    void Start()
    {
        originalGravity = 3.5f;
        originalSpeed = speed;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
        playerAttack = GetComponent<PlayerAttack>();
        enemyAttack = FindAnyObjectByType<EnemyAttack>();
    }

    void Update()
    {
        moveDirection = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        isMoving = moveDirection != 0;

        //Get knockback
        if(KBCounter <= 0)
        {
            playerAttack.canAttackFromKnockback = true;
            cantMove = false;
        }
        else
        {
            if (KnockFromRight == true)
            {
                rb.velocity = new Vector2(-KBForce, KBForce);
            }
            if (KnockFromRight == false)
            {
                rb.velocity = new Vector2(KBForce, KBForce);
            }
            KBCounter -= Time.deltaTime;

            playerAttack.canAttackFromKnockback = false;
            cantMove = true;
        }
 
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
            if (coll.onWall && !coll.onGround && Input.GetButtonDown("Jump") && !playerAttack.isAttacking)
            {
                WallJump();
            }
            if (coll.onWall && !coll.onGround && rb.velocity.y < 0 && moveDirection != 0f && !playerAttack.isAttacking)
            {
               
                WallSlide();
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && !hasDashed && !playerAttack.isAttacking)
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
        rb.gravityScale = originalGravity;
        isClimbingLedge = false;
        transform.position = climbOverPosition;
        cantMove = false;
        Invoke("AllowLedgeGrab", 1f);
    }
    private void AllowLedgeGrab() 
    {
        canGrabLedge = true;
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

        yield return new WaitForSeconds(1f);

        rb.gravityScale = originalGravity;
        GetComponent<BetterJump>().enabled = true;
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
         rb.velocity = new Vector2(wallJumpingDirection * wallJumpingForce.x, wallJumpingForce.y);
         Flip();

        if (isFacingLeft)
        {
            walljumprightParticle.Play();
        }
        else if (!isFacingLeft)
        {
            walljumpleftParticle.Play();
        }

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
        jumpParticle.Play();
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
        if (!cantMove && !playerAttack.isAttacking && !playerAttack.stopFlip && !isClimbingLedge)
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

    public void Flip()
    {
            isFacingLeft = !isFacingLeft;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
    }
}

