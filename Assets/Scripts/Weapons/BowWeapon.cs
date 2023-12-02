using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowWeapon : MonoBehaviour
{
    private WeaponHolder weaponHolder;
    private PlayerMovement playerMovement;
    private PlayerState playerState;
    private PlayerAttack playerAttack;
    private Animator weaponAnimator;
    public Transform projectilePoint;
    public GameObject arrowProjectile;

    [Space]
    [Header("Animation")]
    public bool isBowAttacking = false;
    public bool isBowShooting = false;
    public bool bowCharge = false;
    public bool canDisableBowCharge = false;
    private float timer = 0f;
    private float disableBowChargeTimer = 3;

    [Space]
    [Header("Balancing")]
    [SerializeField] private int minDmg = 2;
    [SerializeField] private int maxDmgMinusOne = 5;
    public float staminaPerCharge = 70;
    public bool doubleProjectile = false;
    private float timeElapsed = 0.4f;
    private float damageMultiplier = 1.0f;
    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        weaponHolder = GetComponentInParent<WeaponHolder>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerState = GetComponentInParent<PlayerState>();
        playerAttack = GetComponentInParent<PlayerAttack>();
        weaponAnimator = transform.Find("weaponAnim").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        playerAttack.VengeanceChangeColor(GetComponentInChildren<SpriteRenderer>());
        playerState.GuardianChangeColor(GetComponentInChildren<SpriteRenderer>());

        //cancels bow
        if (weaponHolder.isSwappingWeapons || playerMovement.isDashing)
        {
            canDisableBowCharge = false;
            bowCharge = false;
            isBowAttacking = false;
            isBowShooting = false;
            playerAttack.isAttacking = false;
            playerAttack.canAttack = true;
            count = 0;
            damageMultiplier = 1;
        }

        if (playerState.currentStamina < staminaPerCharge)
        {
            playerAttack.stopAttacking = true;
            playerState.lowMana = true;
        }
        else
        {
            playerState.lowMana = false;
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (playerAttack.isAttacking)
        {
            if (mousePos.x > playerMovement.transform.position.x && playerMovement.isFacingLeft)
            {
                playerMovement.Flip();
            }
            else if (mousePos.x < playerMovement.transform.position.x && !playerMovement.isFacingLeft)
            {
                playerMovement.Flip();
            }
        }

        if (canDisableBowCharge)
        {
            timer += Time.deltaTime;
            if(disableBowChargeTimer <= timer)
            {
                canDisableBowCharge = false;
                bowCharge = false;
                isBowShooting = false;
                playerAttack.canAttack = true;
                playerAttack.isAttacking = false;
            }
        }
        else
        {
            timer = 0;
        }

        if (playerAttack.isAttacking && playerAttack.canAttack && !playerAttack.stopAttacking )
        {
            BowCharge();
        }

        if (playerAttack.isAttacking)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= .6f && count != 3)
            {
                count++;
                damageMultiplier += 0.4f+(count*0.65f);
                timeElapsed = 0f;
            }
        }

        if (bowCharge && Input.GetMouseButtonUp(0) )
        {
            BowShoot(damageMultiplier);
            count = 0;
            isBowShooting = true;
            bowCharge = false;
            canDisableBowCharge = false;
        }
        weaponAnimator.SetBool("isBowCharging", canDisableBowCharge);
        weaponAnimator.SetBool("isBowShooting", isBowShooting);
    }

    public void BowCharge()
    {
        isBowAttacking = true;
        canDisableBowCharge = true;
        playerAttack.canAttack = false;
        playerAttack.isAttacking = true;
        bowCharge = true;
        playerState.ReduceStamina(staminaPerCharge);
    }

    public void BowShoot(float damageMultiplier)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        GameObject newProjectile = Instantiate(arrowProjectile, projectilePoint.position, Quaternion.identity);
        Projectile projectile = newProjectile.GetComponent<Projectile>();

        int randomDamage = Random.Range(minDmg, maxDmgMinusOne);
        float totalDamage;
        if (playerAttack.critAttack)
        {
            totalDamage = randomDamage * damageMultiplier * playerAttack.critDamageMultiplier;
        }
        else
        {
            totalDamage = randomDamage * damageMultiplier;
        }
        if (playerAttack.rageEnabled)
        {
            totalDamage *= playerAttack.rageDamageMultiplier;
        }
        int roundedDamage = Mathf.RoundToInt(totalDamage);
        projectile.SetDamage(roundedDamage);
        projectile.SetMousePosition(mousePos);
        projectile.SetArrowDamageMultipler(damageMultiplier);

        if (doubleProjectile)
        {
            StartCoroutine(DoubleProjectile(mousePos, damageMultiplier));
        }

        StartCoroutine(FinishAnimation());
    }

    public IEnumerator DoubleProjectile(Vector2 mousePos, float damageMultiplier)
    {
        yield return new WaitForSeconds(.1f);

        GameObject newProjectile = Instantiate(arrowProjectile, projectilePoint.position, Quaternion.identity);
        Projectile projectile = newProjectile.GetComponent<Projectile>();

        int randomDamage = Random.Range(minDmg, maxDmgMinusOne);
        float totalDamage;
        if (playerAttack.critAttack)
        {
            totalDamage = randomDamage * damageMultiplier * playerAttack.critDamageMultiplier;
        }
        else
        {
            totalDamage = randomDamage * damageMultiplier;
        }
        if (playerAttack.rageEnabled)
        {
            totalDamage *= playerAttack.rageDamageMultiplier;
        }
        int roundedDamage = Mathf.RoundToInt(totalDamage);
        projectile.SetDamage(roundedDamage);
        projectile.SetMousePosition(mousePos);
        projectile.SetArrowDamageMultipler(damageMultiplier);
    }

    public IEnumerator FinishAnimation()
    {
        yield return new WaitForSeconds(.25f);
        isBowAttacking = false;
        isBowShooting = false;
        playerAttack.isAttacking = false;
        playerAttack.canAttack = true;
        count = 0;
        damageMultiplier = 1;
    }
}
