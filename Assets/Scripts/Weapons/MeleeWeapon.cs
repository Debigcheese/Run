using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private WeaponHolder weaponHolder;
    private PlayerAttack playerAttack;
    private PlayerState playerState;
    private Animator weaponAnimator;
    public LayerMask enemy;
    public Transform attackPoint;
    public string weaponAttackSFX;

    [Space]
    [Header("Animation")]
    public float attackCounter = 1f;
    public bool isMeleeAttacking = false;
    private float attackCounterTimer =0f;

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
        playerMovement = GetComponentInParent<PlayerMovement>();
        weaponHolder = GetComponentInParent<WeaponHolder>();
        playerState = GetComponentInParent<PlayerState>();
        playerAttack = GetComponentInParent<PlayerAttack>();
        weaponAnimator = transform.Find("weaponAnim").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        playerAttack.VengeanceChangeColor(GetComponentInChildren<SpriteRenderer>());
        playerState.GuardianChangeColor(GetComponentInChildren<SpriteRenderer>());

        //cancels melee
        //if (weaponHolder.isSwappingWeapons || playerMovement.isDashing)
        //{
        //    attackCounter *= -1f;
        //    isMeleeAttacking = false;
        //    playerAttack.isAttacking = false;
        //}

        if(playerState.currentStamina <= staminaPerAttack)
        {
            playerAttack.insufficientEnergyAttack = true;
            playerState.lowMana = true;
        }
        else
        {
            playerState.lowMana = false;
            playerAttack.insufficientEnergyAttack = false;
        }

        if(playerAttack.isAttacking && !isMeleeAttacking && playerAttack.canAttack)
        {
            Attack();
        }

        if (!playerAttack.isAttacking )
        {
            attackCounterTimer += Time.deltaTime;
            float delay = 1.2f;
            if(attackCounterTimer >= delay)
            {
                attackCounter = 1f;
                attackCounterTimer = 0f;
            }
        }
        else
        {
            attackCounterTimer = 0;
        }

        weaponAnimator.SetBool("meleeAttack", playerAttack.isAttacking);
        weaponAnimator.SetFloat("attackCounter", attackCounter);
    }

    public void Attack()
    {
        attackCounterTimer = 0f;
        playerAttack.canAttack = false;
        playerState.ReduceStamina(staminaPerAttack);
        isMeleeAttacking = true;
        AudioManager.Instance.PlaySound(weaponAttackSFX);
        StartCoroutine(SwingDelay());
        StartCoroutine(MeleeCD());
    }

    IEnumerator SwingDelay()
    {
        if (attackCounter >= 0)
        {
            yield return new WaitForSeconds(firstSwingDmgDelay);
        }
        else
        {
            yield return new WaitForSeconds(SecondSwingDmgDelay);
        }

        attackDamage = Random.Range(minDmg, maxDmgMinusOne);
        float totalDamage;
        if (playerAttack.critAttack)
        {
            totalDamage = attackDamage * playerAttack.critDamageMultiplier;
        }
        else
        {
            totalDamage = attackDamage;
        }
        if (playerAttack.rageEnabled)
        {
            totalDamage *= playerAttack.rageDamageMultiplier;
        }
        int roundedDamage = Mathf.RoundToInt(totalDamage);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemy);
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHp enemyHp = enemy.GetComponent<EnemyHp>();
            if (enemyHp != null)
            {
                enemyHp.TakeDamage(roundedDamage);
            }
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
