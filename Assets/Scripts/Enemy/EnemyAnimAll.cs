using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimAll : MonoBehaviour
{

    private EnemyAI enemyAI;
    private Animator anim;
    private EnemyAttack enemyAttack;

    public bool isMoving;
    public bool hasSecondAttack;

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
        if (hasSecondAttack)
        {
            anim.SetBool("isSecondAttacking", enemyAttack.isSecondAttacking);
        }

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
