using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    // References
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    public GameObject[] weapons;
    public GameObject currentWeapon;
    public GameObject secondWeapon;
    private GameObject previousWeapon;

    public bool isSwappingWeapons;
    public bool meleeEquipped;
    public bool magicEquipped;
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
            isSwappingWeapons = true;
            StartCoroutine(SwitchWeapon());
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

        //check which weapon is equipped;
        if (currentWeapon == weapons[10] || currentWeapon == weapons[11] || currentWeapon == weapons[12] || currentWeapon == weapons[13] || currentWeapon == weapons[14])
        {
            meleeEquipped = false;
            magicEquipped = true;
        }
        else
        {
            meleeEquipped = true;
            magicEquipped = false;
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
        yield return new WaitForSeconds(.1f);
        playerAttack.isAttacking = false;
        playerAttack.canAttack = true;

        GameObject temp = currentWeapon;
        currentWeapon.SetActive(false);
        currentWeapon = secondWeapon;
        currentWeapon.SetActive(true);
        secondWeapon = temp; 

        isSwappingWeapons = false;
    }

}