using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    public PlayerMovement playerMovement;
    private Collision coll;
    [SerializeField] private float radius;
    public LayerMask layer;

    public bool canDetect;
    public Vector2 dir = Vector2.right;
    public float rayLength;


    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        coll = GetComponentInParent<Collision>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canDetect && coll.onWall)
        {
            if (playerMovement.isFacingLeft)
            {
                 dir = Vector2.left;
            }
            else
            {
                 dir = Vector2.right;
            }

            playerMovement.ledgeDetected = Physics2D.Raycast(transform.position, dir , rayLength,  layer);
        }
        else
        {
            playerMovement.ledgeDetected = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            canDetect = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            canDetect = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (playerMovement.isFacingLeft)
        {
            Gizmos.DrawRay(transform.position, Vector2.left* rayLength);
        }
        else
        {
            Gizmos.DrawRay(transform.position, Vector2.right * rayLength);
        }

    }
}
