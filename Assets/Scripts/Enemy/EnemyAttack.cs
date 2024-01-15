using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerState playerState;
    private EnemyAI enemyAI;
    private EnemyHp enemyHp;
    private Collider2D[] hitPlayersCollider;
    private RaycastHit2D[] hitPlayersRayCollider;
    public Transform attackPoint;
    public bool useRayCast = false;

    [Header("Booleans")]
    public bool isAttacking;
    public bool inRange;
    public bool canAttack = true;

    [Header("Balancing")]
    public int attackDamage;
    public float attackSpeed;
    public float attackDmgDelay;
    public float attackRange;
    public float attackCooldown;

    [Header("Ranged Enemy")]
    public bool isRangedAttacker = false;
    public float rangedProjectileDelay;
    public Transform projectileAttackPoint;
    public GameObject projectile;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        playerState = FindAnyObjectByType<PlayerState>();
        enemyAI = GetComponent<EnemyAI>();
        enemyHp = GetComponent<EnemyHp>();
        canAttack = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        InvokeRepeating("AttackIfInRange", 0f, 0.6f);

        if (!useRayCast)
        {
            CheckForPlayerCircle();
        }
        else
        {
            CheckForPlayerRayCast();
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
            hitPlayersRayCollider = Physics2D.RaycastAll(attackPoint.position, Vector2.right, attackRange);
        }
        else
        {
            hitPlayersRayCollider = Physics2D.RaycastAll(attackPoint.position, Vector2.left, attackRange);
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
        if (inRange)
        {
            enemyAI.canMove = false;
            if (!isAttacking )
            {
                isAttacking = true;
                StartCoroutine("AttackDmgDelay");
            }
        }
    }

    IEnumerator AttackDmgDelay()
    {
        yield return new WaitForSeconds(attackDmgDelay);
        Attack();
    }

    private void Attack()
    {
        if (!isRangedAttacker)
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
        else
        {
            StartCoroutine(RangedAttack());
        }
        
    }

    private IEnumerator RangedAttack()
    {
        if (inRange && canAttack)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            yield return new WaitForSeconds(rangedProjectileDelay);
            if (!enemyHp.isDead)
            {
                GameObject newProjectile = Instantiate(projectile, projectileAttackPoint.position, Quaternion.identity);
                EnemyProjectile enemyProjectile = newProjectile.GetComponent<EnemyProjectile>();

                enemyProjectile.SetProjectileDamage(attackDamage);
            }
        }
        canAttack = false;
        StartCoroutine(AttackSpeed());


        if (enemyHp.enemySFX.EnemyAttackSFX != null)
        {
            enemyHp.enemySFX.PlayEnemySound(enemyHp.enemySFX.EnemyAttackSFX);
        }
    }

    IEnumerator AttackSpeed()
    {
        yield return new WaitForSeconds(attackSpeed);
        isAttacking = false;
        enemyAI.canMove = true;
        canAttack = false;

        StartCoroutine(attackCD());
        
    }

    IEnumerator attackCD()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        if (useRayCast)
        {
            Gizmos.DrawRay(attackPoint.position, Vector2.right * attackRange);
            Gizmos.DrawRay(attackPoint.position, Vector2.left * attackRange);
        }

    }



}
