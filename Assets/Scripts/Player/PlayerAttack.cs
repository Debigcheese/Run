using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    private WeaponHolder weaponHolder;
    public bool isAttacking = false;
    public float AttackMoveSpeed;
    public bool checkMousePositionX;
    public bool stopFlip = false;
    public bool canAttackFromKnockback = true;
    public bool canAttack = true;
    public bool stopAttacking = false;
    public bool dialogueStopAttack = false;

    [Header("CritAttack")]
    public bool critAttack;
    public float critChance = 4f;
    public float critDamageMultiplier = 2f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        weaponHolder = GetComponent<WeaponHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        if (Input.GetButtonDown("Fire1") && !playerMovement.isWallSliding && !playerMovement.isClimbingLedge && !isAttacking && canAttackFromKnockback && canAttack && !stopAttacking && !dialogueStopAttack)
        {
            float randomValue = Random.Range(0f, 100f);
            if (randomValue <= critChance)
            {
                critAttack = true;
            }
            else
            {
                critAttack = false;
            }
            isAttacking = true;

            if (mousePosition.x > transform.position.x && playerMovement.isFacingLeft || mousePosition.x < transform.position.x && !playerMovement.isFacingLeft)
            {
                stopFlip = true;
                playerMovement.Flip();
            }

        }

        if (Input.GetButtonDown("Fire1") && !playerMovement.isWallSliding && !playerMovement.isClimbingLedge && !isAttacking && canAttackFromKnockback && canAttack && !stopAttacking && !dialogueStopAttack)
        {
            float randomValue = Random.Range(0f, 100f);
            if (randomValue <= critChance)
            {
                critAttack = true;
            }
            else
            {
                critAttack = false;
            }
            isAttacking = true;
        }

        //cant attack when ledgeclimb,wallslide,walljump,hurt
        if (playerMovement.isClimbingLedge || playerMovement.isWallSliding || playerMovement.isWallJumping)
        {
            isAttacking = false;
        }
        //if attacking and not in air, slow ms
        if (isAttacking && !playerMovement.isFalling && !playerMovement.isDashing)
        {
            rb.velocity = new Vector2(playerMovement.moveDirection * AttackMoveSpeed, rb.velocity.y);
        }
        //stop flip character when attacking
        if (!isAttacking)
        {
            stopFlip = false;
        }
        if (!isAttacking)
        {
            stopAttacking = false;
        }

    }

}
