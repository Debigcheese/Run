using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMummy : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Animator anim;
    private EnemyAI enemyAI;
    private EnemyHp enemyHP;

    public bool isMoving;
    public bool isAttacking;
    public bool canAttack;

    public Transform attackPoint;

    public int attackDamage;
    public float attackCooldown;
    public float attackDelay;
    public float attackRange;


    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        enemyAI = GetComponent<EnemyAI>();
        enemyHP = GetComponent<EnemyHp>();
        anim = transform.Find("EnemyAnim").GetComponent<Animator>();
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isAttacking", isAttacking);

        if (enemyAI.isMoving)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange);
        if (hitPlayer.CompareTag("Player") && canAttack)
        {
            StartCoroutine("AttackDelay");
        }
 
    }

    IEnumerator AttackDelay()
    {
        enemyAI.canMove = false;
        isAttacking = true;
        canAttack = false;
        yield return new WaitForSeconds(attackDelay);
        Attack();
    }

    private void Attack()
    {
        Debug.Log("hit");
        StartCoroutine("AttackCooldown");
    }

    IEnumerator AttackCooldown()
    {
        isAttacking = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        enemyAI.canMove = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}
