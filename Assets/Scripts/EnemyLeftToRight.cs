using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLeftToRight : MonoBehaviour
{
    public LayerMask layer;
    public Transform groundCheck;
    public float groundRadius;
    public bool onGround;

    public float movement;
    public float Direction;
    private Rigidbody2D rb;
    

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
        rb.velocity = new Vector2(movement * Direction, rb.velocity.y);
        
        if (!onGround)
        {
            Turn();
        }
    }

    private void Turn()
    {
        Direction *= -1f;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }

}
