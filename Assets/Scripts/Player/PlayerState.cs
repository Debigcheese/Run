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
    public Slider staminaBar;

    [Space]
    [Header("Mana")]
    public float maxMana = 100;
    public float currentMana;
    public Slider manaBar;

    [Space]
    [Header("Crosshair")]
    public GameObject mageCrosshair;


    // Start is called before the first frame update
    void Start()
    {
        weaponHolder = GetComponent<WeaponHolder>();
        totalCrystalAmount = 0;
        justCollected = 0;

        manaBar.maxValue = maxMana;
        currentMana = maxMana;
        staminaBar.maxValue = maxStamina;
        currentStamina = maxStamina;
        InvokeRepeating("RefillMana", 0f, .3f);
        InvokeRepeating("RefillStamina", 0f, 0.02f);
    }

    // Update is called once per frame
    void Update()
    {
        if (weaponHolder.meleeEquipped )
        {
            staminaBar.value = currentStamina;
            manaBar.gameObject.SetActive(false);
            staminaBar.gameObject.SetActive(true);
        }

        if (currentStamina >= maxStamina)
        {
            currentStamina = maxStamina;
        }

        if (weaponHolder.magicEquipped)
        {
            manaBar.value = currentMana;
            staminaBar.gameObject.SetActive(false);
            manaBar.gameObject.SetActive(true);
        }

        if (currentMana >= maxMana)
        {
            currentMana = maxMana;
        }
  
    }
    
    public void ReduceStamina(float staminaToReduce)
    {
        currentStamina -= staminaToReduce;
    }

    private void RefillStamina()
    {
        currentStamina += 1f;
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
