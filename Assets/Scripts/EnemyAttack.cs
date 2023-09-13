using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerHP playerHP;
    private EnemyAI enemyAI;
    public Transform attackPoint;

    [Header("Booleans")]
    public bool isAttacking;
    public bool inRange;

    [Header("Balancing")]
    public int attackDamage;
    public float attackSpeed;
    public float attackDmgDelay;
    public float attackRange;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        playerHP = FindAnyObjectByType<PlayerHP>();
        enemyAI = GetComponent<EnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        InvokeRepeating("CheckForPlayer", 0f, 0.3f);

        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange);
        if (hitPlayer != null && hitPlayer.CompareTag("Player"))
        {
            inRange = true;

            
        }
        else
        {
            inRange = false;
        }
    }

    private void CheckForPlayer()
    {
        if (inRange)
        {
            enemyAI.canMove = false;
            if (!isAttacking)
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
            playerHP.takeDamage(attackDamage);

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
