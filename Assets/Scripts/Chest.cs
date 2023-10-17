using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator anim;
    private EnemyHp enemyHp;
    private CrystalDropper crystalDropper;
    public int currentHealth;
    public bool chestIsHurt;
    public bool chestIsOpen;
    private bool justHurt;
    private bool justDied;
    public GameObject chestOpenAnim;
    public bool isWaveChest;
    public bool showWaveChest;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        enemyHp = GetComponent<EnemyHp>();
        crystalDropper = GetComponentInChildren<CrystalDropper>();
        currentHealth = enemyHp.currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("ChestHurt", chestIsHurt);
        anim.SetBool("isDead", chestIsOpen);

        if (currentHealth != enemyHp.currentHealth && !justDied)
        {
            StartCoroutine("IsHurt");
            currentHealth = enemyHp.currentHealth;
        }
        else
        {
            if (!justHurt)
            {
                chestIsHurt = false;
            }

        }

        if (enemyHp.currentHealth <= 0 && !justDied )
        {
            justDied = true;
            chestIsOpen = true;
            Instantiate(chestOpenAnim, transform.position, Quaternion.identity);
            crystalDropper.DropCrystal(enemyHp.crystalDropAmount);
            Destroy(this.gameObject);
        }

    }

    private IEnumerator IsHurt()
    {
        justHurt = true;
        chestIsHurt = true;
        yield return new WaitForSeconds(.25f);
        justHurt = false;
        chestIsHurt = false;
    }
}
