using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private EnemyAI enemyAI;
    private EnemyHp enemyHP;
    public Transform attackPoint;

    [Header("Booleans")]
    public bool isAttacking;
    public bool canAttack;

    [Header("Balancing")]
    public int attackDamage;
    public float attackSpeed;
    public float attackDmgDelay;
    public float attackRange;

    // Start is called before the first frame update
    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        enemyHP = GetComponent<EnemyHp>();
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {

        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange);
        if (hitPlayer != null && hitPlayer.CompareTag("Player"))
        {
            Debug.Log($"Detected GameObject with tag: {hitPlayer.tag}");
            enemyAI.canMove = false;
            if (!isAttacking)
            {
                
                isAttacking = true;
                canAttack = false;
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
        
        StartCoroutine("AttackSpeed");
    }

    IEnumerator AttackSpeed()
    {
        yield return new WaitForSeconds(attackSpeed);
        isAttacking = false;
        canAttack = true;
        enemyAI.canMove = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}
