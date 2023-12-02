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
            playerMovement.ledgeDetected = Physics2D.OverlapCircle(transform.position, radius, layer);
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
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
