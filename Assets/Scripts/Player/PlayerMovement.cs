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
    private AudioManager audioManager;
    private PlayerState playerState;
    private SnowTileScript snowTile;

    [HideInInspector] public float moveDirection = 0f;
    private float y = 0f;

    [Header("RBOriginal")]
    public float originalSpeed;
    [HideInInspector] public float originalGravity;
    private float originalDrag;
    private float originalAngularDrag;

    [Header("BaseStats")]
    public float speed = 10;
    public float slideSpeed = 5;
    public float JumpForce = 50;
    [SerializeField] public Vector2 wallJumpingForce = new Vector2(6f, 15f);
    public float startingMovementTimer = 3f;

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
    public bool justFlipped;
    public bool isInWater;
    private bool playerOnSnow;
    private bool isExitingSnow;
    private bool secondSnowSound;
    private float secondSnowSoundTimer = 0;

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
    private bool enableFlipWhenWalljumping = true;

    [Space]
    [Header("Dash")]
    public float dashSpeed = 5f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 5f;
    private bool canDash = true;
    public bool enableDashUponCollision;
    public Image dashCDImage;
    public GameObject dashAbilityImage;
    public GameObject Abilitybackground;
    private float dashTimer;

    [Space]
    [Header("LedgeClimbing")]
    public bool ledgeDetected;
    private bool canGrabLedge = true;
    public float ledgeClimbFinish = 0.5f;
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
    public ParticleSystem dashParticle;

    void Start()
    {
        damageFlash = GetComponent<DamageFlash>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
        playerAttack = GetComponent<PlayerAttack>();
        audioManager = FindAnyObjectByType<AudioManager>();
        playerState = GetComponent<PlayerState>();
        originalAngularDrag = rb.angularDrag;
        originalDrag = rb.drag;
        originalGravity = rb.gravityScale;
        originalSpeed = speed;
        enableDashUponCollision = PlayerPrefs.GetInt("EnableDash") == 1;

        StartLevelMovement();

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

        //Walking Sounds
        //grassWalking
        if(isMoving && coll.onGround && !playerAttack.isAttacking && !isInWater && !playerState.guardianEnabled && !playerState.isRegeningHp && !playerOnSnow)
        {
            AudioManager.Instance.PlayLoopingSound("playerfootsteps");
        }
        if(isMoving && coll.onGround && (playerAttack.isAttacking || isInWater || playerState.guardianEnabled) && !playerOnSnow)
        {
            AudioManager.Instance.PlayLoopingSound("playerslowfootsteps");
        }
        //snowwalking
        if(isMoving && coll.onGround && !playerAttack.isAttacking && !isInWater && !playerState.guardianEnabled && !playerState.isRegeningHp && playerOnSnow && !secondSnowSound)
        {
            AudioManager.Instance.PlayLoopingSound("playerfootstepssnow");
            secondSnowSoundTimer += Time.deltaTime;
            if(secondSnowSoundTimer >= .24f)
            {
                secondSnowSound = true;
                secondSnowSoundTimer = 0f;
            }
        }
        if (isMoving && coll.onGround && !playerAttack.isAttacking && !isInWater && !playerState.guardianEnabled && !playerState.isRegeningHp && playerOnSnow && secondSnowSound)
        {
            AudioManager.Instance.PlayLoopingSound("playerfootstepssnowsecond");
            secondSnowSoundTimer += Time.deltaTime;
            if (secondSnowSoundTimer >= .24f)
            {
                secondSnowSound = false;
                secondSnowSoundTimer = 0f;
            }
        }
        if (isMoving && coll.onGround && (playerAttack.isAttacking || isInWater || playerState.guardianEnabled) && playerOnSnow)
        {
            AudioManager.Instance.PlayLoopingSound("playerslowfootstepssnow");
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
        else
        {
            Abilitybackground.SetActive(false);
        }

        //Call Methods
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
            int ability = PlayerPrefs.GetInt("Ability", 0);
            if (ability == 0 && enableDashUponCollision)
            {
                dashAbilityImage.SetActive(true);
            }
            else
            {
                dashAbilityImage.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && isMoving && enableDashUponCollision && ability == 0 )
            {
                Dash(dir, false);

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
            AudioManager.Instance.PlayLoopingSound("playerinwater");
            isInWater = true;
            rb.drag = 6;
            rb.angularDrag = 6;
            rb.gravityScale = 0.6f;
            speed = originalSpeed * 0.7f;
        }
        if (collision.CompareTag("Snow"))
        {
            playerOnSnow = true;
            isExitingSnow = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 4)
        {
            isInWater = false;
            rb.drag = originalDrag;
            rb.angularDrag = originalAngularDrag;
            rb.gravityScale = originalGravity;
            speed = originalSpeed;
        }
        if (collision.CompareTag("Snow"))
        {
            isExitingSnow = true;
            StartCoroutine(PlayerExitSnow());
        }
    }

    private IEnumerator PlayerExitSnow()
    {
        yield return new WaitForSeconds(0.43f);
        if (isExitingSnow)
        {
            playerOnSnow = false;
        }
    }

    private void CheckForLedge()
    {
        if (ledgeDetected && canGrabLedge)
        {
            AudioManager.Instance.PlaySound("playerledgeclimb");
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
            rb.simulated = false;
            transform.position = climbBegunPosition;
            StartCoroutine(LedgeClimbOver());
        }

    }

    IEnumerator LedgeClimbOver()
    {
        yield return new WaitForSeconds(ledgeClimbFinish);
        rb.simulated = true;
        isClimbingLedge = false;
        transform.position = climbOverPosition;
        cantMove = false;
        canGrabLedge = true;
    }

    public void Dash(Vector2 dir, bool firstLevelDash)
    {
        dashParticle.Play();
        dashCDImage.fillAmount = 1f;
        canDash = false;
        isDashing = true;
        if(rb.velocity.x > 0 && !firstLevelDash)
        {
            rb.AddForce(new Vector2(1, 0)*dashSpeed, ForceMode2D.Impulse);
        }
        if (firstLevelDash)
        {
            rb.AddForce(new Vector2(1.5f, 1.5f) * dashSpeed, ForceMode2D.Impulse);
        }
        if(rb.velocity.x < 0 && !firstLevelDash)
        {
            rb.AddForce(new Vector2(-1, 0) * dashSpeed, ForceMode2D.Impulse);
        }
        damageFlash.dashFlashOn = true;
        StartCoroutine(DashWait());
        damageFlash.CallDashFlash();
        AudioManager.Instance.PlaySound("playerdash");
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
        AudioManager.Instance.PlaySound("playerjump");
        isJumping = true;
        StartCoroutine(DisableMovement(.12f));
        isWallJumping = true;
        rb.velocity = Vector2.zero;
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
        rb.simulated = false;
        yield return new WaitForSeconds(time);
        rb.simulated = true;
    }

    private void Walk(Vector2 dir)
    {
        
        if (rb.velocity.y < 0 && isWallJumping)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpingSlowMo * Time.deltaTime);
        }
        if(!isWallJumping && !playerAttack.isAttacking )
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
            MoveDirectionFlip();

        }
        else if(enableFlipWhenWalljumping && isWallJumping)
        {
            StartCoroutine(EnableFlip(.4f));
            enableFlipWhenWalljumping = false;
        }
    }

    IEnumerator EnableFlip(float time)
    {
        yield return new WaitForSeconds(time);
        MoveDirectionFlip();
        enableFlipWhenWalljumping = true;
    }

    private void Jump(Vector2 dir)
    {
        jumpParticle.Play();
        isJumpPressed = true;
        isJumping = true;
        rb.velocity = dir.normalized * JumpForce;
        AudioManager.Instance.PlaySound("playerjump");
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
        StartCoroutine(JustFlippedChecker());
    }

    public void StartLevelMovement()
    {
        cantMove = true;
        playerAttack.dialogueStopAttack = true;
        rb.velocity = new Vector2(speed, 0);
        isMoving = true;
        StartCoroutine(StopStartingMovement());
    }

    private IEnumerator StopStartingMovement()
    {
        yield return new WaitForSeconds(startingMovementTimer);
        cantMove = false;
        playerAttack.dialogueStopAttack = false;
    }

    public void FinishLevelMovement()
    {
        cantMove = true;
        playerAttack.dialogueStopAttack = true;
        isMoving = true;
        if (isFacingLeft)
        {
            rb.velocity = new Vector2(-speed, 0);
        }
        if (!isFacingLeft)
        {
            rb.velocity = new Vector2(speed, 0);
        }

    }

    private IEnumerator JustFlippedChecker()
    {
        justFlipped = true;
        yield return new WaitForSeconds(0.01f);
        justFlipped = false;
    }

    public bool GroundCheck()
    {
        return coll.onGround;
    }
}

