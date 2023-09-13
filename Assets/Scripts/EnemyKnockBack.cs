using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnockBack : MonoBehaviour
{
    private Rigidbody2D rb;
    private EnemyAI enemyAI;
    private EnemyHp enemyHp;
    private SwordWeapon swordWeapon;

    [Space]
    [Header("Knockback")]
    public float KBForceX;
    public float KBForceY;
    public float meleeKBForceX = 3f;
    public float meleeKBForceY = 3f;
    public float KBCounter;
    public float KBTotalTime;
    public bool KnockFromRight;
    public bool firstSwing;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAI = GetComponent<EnemyAI>();
        enemyHp = GetComponent<EnemyHp>();
        swordWeapon = FindObjectOfType<SwordWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (KBCounter <= 0)
        {
            enemyAI.canMove = true;
        }
        else
        {
            enemyAI.canMove = false;

            SwordWeaponKnockback();
            

            KBCounter -= Time.deltaTime;

            //if (KnockFromRight == true)
            //{
            //    rb.velocity = new Vector2(-KBForceX, KBForceY);
            //}
            //if (KnockFromRight == false)
            //{
            //    rb.velocity = new Vector2(KBForceX, KBForceY);
            //}
        }
    }

    private void SwordWeaponKnockback()
    {

        if (firstSwing)
        {
            Debug.Log("first");
            if (KnockFromRight == true)
            {
                rb.velocity = new Vector2(-1.5f, 0);
            }
            if (KnockFromRight == false)
            {
                rb.velocity = new Vector2(1.5f, 0);
            }
        }
        else 
        {
            Debug.Log("second");
            if (KnockFromRight == true)
            {
                rb.velocity = new Vector2(-meleeKBForceX, meleeKBForceY);
            }
            if (KnockFromRight == false)
            {
                rb.velocity = new Vector2(meleeKBForceX, meleeKBForceY);
            }
        }
    }
}
