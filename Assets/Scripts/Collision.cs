using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{

    [Space]

    [Header("Collision")]
    [SerializeField] private LayerMask layer;
    [SerializeField] private Transform groundcheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float groundRadius;
    [SerializeField] private float wallRadius;

    public bool onGround;
    public bool onWall;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.OverlapCircle(groundcheck.position, groundRadius, layer);
        onWall = Physics2D.OverlapCircle(wallCheck.position, wallRadius, layer);
    }

    private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, wallRadius);
            Gizmos.DrawWireSphere(groundcheck.position, groundRadius);
        }

}
