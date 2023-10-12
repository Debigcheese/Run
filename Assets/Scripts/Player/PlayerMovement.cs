using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collision coll;
    private PlayerAttack playerAttack;
    private DamageFlash damageFlash;

    [HideInInspector] public float moveDirection = 0f;
    private float y = 0f;

    [Header("RBOriginal")]
    private float originalSpeed;
    [HideInInspector] public float originalGravity;
    private float originalDrag;
    private float originalAngularDrag;

    [Header("BaseStats")]
    public float speed = 10;
    public float slideSpeed = 5;
    public float JumpForce = 50;
    [SerializeField] public Vector2 wallJumpingForce = new Vector2(6f, 15f);

    [Space]
    [Header("Booleans")]
    public bool isMoving;
    public bool isJumpPressed;
    public bool isJumping;
    public bool isFalling;
    public bool isWallSliding;
    public bool isWallJumping;
    public bool isClimbingLedge;
    public bool isDashing;
    public bool isFacingLeft;
    public bool cantMove;

    [Space]
    [Header("KnockBack")]
    public float KBForce;
    public float KBCounter;
    public float KBTotalTime;
    public bool KnockFromRight;
    public bool cantMoveFromKnockback;

    [Space]
    [Header("WallJumping")]
    private float wallJumpingDirection;
    private float wallJumpingSlowMo = 16f;

    [Space]
    [Header("Dash")]
    public float dashSpeed = 5f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 5f;
    private bool canDash = true;
    public bool enableDashUponCollision;
    public Image dashCDImage;
    public GameObject Abilitybackground;
    private float dashTimer;

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
        damageFlash = GetComponent<DamageFlash>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
        playerAttack = GetComponent<PlayerAttack>();
        originalAngularDrag = rb.angularDrag;
        originalDrag = rb.drag;
        originalGravity = rb.gravityScale;
        originalSpeed = speed;
    }


    void Update()
    {


        //Get knockback
        if (KBCounter <= 0)
        {
            playerAttack.canAttackFromKnockback = true;
            cantMoveFromKnockback = false;
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
            cantMoveFromKnockback = true;
        }
 
        //Animation
        if (coll.onGround)
        {
            isJumping = false;
            isWallJumping = false;
            isFalling = false;
            isWallSliding = false;
        }

        if (rb.velocity.y > 0 && !coll.onGround && isJumpPressed)
        {
            isJumping = true;
            isWallSliding = false;
        }
        else if (rb.velocity.y < 0 && !isWallSliding && !coll.onGround)
        {
            isWallSliding = false;
            isFalling = true;
            isJumpPressed = false;
        }
        if(dashCDImage.fillAmount != 0)
        {
            dashTimer += Time.deltaTime;
            float fillPercentage = 1f- (dashTimer / (dashCooldown + dashDuration));
            dashCDImage.fillAmount = fillPercentage;
            if(dashCDImage.fillAmount == 0)
            {
                dashTimer = 0;
            }
        }

        if (enableDashUponCollision)
        {
            Abilitybackground.SetActive(true);
        }
        

        //Methods
       
        if (!cantMove && !isDashing && !cantMoveFromKnockback)
        {
            moveDirection = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");
            isMoving = moveDirection != 0;

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

            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && isMoving && enableDashUponCollision)
            {
                Dash(dir);
            }
        }

        CheckForLedge();
        
        GetComponent<BetterJump>().enabled = true;
    }

    //inWater
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 4)
        {
            rb.drag = 6;
            rb.angularDrag = 6;
            rb.gravityScale = 0.6f;
            speed = originalSpeed * 0.7f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 4)
        {
            rb.drag = originalDrag;
            rb.angularDrag = originalAngularDrag;
            rb.gravityScale = originalGravity;
            speed = originalSpeed;
        }
    }

    private void CheckForLedge()
    {
        if (ledgeDetected && canGrabLedge  )
        {
            rb.velocity = Vector2.zero;
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

    public void Dash(Vector2 dir)
    {
        dashCDImage.fillAmount = 1f;
        canDash = false;
        isDashing = true;
        if(rb.velocity.x > 0)
        {
            rb.AddForce(new Vector2(1, 0)*dashSpeed, ForceMode2D.Impulse);
        }
        if(rb.velocity.x < 0)
        {
            rb.AddForce(new Vector2(-1, 0) * dashSpeed, ForceMode2D.Impulse);
        }
        damageFlash.dashFlashOn = true;
        StartCoroutine(DashWait());
        damageFlash.CallDashFlash();
    }

    IEnumerator DashWait()
    {
        rb.gravityScale = 0;
        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
        damageFlash.dashFlashOn = false;
        StartCoroutine(DashCooldown());
    }

    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
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
        isJumpPressed = true;
        isJumping = true;
        rb.velocity = dir.normalized * JumpForce;
    }

    private void WallSlide()
    {
        isJumpPressed = false;
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

