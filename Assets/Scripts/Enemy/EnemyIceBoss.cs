using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class EnemyIceBoss : MonoBehaviour
{
    private Animator anim;
    public Animator uiAnim;
    private PlayerMovement playerMovement;
    private PlayerState playerState;
    private EnemyAI enemyAI;
    private EnemyHp enemyHp;
    private WaveSpawner waveSpawner;
    private Collider2D[] hitPlayersCollider;
    private ChromaticAberration CA;

    public GameObject healthBar;
    public GameObject healthUI;
    public Slider enemyHealthBar;
    public Slider easeHealthBar;

    [Header("SFX")]
    public string spawnSFX;

    [Header("Booleans")]
    public bool isMoving;
    public bool isAttacking;
    public int attackStyle = 1;
    public bool inRange;
    public bool inRangeLongAttack;
    public bool inRangeSpecialAttack;
    public bool canAttack = true;
    public bool stopAttack = false;

    [Header("Hiding")]
    public bool isHiding = false;
    public bool isShowing = false;
    private int hideCount = 0;
    private int stopHiding = 0;

    public GameObject[] spawnTotems;
    public Transform[] totemSpawnpoint;
    public int spawnersDestroyed = 0;

    [Header("Balancing")]
    public int attackDamage;
    public float attackCooldown;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange;
    public float attackDmgDelay;
    public float attackAnimationTime;

    [Header("LongAttack")]
    public Transform longAttackPoint;
    public float longAttackRange;
    public float longAttackDmgDelay;
    public float longAttackAnimationTime;
    public float longAttackCooldown;

    [Space]
    public float longAttackTimer = 0f;
    public bool canLongAttack = false;
    [SerializeField] public bool resetLongAttack = false;
    public bool longAttackDisableKnockback = false;

    [Header("LongAttackSpecial")]
    public Transform specialAttackPoint;
    public float specialAttackRange;
    public float specialAttackDmgDelay;

    private bool switchToSpecial;

    //[Header("Enraged")]
    //public bool enraged = false;
    //public bool isEnragedAnim = false;
    //public Color originalColor;
    //public Color enragedColor;
    //private float enragedColorTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        CA = FindObjectOfType<ChromaticAberration>();
        enemyAI = GetComponent<EnemyAI>();
        anim = transform.Find("EnemyAnim").GetComponent<Animator>();
        healthUI.SetActive(true);
        //

        playerMovement = FindAnyObjectByType<PlayerMovement>();
        playerState = FindAnyObjectByType<PlayerState>();
        enemyAI = GetComponent<EnemyAI>();
        enemyHp = GetComponent<EnemyHp>();
        waveSpawner = GetComponentInParent<WaveSpawner>();
        canAttack = true;

        enemyHealthBar.maxValue = enemyHp.maxHealth;
        easeHealthBar.maxValue = enemyHp.maxHealth;

        healthBar.SetActive(false);
        GetComponent<Rigidbody2D>().simulated = false;
        anim.SetBool("isShowingStart", true);
        enemyAI.canMove = false;
        canAttack = false;

        StartCoroutine(IsShowingStart());
        StartCoroutine(DiableHealthBarUIAnim());
    }

    private IEnumerator IsShowingStart()
    {
        yield return new WaitForSeconds(1.5f);
        anim.SetBool("isShowingStart", false);
        healthBar.SetActive(true);
        GetComponent<Rigidbody2D>().simulated = true;
        enemyAI.canMove = true;
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isAttacking", isAttacking);
        anim.SetInteger("attackStyle", attackStyle);
        anim.SetBool("inRangeLongAttack", inRangeLongAttack);
        anim.SetBool("inRangeSpecialAttack", inRangeSpecialAttack);

        anim.SetBool("isHiding", isHiding);
        anim.SetBool("isShowing", isShowing);

        enemyHealthBar.value = enemyHp.enemyHealthBar.value;
        easeHealthBar.value = enemyHp.easeHealthBar.value;

        if (enemyAI.isMoving)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (enemyHp.currentHealth <= (enemyHp.maxHealth / 1.5) && !isHiding && hideCount == 0)
        {
            enemyHp.currentHealth = Mathf.RoundToInt(enemyHp.maxHealth / 1.5f);
            enemyAI.canMove = false; 
            canAttack = false;
            isHiding = true;
            hideCount = 1;
            StartCoroutine(GoHide());
        }

        if (enemyHp.currentHealth <= (enemyHp.maxHealth / 3) && !isHiding && hideCount == 1)
        {
            enemyHp.currentHealth = Mathf.RoundToInt(enemyHp.maxHealth / 3);
            enemyAI.canMove = false;
            canAttack = false;
            isHiding = true;
            hideCount = 2;
            StartCoroutine(GoHide());
        }

        //if (enemyHp.currentHealth <= (enemyHp.maxHealth / 2.1f) && !enraged)
        //{
        //    Enraged();
        //}
        //if (isEnragedAnim)
        //{
        //    enragedColorTimer += Time.deltaTime;
        //    GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(originalColor, enragedColor, enragedColorTimer / 3.5f);
        //}

        InvokeRepeating("AttackIfInRange", 0f, 0.6f);

        if (attackStyle <= 1 && !inRangeLongAttack && !inRangeSpecialAttack)
        {
            CheckForPlayerAttack();
        }
        if (canLongAttack)
        {
            CheckForPlayerLongAttack();
        }

        if (!canLongAttack)
        {
            longAttackTimer += Time.deltaTime;
            if (longAttackTimer >= 4)
            {
                canLongAttack = true;
                longAttackTimer = 0f;
            }
        }

        if (enemyHp.currentHealth <= 0 && waveSpawner != null)
        {
            waveSpawner.bossDefeated = true;
        }

        if (spawnTotems[1].GetComponentInChildren<EnemySpawner>().spawnerDestroyed && spawnTotems[2].GetComponentInChildren<EnemySpawner>().spawnerDestroyed)
        {
            spawnersDestroyed = 3;
        }
        else if (spawnTotems[0].GetComponentInChildren<EnemySpawner>().spawnerDestroyed)
        {
            spawnersDestroyed = 1;
        }

        if ((spawnersDestroyed == 1 && stopHiding == 0) || (spawnersDestroyed == 3 && stopHiding == 1))
        {
            StartCoroutine(UnHide());
        }

        if (enemyHp.isDead)
        {
            canAttack = false;
            isAttacking = false;
            healthUI.SetActive(false);
        }
    }

    private IEnumerator GoHide()
    {
        enemyAI.directionLookEnabled = false;
        GetComponent<Rigidbody2D>().simulated = false;

        yield return new WaitForSeconds(0.55f);
        if (hideCount == 1)
        {
            spawnTotems[0] = Instantiate(spawnTotems[0], totemSpawnpoint[0].position, Quaternion.identity, totemSpawnpoint[0].transform);
            spawnTotems[0].GetComponentInChildren<EnemySpawner>().spawnedByBoss = true;
        }
        else
        {
            spawnTotems[1] = Instantiate(spawnTotems[1], totemSpawnpoint[1].position, Quaternion.identity, totemSpawnpoint[1].transform);
            spawnTotems[2] = Instantiate(spawnTotems[2], totemSpawnpoint[2].position, Quaternion.identity, totemSpawnpoint[2].transform);
            spawnTotems[1].GetComponentInChildren<EnemySpawner>().spawnedByBoss = true;
            spawnTotems[2].GetComponentInChildren<EnemySpawner>().spawnedByBoss = true;
        }

        yield return new WaitForSeconds(1f);
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        healthBar.SetActive(false);
    }


    private IEnumerator UnHide()
    {
        stopHiding++;
        isHiding = false;
        isShowing = true;
        healthBar.SetActive(true);
        GetComponentInChildren<SpriteRenderer>().enabled = true;

        yield return new WaitForSeconds(1.5f);
        enemyAI.directionLookEnabled = true;
        isShowing = false;
        enemyAI.canMove = true;
        canAttack = true;

        GetComponent<Rigidbody2D>().simulated = true;
    }

    private void CheckForPlayerAttack()
    {
        hitPlayersCollider = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        foreach (Collider2D hitplayer in hitPlayersCollider)
        {
            if (hitplayer.CompareTag("Player") && hitplayer != null && canAttack)
            {
                inRange = true;
                break;
            }
            else 
            {
                inRange = false;
            }
        }
    }

    private void CheckForPlayerLongAttack()
    {
        if (!switchToSpecial)
        {
            hitPlayersCollider = Physics2D.OverlapCircleAll(longAttackPoint.position, longAttackRange);
        }
        else
        {
            hitPlayersCollider = Physics2D.OverlapCircleAll(specialAttackPoint.position, specialAttackRange);
        }

        foreach (Collider2D hitplayer in hitPlayersCollider)
        {
            if (hitplayer.CompareTag("Player") && hitplayer != null && canAttack)
            {
                if (!switchToSpecial)
                {
                    inRangeLongAttack = true;
                    break;
                }
                else
                {
                    inRangeSpecialAttack = true;
                    break;
                }
            }
            else 
            {
                if (!switchToSpecial)
                {
                    inRangeLongAttack = false;
                }
                else
                {
                    inRangeSpecialAttack = false;
                }
            }
        }
    }

    private void AttackIfInRange()
    {
        if ((inRange || inRangeLongAttack || inRangeSpecialAttack) && !stopAttack)
        {
            enemyAI.canMove = false;
            stopAttack = true;
            if (!isAttacking)
            {
                isAttacking = true;
                if (inRangeLongAttack || inRangeSpecialAttack)
                {
                    StartCoroutine(LongAttackDmgDelay());
                    StartCoroutine(LongAttackAnimationTime());
                    StartCoroutine(ChromaticAbbreviation());
                }
                else
                {
                    StartCoroutine(AttackDmgDelay());
                    StartCoroutine(AttackAnimationTime());
                }

            }
            if (enemyHp.enemySFX.EnemyAttackSFX != null)
            {
                enemyHp.enemySFX.PlayEnemySound(enemyHp.enemySFX.EnemyAttackSFX);
            }

        }
    }

    IEnumerator ChromaticAbbreviation()
    {
        yield return new WaitForSeconds(.3f);
        CA.intensity.value = 0.5f;
        yield return new WaitForSeconds(.2f);
        CA.intensity.value = 0f;
    }

    IEnumerator AttackDmgDelay()
    {
        yield return new WaitForSeconds(attackDmgDelay);
        Attack();
    }

    IEnumerator LongAttackDmgDelay()
    {
        enemyAI.directionLookEnabled = false;
        longAttackDisableKnockback = true;
        if (!switchToSpecial)
        {
            yield return new WaitForSeconds(longAttackDmgDelay);
        }
        else
        {
            yield return new WaitForSeconds(specialAttackDmgDelay);
        }
        resetLongAttack = true;
        Attack();
    }

    private void Attack()
    {
        if ((inRange && !resetLongAttack)  || (inRangeLongAttack && !switchToSpecial) || (inRangeSpecialAttack && switchToSpecial))
        {
            playerState.TakeDamage(attackDamage);
            //knockback the player
            playerMovement.KBCounter = playerMovement.KBTotalTime;
            if (playerMovement.transform.position.x <= transform.position.x)
            {
                playerMovement.KnockFromRight = true;
            }
            if (playerMovement.transform.position.x >= transform.position.x)
            {
                playerMovement.KnockFromRight = false;
            }
        }
    }
    
    IEnumerator AttackAnimationTime()
    {
        yield return new WaitForSeconds(attackAnimationTime);
        isAttacking = false;

        StartCoroutine(AttackCooldown());
    }

    IEnumerator LongAttackAnimationTime()
    {
        yield return new WaitForSeconds(longAttackAnimationTime);
        isAttacking = false;
        longAttackDisableKnockback = false;
        enemyAI.directionLookEnabled = true;
        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        if (resetLongAttack)
        {
            yield return new WaitForSeconds(longAttackCooldown);
        }
        else
        {
            yield return new WaitForSeconds(attackCooldown);
        }

        enemyAI.canMove = true;
        stopAttack = false;
        attackStyle *= -1;

        if (resetLongAttack)
        {
            canLongAttack = false;
            inRangeLongAttack = false;
            inRangeSpecialAttack = false;
            resetLongAttack = false;

            if (!switchToSpecial)
            {
                switchToSpecial = true;
            }
            else
            {
                switchToSpecial = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        Gizmos.DrawWireSphere(longAttackPoint.position, longAttackRange);
        Gizmos.DrawWireSphere(specialAttackPoint.position, specialAttackRange);

    }

    //private void Enraged()
    //{
    //    enraged = true;
    //    isEnragedAnim = true;
    //    canAttack = false;
    //    enemyAI.canMove = false;
    //    enemyHp.canTakeDamage = false;
    //    GetComponent<Rigidbody2D>().simulated = false;

    //    attackDamage = Mathf.RoundToInt(attackDamage * 1.5f);
    //    enemyAI.speed = enemyAI.speed * 1.52f;

    //    AudioManager.Instance.PlaySound(spawnSFX);
    //    FindObjectOfType<CameraShake>().ShakeCameraFlex(4f, 2.5f);

    //    StartCoroutine(StopEnragedAnim());
    //}

    //private IEnumerator StopEnragedAnim()
    //{
    //    yield return new WaitForSeconds(3.5f);
    //    isEnragedAnim = false;
    //    canAttack = true;
    //    enemyAI.canMove = true;
    //    enemyHp.canTakeDamage = true;
    //    GetComponent<Rigidbody2D>().simulated = true;
    //}

    private IEnumerator DiableHealthBarUIAnim()
    {
        yield return new WaitForSeconds(2.45f);
        uiAnim.enabled = false;
    }

}

