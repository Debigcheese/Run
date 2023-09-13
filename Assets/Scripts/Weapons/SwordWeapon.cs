using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : MonoBehaviour
{
    private PlayerAttack playerAttack;
    private Animator weaponAnimator;
    public LayerMask enemy;
    public Transform attackPoint;

    [Space]
    [Header("Animation")]
    private float attackCounter = 1f;
    public bool isSwordAttacking = false;

    [Space]
    [Header("Balancing")]
    [SerializeField] private int minDmg = 2;
    [SerializeField] private int maxDmgMinusOne = 5;
    private int attackDamage = 20;
    public float attackRange = 0.5f;
    public float attackSpeed = 0.4f;
    public float secondSwingDmgDelay = 0.4f;
    

    // Start is called before the first frame update
    void Start()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
        weaponAnimator = transform.Find("weaponAnim").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerAttack.isAttacking && !isSwordAttacking && attackCounter <= 0)
        {
            Attack();
        }
        else if(playerAttack.isAttacking && !isSwordAttacking && attackCounter >= 0)
        {
            StartCoroutine("AttackDelay");
        }
        weaponAnimator.SetBool("isSwordAttacking", isSwordAttacking);
        weaponAnimator.SetFloat("attackCounter", attackCounter);
    }

    public void Attack()
    {
        isSwordAttacking = true;
        attackDamage = Random.Range(minDmg, maxDmgMinusOne);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemy);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHp>().TakeDamage(attackDamage);
        }
        StartCoroutine("SwordCD");
    }

    IEnumerator AttackDelay()
    {
        isSwordAttacking = true;
        yield return new WaitForSeconds(secondSwingDmgDelay);
        Attack();
    }

    IEnumerator SwordCD()
    {
        yield return new WaitForSeconds(attackSpeed);
        isSwordAttacking = false;
        playerAttack.isAttacking = false;
        attackCounter *= -1;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

    }
}
