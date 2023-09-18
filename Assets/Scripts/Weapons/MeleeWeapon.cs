using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private Animator weaponAnimator;
    public LayerMask enemy;
    public Transform attackPoint;

    [Space]
    [Header("Animation")]
    public float attackCounter = 1f;
    public bool isMeleeAttacking = false;

    [Space]
    [Header("Balancing")]
    [SerializeField] private int minDmg = 2;
    [SerializeField] private int maxDmgMinusOne = 5;
    private int attackDamage = 20;
    public float attackRange = 0.5f;
    public float attackSpeed = 0.4f;

    [Space]
    [Header("DamageDelay")]
    public float firstSwingDmgDelay = 0.4f;
    public float SecondSwingDmgDelay = 0.1f;


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
       
        if(playerAttack.isAttacking && !isMeleeAttacking )
        {
            Attack();
        }
 
        weaponAnimator.SetBool("meleeAttack", playerAttack.isAttacking);
        weaponAnimator.SetFloat("attackCounter", attackCounter);
    }

    public void Attack()
    {
        isMeleeAttacking = true;
        attackDamage = Random.Range(minDmg, maxDmgMinusOne);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemy);
        List<GameObject> damagedEnemies = new List<GameObject>();
        foreach (Collider2D enemy in hitEnemies)
        {
            damagedEnemies.Add(enemy.gameObject);
        }
        StartCoroutine(SwingDelay(damagedEnemies));
        StartCoroutine(MeleeCD());
    }

    IEnumerator SwingDelay(List<GameObject> damagedEnemies)
    {
        if (attackCounter >= 0)
        {
            yield return new WaitForSeconds(firstSwingDmgDelay);
        }
        else
        {
            yield return new WaitForSeconds(SecondSwingDmgDelay);
        }
        
        foreach (GameObject enemy in damagedEnemies)
        {
            enemy.GetComponent<EnemyHp>().TakeDamage(attackDamage);
        }

    }

    IEnumerator MeleeCD()
    {
        yield return new WaitForSeconds(attackSpeed);
        attackCounter *= -1f;
        isMeleeAttacking = false;
        playerAttack.isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

    }
}