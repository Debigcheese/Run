using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerState playerState;
    private EnemyAI enemyAI;
    private EnemyHp enemyHp;
    public Transform attackPoint;

    [Header("Booleans")]
    public bool isAttacking;
    public bool inRange;
    public bool canAttack;

    [Header("Balancing")]
    public int attackDamage;
    public float attackSpeed;
    public float attackDmgDelay;
    public float attackRange;

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
        InvokeRepeating("CheckForPlayer", 0f, 0.6f);

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
        foreach (Collider2D hitplayer in hitPlayers) 
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

    private void CheckForPlayer()
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
        enemyAI.canMove = true;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}
