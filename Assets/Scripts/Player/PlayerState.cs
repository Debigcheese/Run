using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    private WeaponHolder weaponHolder;

    [Header("Respawn")]
    private GameObject RespawnPosition;
    [SerializeField] private GameObject startPosition;
    [SerializeField] private bool useStartPosition = true;

    [Header("Crystals")]
    public int totalCrystalAmount;
    public int tempCrystalAmount;
    public int justCollected;
    public int previousCollected;

    public bool canPickup = true;

    [Space]
    [Header("Stamina")]
    public float maxStamina = 100;
    public float currentStamina;
    public Slider manaStaminaBar;

    [Space]
    [Header("Mana")]
    public float maxMana = 100;
    public float currentMana;

    [Space]
    [Header("Crosshair")]
    public GameObject mageCrosshair;


    // Start is called before the first frame update
    void Start()
    {
        weaponHolder = GetComponent<WeaponHolder>();
        totalCrystalAmount = 0;
        justCollected = 0;

        mageCrosshair.SetActive(true);

        manaStaminaBar.maxValue = maxMana;
        currentMana = maxMana;
        currentStamina = maxStamina;
        InvokeRepeating("RefillMana", 0f, .3f);
        InvokeRepeating("RefillStamina", 0f, 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        if (weaponHolder.meleeEquipped)
        {
            manaStaminaBar.value = currentStamina;
            
        }

        if (currentStamina >= maxStamina)
        {
            currentStamina = maxStamina;
        }

        if (weaponHolder.magicEquipped)
        {
            manaStaminaBar.value = currentMana;
        }

        if (currentMana >= maxMana)
        {
            currentMana = maxMana;
        }

        if (weaponHolder.justSwitchedWeapon)
        {
            currentMana = maxMana;
            currentStamina = maxStamina;
        }
  
    }
    
    public void ReduceStamina(float staminaToReduce)
    {
        currentStamina -= staminaToReduce;
    }

    private void RefillStamina()
    {
        currentStamina += 5f;
    }

    public void ReduceMana(float manaToReduce)
    {
        currentMana -= manaToReduce;
    }

    private void RefillMana()
    {
        currentMana += 5f;
    }

}
