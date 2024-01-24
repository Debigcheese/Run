using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    // References
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private MeleeWeapon[] meleeWeapon;
    private BowWeapon[] bowWeapon;
    private MagicWeapon[] magicWeapon;

    public GameObject[] weapons;
    public GameObject currentWeapon;
    public GameObject secondWeapon;
    private GameObject previousWeapon;

    public bool isSwappingWeapons;
    public bool attackUseStamina; //meleeEquipped
    public bool attackUseMana; //magicEquipped
    public bool isUsingMelee;
    public bool isUsingRanged;
    public bool isUsingMagic;
    public bool justSwitchedWeapon;
    public bool canSwitchWeapons = true;

    [Header("Images")]
    public GameObject canvas;
    public GameObject[] currentWeaponImages;
    public GameObject[] secondWeaponImages;
    private GameObject currentWeaponImage;
    private GameObject secondWeaponImage;

    public GameObject showSecondWeaponUI;

    // Start is called before the first frame update
    void Start()
    {

        playerAttack = GetComponent<PlayerAttack>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        meleeWeapon = GetComponentsInChildren<MeleeWeapon>();
        bowWeapon = GetComponentsInChildren<BowWeapon>();
        magicWeapon = GetComponentsInChildren<MagicWeapon>();

        int currentWeaponIndex = PlayerPrefs.GetInt("CurrentWeapon", 0);
        int secondWeaponIndex = PlayerPrefs.GetInt("SecondWeapon", 0);

        currentWeapon = weapons[currentWeaponIndex];
        secondWeapon = weapons[secondWeaponIndex];

        //fill the array from empty gameobjects and sets the weapons to false;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == currentWeapon)
            {
                currentWeapon = weapons[i];
            }
            if (weapons[i] == secondWeapon)
            {
                secondWeapon = weapons[i];
            }
            else if(weapons[i] == null)
            {
                weapons[i] = weapons[0];
            }
 
            weapons[i].SetActive(false);
        }

        previousWeapon = currentWeapon;
        if (PlayerPrefs.GetInt("ShowSecondWeaponUI") == 1)
        {
            showSecondWeaponUI.SetActive(true);
        }
        else
        {
            showSecondWeaponUI.SetActive(false);
        }
        currentWeapon.SetActive(true);
        canvas.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        //images
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == currentWeapon)
            {
                currentWeaponImage = currentWeaponImages[i];
                currentWeaponImage.SetActive(true);
            }
            else
            {
                currentWeaponImages[i].SetActive(false);
            }
            if (weapons[i] == secondWeapon)
            {
                secondWeaponImage = secondWeaponImages[i];
                secondWeaponImage.SetActive(true);
            }
            else
            {
                secondWeaponImages[i].SetActive(false);
            }
        }

        //swap weapons
        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("Fire2")) && !isSwappingWeapons)
        {
            //player
            playerAttack.canAttack = true;
            playerAttack.isAttacking = false;

            //melee
            foreach (MeleeWeapon m in meleeWeapon)
            {
                m.attackCounter *= -1f;
                m.isMeleeAttacking = false;
            }
            //bow
            foreach (BowWeapon b in bowWeapon)
            {
                b.canDisableBowCharge = false;
                b.bowCharge = false;
                b.isBowAttacking = false;
                b.isBowShooting = false;
                b.count = 0;
                b.damageMultiplier = 1;
            }
            //magic
            foreach (MagicWeapon m in magicWeapon)
            {
                m.isMagicAttacking = false;
            }

            GameObject temp = currentWeapon;
            currentWeapon.SetActive(false);
            currentWeapon = secondWeapon;
            currentWeapon.SetActive(true);
            secondWeapon = temp;

            AudioManager.Instance.PlaySound("playerswitchweapons");
            StartCoroutine(SwitchWeapon());
            isSwappingWeapons = true;
        }

        if(currentWeapon == null)
        {
            currentWeapon = weapons[0];
        }
        if(currentWeapon != previousWeapon)
        {
            justSwitchedWeapon = true;
            previousWeapon = currentWeapon;
        }
        else
        {
            justSwitchedWeapon = false;
        }


        //check which energi to use;
        if (currentWeapon == weapons[10] || currentWeapon == weapons[11] || currentWeapon == weapons[12] || currentWeapon == weapons[13] || currentWeapon == weapons[14])
        {
            attackUseStamina = false;
            attackUseMana = true;
        }
        else
        {
            attackUseStamina = true;
            attackUseMana = false;
        }
        //check which type of weapon is equipped;
        if (currentWeapon == weapons[5] || currentWeapon == weapons[6] || currentWeapon == weapons[7] || currentWeapon == weapons[8] || currentWeapon == weapons[9])
        {
            isUsingMelee = false;
            isUsingRanged = true;
            isUsingMagic = false;
        }
        else if(currentWeapon == weapons[10] || currentWeapon == weapons[11] || currentWeapon == weapons[12] || currentWeapon == weapons[13] || currentWeapon == weapons[14])
        {
            isUsingMelee = false;
            isUsingRanged = false;
            isUsingMagic = true;
        }
        else
        {
            isUsingMelee = true;
            isUsingRanged = false;
            isUsingMagic = false;
        }

    }

    public int GetCurrentWeaponIndex()
    {
        int index = 0;
        for(int i = 0; i<weapons.Length; i++)
        {
            if (weapons[i] == currentWeapon)
            {
                index = i;
                return index;
            }
           
        }
        return index;
    }

    public int GetSecondaryWeaponIndex()
    {
        int index = 0;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == secondWeapon)
            {
                index = i;
                return index;
            }

        }
        return index;
    }

    public IEnumerator SwitchWeapon()
    {
        yield return new WaitForSeconds(.4f);

        isSwappingWeapons = false;
    }

}