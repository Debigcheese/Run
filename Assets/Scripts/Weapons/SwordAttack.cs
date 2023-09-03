using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;
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
    public float attackRange = 0.5f;
    private int attackDamage = 20;
    [SerializeField] private int minDmg = 2;
    [SerializeField] private int maxDmg = 5;
    public float swordCooldown = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerAttack = GetComponentInParent<PlayerAttack>();
        weaponAnimator = transform.Find("weaponAnim").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerAttack.isAttacking)
        {
            Attack();
        }
        weaponAnimator.SetBool("isSwordAttacking", isSwordAttacking);
        weaponAnimator.SetFloat("attackCounter", attackCounter);
    }

    public void Attack()
    {
        if (!isSwordAttacking)
        {
            isSwordAttacking = true;
            attackCounter *= -1;

            attackDamage = Random.Range(minDmg, maxDmg);

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemy);
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<EnemyHp>().TakeDamage(attackDamage);
                Debug.Log(attackDamage);
            }

            StartCoroutine("SwordCD");
        }
    }

    IEnumerator SwordCD()
    {
        yield return new WaitForSeconds(swordCooldown);
        isSwordAttacking = false;
        playerAttack.isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

    }
}
