using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowWeapon : MonoBehaviour
{
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
    private bool isFlipped;

    [Space]
    [Header("Balancing")]
    [SerializeField] private int minDmg = 2;
    [SerializeField] private int maxDmgMinusOne = 5;
    public float staminaPerProjectile = 70;
    public bool doubleProjectile = false;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerState = GetComponentInParent<PlayerState>();
        playerAttack = GetComponentInParent<PlayerAttack>();
        weaponAnimator = transform.Find("weaponAnim").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState.currentStamina < staminaPerProjectile+staminaPerProjectile)
        {
            playerAttack.stopAttacking = true;
        }
        else if (!playerAttack.isAttacking)
        {
            playerAttack.stopAttacking = false;
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (playerAttack.isAttacking)
        {
            if(mousePos.x > playerMovement.transform.position.x && !isFlipped )
            {
                playerMovement.Flip();
                isFlipped = true;
            }
            else if(mousePos.x < playerMovement.transform.position.x && isFlipped)
            {
                playerMovement.Flip();
                isFlipped = false;
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
            StartCoroutine(BowCharge());
        }

        if (bowCharge && Input.GetButtonDown("Fire1") )
        {
            BowShoot();
            isBowShooting = true;
            bowCharge = false;
            canDisableBowCharge = false;
        }
        weaponAnimator.SetBool("isBowCharging", canDisableBowCharge);
        weaponAnimator.SetBool("isBowShooting", isBowShooting);
    }

    public IEnumerator BowCharge()
    {
        isBowAttacking = true;
        canDisableBowCharge = true;
        playerAttack.canAttack = false;
        playerAttack.isAttacking = true;
        yield return new WaitForSeconds(.8f);
        bowCharge = true;
    }

    public void BowShoot()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        GameObject newProjectile = Instantiate(arrowProjectile, projectilePoint.position, Quaternion.identity);
        Projectile projectile = newProjectile.GetComponent<Projectile>();

        int randomDamage = Random.Range(minDmg, maxDmgMinusOne);
        projectile.SetDamage(randomDamage);
        projectile.SetMousePosition(mousePos);

        if (doubleProjectile)
        {
            StartCoroutine(DoubleProjectile(mousePos));
        }
        playerState.ReduceStamina(staminaPerProjectile);
        StartCoroutine(FinishAnimation());
    }

    public IEnumerator DoubleProjectile(Vector2 mousePos)
    {
        yield return new WaitForSeconds(.1f);

        GameObject newProjectile = Instantiate(arrowProjectile, projectilePoint.position, Quaternion.identity);
        Projectile projectile = newProjectile.GetComponent<Projectile>();

        int randomDamage = Random.Range(minDmg, maxDmgMinusOne);
        projectile.SetDamage(randomDamage);
        projectile.SetMousePosition(mousePos);
        playerState.ReduceStamina(staminaPerProjectile);

    }

    public IEnumerator FinishAnimation()
    {
        yield return new WaitForSeconds(.35f);
        isBowAttacking = false;
        isBowShooting = false;
        playerAttack.isAttacking = false;
        playerAttack.canAttack = true;
    }
}
