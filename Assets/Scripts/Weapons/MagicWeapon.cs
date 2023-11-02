using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicWeapon : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private WeaponHolder weaponHolder;
    private PlayerState playerState;
    private PlayerAttack playerAttack;
    private Animator weaponAnimator;
    public Transform projectilePoint;
    public GameObject magicProjectile;

    private Vector3 storedMousePos;

    [Space]
    [Header("Animation")]
    public float attackCounter = 1f;
    public bool isMagicAttacking;
    public bool canAttack = true;

    [Space]
    [Header("Balancing")]
    [SerializeField] private int minDmg = 2;
    [SerializeField] private int maxDmgMinusOne = 5;
    public float attackCooldown = 1f;
    public float projectileDelay = .4f;
    public float manaPerProjectile = 30;

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
        if (weaponHolder.isSwappingWeapons || playerMovement.isDashing)
        {
            isMagicAttacking = false;
            playerAttack.isAttacking = false;
            playerAttack.canAttack = true;
        }

        if (playerState.currentMana < manaPerProjectile)
        {
            playerAttack.canAttack = false;
        }
        else if(!playerAttack.isAttacking)
        {
            playerAttack.canAttack = true;
        }

        if (playerAttack.isAttacking && playerAttack.canAttack && !isMagicAttacking)
        {
            storedMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            storedMousePos.z = 0;

            StartCoroutine(ProjectileDelay());
        }
        weaponAnimator.SetBool("isMagicAttacking", playerAttack.isAttacking);
    }

    private IEnumerator ProjectileDelay()
    {
        playerState.ReduceMana(manaPerProjectile);
        isMagicAttacking = true;
        playerAttack.canAttack = false;
        yield return new WaitForSeconds(projectileDelay);
        ShootProjectile(storedMousePos);
    }

    public void ShootProjectile(Vector3 storedMousePos)
    {
        GameObject newProjectile = Instantiate(magicProjectile, projectilePoint.position, Quaternion.identity);
        Projectile projectile = newProjectile.GetComponent<Projectile>();

        int randomDamage = Random.Range(minDmg, maxDmgMinusOne);
        float totalDamage;
        if (playerAttack.critAttack)
        {
            totalDamage = randomDamage * playerAttack.critDamageMultiplier;
        }
        else
        {
            totalDamage = randomDamage;
        }
        if (playerAttack.rageEnabled)
        {
            totalDamage *= playerAttack.rageDamageMultiplier;
        }
        int roundedDamage = Mathf.RoundToInt(totalDamage);
        projectile.SetDamage(roundedDamage);
        projectile.SetMousePosition(storedMousePos);

        StartCoroutine(ShootCooldown());
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isMagicAttacking = false;
        playerAttack.isAttacking = false;
        StartCoroutine(ExtraCooldown());
    }

    private IEnumerator ExtraCooldown()
    {
        yield return new WaitForSeconds(1f);
        playerAttack.canAttack = true;
    }

}
