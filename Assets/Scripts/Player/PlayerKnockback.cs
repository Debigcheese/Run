using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Collider2D collider;

    public float KBForce;
    public float KBCounter;
    public float KBTotalTime;
    public bool KnockFromLeft;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (KnockFromLeft)
            {
                
            }
        }
    }
}
