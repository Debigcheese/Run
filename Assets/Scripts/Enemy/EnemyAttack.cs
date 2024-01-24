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
    public Transform secondAttackPoint;

    [Header("Booleans")]
    public bool useRayCast = false;
    public bool isAttacking;
    public bool inRange;
    public bool canAttack = true;

    [Header("Balancing")]
    public int attackDamage;
    public float attackSpeed;
    public float attackDmgDelay;
    public float attackRange;
    public float attackCooldown;

    [Header("Second Attack")]
    public bool turnOnSecondAttack = false;
    public bool isSecondAttacking = false;
    public bool useRayCastSecondAttack = false;
    public float secondAttackSpeed;
    public float secondAttackDmgDelay;
    public float secondAttackRange;

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

        if ((!useRayCast && !isSecondAttacking) || (!useRayCastSecondAttack && isSecondAttacking) )
        {
            CheckForPlayerCircle();
        }
        else if((useRayCast && !isSecondAttacking) || (useRayCastSecondAttack && isSecondAttacking))
        {
            CheckForPlayerRayCast();
        }
    }

    private void CheckForPlayerCircle()
    {
        if (!useRayCastSecondAttack)
        {
            hitPlayersCollider = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
        }
        else
        {
            if(secondAttackPoint != null)
            {
                hitPlayersCollider = Physics2D.OverlapCircleAll(secondAttackPoint.position, secondAttackRange);
            }
        }


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
            if (!useRayCastSecondAttack)
            {
                hitPlayersRayCollider = Physics2D.RaycastAll(attackPoint.position, Vector2.right, attackRange);
            }
            else
            {
                if (secondAttackPoint != null)
                {
                    hitPlayersRayCollider = Physics2D.RaycastAll(secondAttackPoint.position, Vector2.right, secondAttackRange);
                }

            }

        }
        else
        {
            if (!useRayCastSecondAttack)
            {
                hitPlayersRayCollider = Physics2D.RaycastAll(attackPoint.position, Vector2.left, attackRange);
            }
            else
            {
                if (secondAttackPoint != null)
                {
                    hitPlayersRayCollider = Physics2D.RaycastAll(secondAttackPoint.position, Vector2.left, secondAttackRange);
                }
            }
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
        if (!isRangedAttacker)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        if (!isSecondAttacking)
        {
            yield return new WaitForSeconds(attackDmgDelay);
        }
        else
        {
            yield return new WaitForSeconds(secondAttackDmgDelay);
        }

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
            StartCoroutine(AttackSpeed());

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
        if (!isSecondAttacking)
        {
            yield return new WaitForSeconds(attackSpeed);
        }
        else
        {
            yield return new WaitForSeconds(secondAttackSpeed);
        }

        isAttacking = false;
        enemyAI.canMove = true;
        canAttack = false;

        StartCoroutine(attackCD());
        
    }

    IEnumerator attackCD()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;

        if (!isSecondAttacking && turnOnSecondAttack)
        {
            isSecondAttacking = true;
        }
        else if (isSecondAttacking && turnOnSecondAttack)
        {
            isSecondAttacking = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        if(secondAttackPoint != null)
        {
            Gizmos.DrawWireSphere(secondAttackPoint.position, secondAttackRange);
        }
        if (useRayCast)
        {
            Gizmos.DrawRay(attackPoint.position, Vector2.right * attackRange);
            Gizmos.DrawRay(attackPoint.position, Vector2.left * attackRange);
        }

    }



}
