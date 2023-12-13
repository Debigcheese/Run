using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator anim;
    private EnemyHp enemyHp;
    private CrystalDropper crystalDropper;
    private int currentHealth;

    [Header("Loot To Drop")]
    public bool dropCrystalSwitch = false;
    public bool dropObjSwitch = false;
    public GameObject objToDrop;

    [Space]
    [Header("Anim")]
    public GameObject chestOpenAnim;
    public bool chestIsHurt;
    public bool chestIsOpen;
    private bool justHurt;
    private bool justDied;
    public bool isWaveChest;
    public bool showWaveChest;
    private int hitCounter;

    [Space]
    [Header("SFX")]
    public string[] chestHitSFX;
    public string chestOpenSFX;

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
            AudioManager.Instance.PlaySound(chestHitSFX[hitCounter]);
            if(hitCounter != 3)
            {
                hitCounter++;
            }
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
            AudioManager.Instance.PlaySound(chestOpenSFX);
            justDied = true;
            chestIsOpen = true;
            Instantiate(chestOpenAnim, transform.position, Quaternion.identity);

            if(dropCrystalSwitch)
            {
                crystalDropper.DropCrystal(enemyHp.crystalDropAmount);
            }
            if (dropObjSwitch && objToDrop != null)
            {
                Instantiate(objToDrop, transform.position, Quaternion.identity);
            }

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
