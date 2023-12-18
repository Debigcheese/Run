using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMinotaur : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement playerMovement;
    private PlayerState playerState;
    private EnemyAI enemyAI;
    private EnemyHp enemyHp;
    private WaveSpawner waveSpawner;
    private Collider2D[] hitPlayersCollider;
    private RaycastHit2D[] hitPlayersRayCollider;
    public Transform attackPoint;
    public Transform swingAttackPoint;

    public GameObject healthUI;
    public Slider enemyHealthBar;
    public Slider easeHealthBar;

    [Header("Booleans")]
    public bool isMoving;
    public bool isAttacking;
    public int attackStyle = 1;
    public bool inRange;
    public bool canAttack = true;
    public bool stopAttack = false;
    public bool useRayCast = false;

    [Header("Balancing")]
    public int attackDamage;
    public float attackSpeed;
    public float attackDmgDelay;
    public float attackSwingDmgDelay;
    public float attackRange;
    public float attackRangeSwing;
    public float attackCooldown;

    // Start is called before the first frame update
    void Start()
    {
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
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isAttacking", isAttacking);
        anim.SetInteger("attackStyle", attackStyle);

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

        //

        InvokeRepeating("AttackIfInRange", 0f, 0.6f);

        if (!useRayCast && attackStyle <0)
        {
            CheckForPlayerCircle();
        }
        else
        {
            CheckForPlayerRayCast();
        }

        if(enemyHp.currentHealth <= 0)
        {
            waveSpawner.bossDefeated = true;
        }
    }


    private void CheckForPlayerCircle()
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

    private void CheckForPlayerRayCast()
    {
        if (transform.position.x < playerMovement.transform.position.x)
        {
            hitPlayersRayCollider = Physics2D.RaycastAll(swingAttackPoint.position, Vector2.right, attackRangeSwing);
        }
        else
        {
            hitPlayersRayCollider = Physics2D.RaycastAll(swingAttackPoint.position, Vector2.left, attackRangeSwing);
        }

        foreach (RaycastHit2D hitplayer in hitPlayersRayCollider)
        {
            if (hitplayer.collider.CompareTag("Player") && hitplayer.collider != null && canAttack)
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


    private void AttackIfInRange()
    {
        if (inRange && !stopAttack)
        {
            enemyAI.canMove = false;
            stopAttack = true;
            if (!isAttacking)
            {
                isAttacking = true;
                if (useRayCast)
                {
                    StartCoroutine(SwingAttackDmgDelay());
                }
                else
                {
                    StartCoroutine(AttackDmgDelay());
                }

            }
        }
    }

    IEnumerator AttackDmgDelay()
    {
        yield return new WaitForSeconds(attackDmgDelay);
        Attack();
    }

    IEnumerator SwingAttackDmgDelay()
    {
        yield return new WaitForSeconds(attackSwingDmgDelay);
        Attack();
    }

    private void Attack()
    {
        if (inRange)
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
        StartCoroutine("AttackSpeed");

        if (enemyHp.enemySFX.EnemyAttackSFX != null)
        {
            enemyHp.enemySFX.PlayEnemySound(enemyHp.enemySFX.EnemyAttackSFX);
        }
    }

    IEnumerator AttackSpeed()
    {
        yield return new WaitForSeconds(attackSpeed);
        isAttacking = false;

        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        enemyAI.canMove = true;
        stopAttack = false;
        attackStyle *= -1;

        if (attackStyle < 0)
        {
            useRayCast = true;
        }
        else
        {
            useRayCast = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        Gizmos.DrawRay(swingAttackPoint.position, Vector2.right * attackRangeSwing);
        Gizmos.DrawRay(swingAttackPoint.position, Vector2.left * attackRangeSwing);
    }

}
