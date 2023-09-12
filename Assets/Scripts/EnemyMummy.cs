using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMummy : MonoBehaviour
{
    private EnemyAI enemyAI;
    private Animator anim;
    private EnemyAttack enemyAttack;

    public bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        enemyAttack = GetComponent<EnemyAttack>();
        enemyAI = GetComponent<EnemyAI>();
        anim = transform.Find("EnemyAnim").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isAttacking", enemyAttack.isAttacking);

        if (enemyAI.isMoving)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
 
    }

}
