using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private Animator baseAnimator;
    public LayerMask enemy;
    public Transform attackPoint;

    [Space]
    [Header("Animation")]
    private float attackCounter = 1f;
    public bool isPunching = false;

    [Space]
    [Header("Balancing")]
    public float attackRange = 0.5f;
    private int attackDamage = 20;
    [SerializeField] private int minDmg = 2;
    [SerializeField] private int maxDmg = 5;
    public float punchCooldown = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerAttack = GetComponentInParent<PlayerAttack>();
        baseAnimator = transform.Find("Base").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerAttack.isAttacking)
        {
            Attack();
        }
        baseAnimator.SetBool("isPunching", isPunching);
        baseAnimator.SetFloat("attackCounter", attackCounter);
    }

    public void Attack()
    {
        if (!isPunching)
        {
            isPunching = true;
            attackCounter *= -1;
           
            attackDamage = Random.Range(minDmg, maxDmg);

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemy);
            foreach(Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
                Debug.Log(attackDamage);
            }
            
            StartCoroutine("PunchCD");
        }
    }

    IEnumerator PunchCD()
    {
        yield return new WaitForSeconds(punchCooldown);
        isPunching = false;
        playerAttack.isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        return;
          
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        
    }
}
