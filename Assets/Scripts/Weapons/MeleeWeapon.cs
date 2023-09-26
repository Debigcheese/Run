using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    private WeaponHolder weaponHolder;
    private PlayerAttack playerAttack;
    private PlayerState playerState;
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
    public float staminaPerAttack = 25f;

    [Space]
    [Header("DamageDelay")]
    public float firstSwingDmgDelay = 0.4f;
    public float SecondSwingDmgDelay = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        weaponHolder = GetComponentInParent<WeaponHolder>();
        playerState = GetComponentInParent<PlayerState>();
        playerAttack = GetComponentInParent<PlayerAttack>();
        weaponAnimator = transform.Find("weaponAnim").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (weaponHolder.swapWeapons)
        {
            attackCounter *= -1f;
            isMeleeAttacking = false;
            playerAttack.isAttacking = false;
            playerAttack.canAttack = true;
        }

        if(playerState.currentStamina <= staminaPerAttack)
        {
            playerAttack.canAttack = false;
        }
        else
        {
            playerAttack.canAttack = true;
        }

        if(playerAttack.isAttacking && !isMeleeAttacking && playerAttack.canAttack)
        {
            Attack();
        }
 
        weaponAnimator.SetBool("meleeAttack", playerAttack.isAttacking);
        weaponAnimator.SetFloat("attackCounter", attackCounter);
    }

    public void Attack()
    {
        playerAttack.canAttack = false;
        playerState.ReduceStamina(staminaPerAttack);
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
        playerAttack.canAttack = true;
    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

    }
}
