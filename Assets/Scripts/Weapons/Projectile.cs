using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    public LayerMask enemyLayer;
    public LayerMask groundLayer;
    private SpriteRenderer spriteRenderer;
    private Vector3 mousePos;
    public GameObject explosionPrefab;
    public bool hitGroundDontExplode;
    private bool hitGround;
    private string hitSFX;

    [Header("balancing")]
    public float force;
    public float radiusCollider;
    public Vector3 radiusColliderOffset = new Vector3();
    private int damage = 0;

    [Header("Rotation")]
    public float rotationOff;
    public bool flipY;

    [Header("Arrow")]
    public bool arrowProjectile;
    public float arrowGravity = 0.01f;
    public float arrowGravityDelay = 2f;
    private float timer;
    private float dmgMultiplier;
    public bool arrowGoThroughEnemies;
    public bool hitEnemy;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        Vector2 direction = (GetMousePosition() - transform.position).normalized;
        Vector2 rotation = transform.position - GetMousePosition();

        if (direction.x < 0 && flipY)
        {
            spriteRenderer.flipY = true;
        }

        if (GetArrowDamageMultipler() == 1f)
        {
            force -= 6f;
        }
        if (GetArrowDamageMultipler() <= 1.25f)
        {
            force -= 3f;
        }
        if (GetArrowDamageMultipler() <= 1.75f)
        {
            force -= 1f;
        }

        rb.velocity = new Vector2(direction.x, direction.y) * force;

        
        if (!arrowProjectile)
        {
            float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rot + rotationOff);
        }

        StartCoroutine(TimeToDestroy());
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (arrowProjectile && timer >= arrowGravityDelay)
        {
            if (GetArrowDamageMultipler() == 1f)
            {
                rb.gravityScale += arrowGravity * 4f * Time.deltaTime;
            }
            if (GetArrowDamageMultipler() <= 1.25f)
            {
                rb.gravityScale += arrowGravity * 2.5f * Time.deltaTime;
            }
            if (GetArrowDamageMultipler() <= 1.75f)
            {
                rb.gravityScale += arrowGravity * 1.125f * Time.deltaTime;
            }
            else if(GetArrowDamageMultipler() >= 2.5f)
            {
                rb.gravityScale += arrowGravity * 1.025f * (Time.deltaTime/2);
            }
            if (arrowProjectile && (!(hitGround || hitEnemy)))
            {
                float rot = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, rot + rotationOff);
            }

        }

        if(arrowProjectile && !arrowGoThroughEnemies && (hitGround || hitEnemy))
        {
            Collider2D collider = GetComponent<Collider2D>();
            collider.isTrigger = false;
            int layer = LayerMask.NameToLayer("ArrowCollidedWithEnemy");
            gameObject.layer = layer;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        if (rb.velocity.y > 0 && hitGround && arrowProjectile)
        {
            rb.simulated = false;
        }
        if (rb.velocity.y < 0 && hitGround && arrowProjectile && arrowGoThroughEnemies)
        {
            rb.simulated = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            HashSet<GameObject> hurtEnemies = new HashSet<GameObject>();
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position + radiusColliderOffset, radiusCollider, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                if (!hurtEnemies.Contains(enemy.gameObject) && enemy != null)
                {
                    EnemyHp enemyHp = enemy.GetComponent<EnemyHp>();
                    if(enemyHp != null && !arrowProjectile)
                    {
                        enemyHp.TakeDamage(GetDamage());
                        hurtEnemies.Add(enemy.gameObject);
                        hitEnemy = true;
                    }
                    else
                    {
                        if (!hitEnemy && arrowProjectile)
                        {
                            enemyHp.TakeDamage(GetDamage());
                            hurtEnemies.Add(enemy.gameObject);
                            hitEnemy = true;
                        }
                    }

                }

            }

            BeforeExplosion();
            AudioManager.Instance.PlaySound(GetHitSound());
        }
        else if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radiusCollider, enemyLayer);
            hitGround = true;
            foreach (Collider2D enemy in hitEnemies)
            {
                if(enemy != null && !arrowProjectile)
                {
                    enemy.GetComponent<EnemyHp>().TakeDamage(GetDamage());
                }

            }

            if (!arrowProjectile)
            {
                BeforeExplosion();
            }
            // Destroy the projectile when it collides with the ground layer
            if(!(transform.position.x > FindObjectOfType<PlayerMovement>().transform.position.x + 25 || transform.position.x < FindObjectOfType<PlayerMovement>().transform.position.x - 25))
            {
                AudioManager.Instance.PlaySound(GetHitSound());
            }

        }
    }

    private void BeforeExplosion()
    {
        Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false;
        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        renderer.enabled = false;
        rb.velocity = Vector2.zero;


        if(hitGroundDontExplode && hitGround)
        {
           
        }
        else
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        
        StartCoroutine(Explode());
    }

    public IEnumerator Explode()
    {
        yield return new WaitForSeconds(1.6f);
        Destroy(this.gameObject);
    }

    private IEnumerator TimeToDestroy()
    {
        yield return new WaitForSeconds(3f);
        BeforeExplosion();
    }

    public void SetMousePosition(Vector3 mousePOS)
    {
        mousePos = mousePOS;
    }
    public Vector3 GetMousePosition()
    {
        return mousePos;
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    public int GetDamage()
    {
        return damage;
    }

    public void SetHitSound(string hitSound)
    {
        hitSFX = hitSound;
    }

    public string GetHitSound()
    {
        return hitSFX;
    }

    public void SetArrowDamageMultipler(float damageMultiplier)
    {
        dmgMultiplier = damageMultiplier;
    }
    public float GetArrowDamageMultipler()
    {
        return dmgMultiplier;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + radiusColliderOffset, radiusCollider);
    }


}
