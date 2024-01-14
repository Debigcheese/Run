using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerState playerState;
    private Rigidbody2D rb;
    private Collider2D projectileCollider;

    public Animator anim;
    public LayerMask playerLayer;
    public LayerMask groundLayer;

    [Header("balancing")]
    public int projectileDamage;
    public float force;
    public float radiusCollider;
    public Vector3 radiusColliderOffset = new Vector3();
    public float rotationOff;

    [Header("SFX")]
    public string explosionSFX;


    // Start is called before the first frame update
    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerState = FindObjectOfType<PlayerState>();
        rb = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();

        Vector2 direction = (playerMovement.transform.position - transform.position).normalized;
        Vector2 rotation = playerMovement.transform.position - transform.position;

        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + rotationOff);

        if (direction.x < 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipY = true;
        }

        rb.velocity = new Vector2(direction.x, direction.y) * force;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerState.TakeDamage(projectileDamage);
            ApplyPlayerKnockBack();
            anim.SetBool("ActivateExplosion", true);
            projectileCollider.enabled = false;
            rb.velocity = Vector2.zero;
            AudioManager.Instance.PlaySound(explosionSFX);
        }
        else if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(transform.position + radiusColliderOffset, radiusCollider, playerLayer);

            foreach(Collider2D hit in hitPlayer)
            {
                if (hit.CompareTag("Player"))
                {
                    playerState.TakeDamage(projectileDamage);
                    ApplyPlayerKnockBack();
                }
            }
            rb.velocity = Vector2.zero;
            projectileCollider.enabled = false;
            anim.SetBool("ActivateExplosion", true);
            AudioManager.Instance.PlaySound(explosionSFX);
        }
    }

    public void SetProjectileDamage(int damage)
    {
        projectileDamage = damage;
    }

    public void ApplyPlayerKnockBack()
    {
        playerMovement.KBCounter = playerMovement.KBTotalTime;
        if (playerMovement.transform.position.x <= transform.position.x)
        {
            playerMovement.KnockFromRight = true;
        }
        if (playerMovement.transform.position.x >= transform.position.x)
        {
            playerMovement.KnockFromRight = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + radiusColliderOffset, radiusCollider);
    }

}
