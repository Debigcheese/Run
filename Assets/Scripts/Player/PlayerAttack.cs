using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    private WeaponHolder weaponHolder;
    public Animator[] anim;
    public LayerMask enemyLayer;
    public bool isAttacking = false;
    [HideInInspector]
    public float originalAttackMoveSpeed;
    public float meleeAttackMoveSpeed = 1f;
    public float rangedAttackMoveSpeed = 1.9f;
    public bool checkMousePositionX;
    public bool stopFlip = false;
    public bool canAttackFromKnockback = true;
    public bool canAttack = true;
    public bool stopAttacking = false;
    public bool dialogueStopAttack = false;
    public float meleeAttackForce;
    public bool insufficientEnergyAttack;

    public bool attackDashDone = true;

    [Header("CritAttack")]
    public bool critAttack;
    public float critChance;
    public float critDamageMultiplier;

    [Header("VengeanceAbility")]
    public bool rageEnabled = false;
    public float rageDamageMultiplier = 1.2f;
    public float rageDuration;
    public float rageCooldown;
    private bool checkRageCooldown;
    private float vengeanceTimer = 0f;
    public int ExplosionDamage;
    public float explosionRadius;
    public Image vengeanceCDImage;
    public GameObject vengeanceIcon;
    public Color vengeanceColor;
    public Color originalColor;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        weaponHolder = GetComponent<WeaponHolder>();
        originalAttackMoveSpeed = meleeAttackMoveSpeed;
        critChance = PlayerPrefs.GetFloat("CritChance", 4f);
        critDamageMultiplier = PlayerPrefs.GetFloat("CritDamage", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        if (Input.GetButtonDown("Fire1") && !playerMovement.isWallSliding && !playerMovement.isClimbingLedge && !isAttacking && canAttackFromKnockback && canAttack && !stopAttacking && !dialogueStopAttack && !insufficientEnergyAttack)
        {
            float randomValue = Random.Range(0f, 100f);
            if (randomValue <= critChance)
            {
                critAttack = true;
            }
            else
            {
                critAttack = false;
            }
            isAttacking = true;

            if (mousePosition.x > transform.position.x && playerMovement.isFacingLeft || mousePosition.x < transform.position.x && !playerMovement.isFacingLeft)
            {
                stopFlip = true;
                playerMovement.Flip();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && PlayerPrefs.GetInt("Ability") == 2 && !checkRageCooldown)
        {
            AudioManager.Instance.PlaySound("playeractivatevengeance");
            rageEnabled = true;
            checkRageCooldown = true;
            vengeanceCDImage.fillAmount = 1f;
            VengeanceExplosion();
            StartCoroutine(RageDuration());
        }
        if(PlayerPrefs.GetInt("Ability") == 2)
        {
            vengeanceIcon.SetActive(true);
            if (!rageEnabled && checkRageCooldown)
            {
                vengeanceTimer += Time.deltaTime;
                float fillPercentage = 1f - (vengeanceTimer / (rageCooldown));
                vengeanceCDImage.fillAmount = fillPercentage;
                if (vengeanceCDImage.fillAmount == 0)
                {
                    vengeanceTimer = 0;
                }
            }
        }
        else
        {
            vengeanceIcon.SetActive(false);
        }
        if (rageEnabled)
        {
            AudioManager.Instance.PlayLoopingSound("playerduringvengeance");
        }
        

        //cant attack when ledgeclimb,wallslide,walljump,hurt
        if (playerMovement.isClimbingLedge || playerMovement.isWallSliding)
        {
            isAttacking = false;
        }
        //if attacking and not in air, slow ms

        if (isAttacking && !playerMovement.isDashing && (weaponHolder.isUsingMagic || weaponHolder.isUsingRanged))
        {
            rb.velocity = new Vector2(playerMovement.moveDirection * rangedAttackMoveSpeed, rb.velocity.y);
        }

        //melee attack dash, (might need to change in playermovement)
        if (isAttacking && !playerMovement.isFalling  && !playerMovement.isDashing && weaponHolder.isUsingMelee)
        {
            playerMovement.cantMove = true;
            attackDashDone = false;
            Vector2 directionToMouse = (mousePosition - transform.position);
            rb.velocity = (directionToMouse * meleeAttackMoveSpeed).normalized;

        }
        if (!isAttacking && !attackDashDone && !dialogueStopAttack)
        {
            playerMovement.cantMove = false;
            attackDashDone = true;
        }
        //stop flip character when attacking
        if (!isAttacking)
        {
            stopFlip = false;
        }
        if (!isAttacking)
        {
            stopAttacking = false;
        }

        for(int i = 0; i< anim.Length; i++)
        {
            anim[i].SetBool("VengeanceActive", rageEnabled);
        }

        VengeanceChangeColor(GetComponentInChildren<SpriteRenderer>());
    }

    private IEnumerator RageDuration()
    {
        yield return new WaitForSeconds(rageDuration);
        rageEnabled = false;
        AudioManager.Instance.DisableSound("playerduringvengeance");
        AudioManager.Instance.PlaySound("playerdisablevengeance");
        StartCoroutine(RageCooldownTimer());
    }

    private IEnumerator RageCooldownTimer()
    {
        yield return new WaitForSeconds(rageCooldown);
        checkRageCooldown = false;
    }

    public void VengeanceChangeColor(SpriteRenderer sprite)
    {
        if (PlayerPrefs.GetInt("Ability") == 2)
        {
            if (rageEnabled)
            {
                sprite.color = vengeanceColor;
            }
            else
            {
                sprite.color = originalColor;
            }
        }
        
    }

    public void VengeanceExplosion()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHp>().TakeDamage(ExplosionDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

}
