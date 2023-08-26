using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchAttack : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemy;
    private bool switchToLongPunch = false;
    public bool isLongPunching;
    public bool isNormalPunching;
    public bool isPunching = false;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Punch()
    {

        if (!isPunching)
        {
            isPunching = true;
            if (switchToLongPunch)
            {
                isLongPunching = true;
                isNormalPunching = false;
                switchToLongPunch = false;
            }
            else
            {
                isLongPunching = false;
                isNormalPunching = true;
                switchToLongPunch = true;
            }

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemy);
            Debug.Log("hit");
            StartCoroutine("PunchCD");
        }
  
    }

    IEnumerator PunchCD()
    {
        yield return new WaitForSeconds(.4f);
        isLongPunching = false;
        isNormalPunching = false;
        isPunching = false;
    }
    private void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        return;
          
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        
    }
}
