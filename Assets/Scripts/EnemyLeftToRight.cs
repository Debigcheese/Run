using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLeftToRight : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    private float Direction;
    private Rigidbody2D rb;

    [Space]
    [Header("checks")]
    public LayerMask layer;
    public Transform groundCheck;
    public Transform wallCheck;
    public float groundRadius;
    private bool onGround;
    private bool onWall;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Direction = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.OverlapCircle(groundCheck.position, groundRadius, layer);
        onWall = Physics2D.OverlapCircle(wallCheck.position, groundRadius, layer);

        rb.velocity = new Vector2(speed * Direction, rb.velocity.y);
        
        if (!onGround || onWall)
        {
            Flip();
        }
        
    }

    private void Flip()
    {
        Direction *= -1f;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        Gizmos.DrawWireSphere(wallCheck.position, groundRadius);
    }

}
