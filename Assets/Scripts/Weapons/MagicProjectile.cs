using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    public LayerMask enemyLayer;
    public LayerMask groundLayer;
    private Vector3 mousePos;

    public float force;
    public float radiusCollider;
    private int damage = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        Vector3 direction = (GetMousePosition() - transform.position).normalized;
        Vector3 rotation = transform.position - GetMousePosition();
        rb.velocity = new Vector2(direction.x, direction.y) * force;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot );

        StartCoroutine(TimeToDestroy());
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radiusCollider, enemyLayer);
            List<GameObject> hurtEnemies = new List<GameObject>();
            foreach (Collider2D enemy in hitEnemies)
            {
                if (!hurtEnemies.Contains(enemy.gameObject))
                {
                    enemy.GetComponent<EnemyHp>().TakeDamage(getDamage());
                }
                hurtEnemies.Add(enemy.gameObject);
            }
            Explode();
        }
        else if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            // Destroy the projectile when it collides with the ground layer
            Explode();
        }
    }

    public void Explode()
    {
        Destroy(this.gameObject);
    }

    private IEnumerator TimeToDestroy()
    {
        yield return new WaitForSeconds(1.6f);
        Explode();
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

    public int getDamage()
    {
        return damage;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radiusCollider);
    }


}
