using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    private PlayerMovement pMovement;
    private PlayerAttack pAttack;
    private WeaponHolder weaponHolder;
    private DamageFlash damageFlash;

    [Header("Respawn")]
    [SerializeField] private GameObject startPosition;

    [Space]
    [Header("Health")]
    public Slider HealthBar;
    public Slider easeHealthBar;
    public int maxHealth = 100;
    public int currentHealth;
    public bool isHurt;
    private float lerpSpeed = 0.014f;

    [Space]
    [Header("Stamina")]
    public Slider staminaBar;
    public float maxStamina = 100;
    public float currentStamina;

    [Space]
    [Header("Mana")]
    public Slider manaBar;
    public float maxMana = 100;
    public float currentMana;

    [Header("Crystals")]
    public int totalCrystalAmount;
    public int tempCrystalAmount;
    public int justCollected;
    public int previousCollected;

    public bool canPickup = true;

    [Space]
    [Header("DamagePopup")]
    public GameObject damagePopupPrefab;
    [SerializeField] private float maxOffsetDistanceX = 0.1f;
    [SerializeField] private float maxOffsetDistanceY = 0.2f;

    [Space]
    [Header("Crosshair")]
    public GameObject mageCrosshair;

    // Start is called before the first frame update
    void Start()
    {
        pMovement = GetComponent<PlayerMovement>();
        pAttack = GetComponent<PlayerAttack>();
        damageFlash = GetComponent<DamageFlash>();
        weaponHolder = GetComponent<WeaponHolder>();
        totalCrystalAmount = 0;
        justCollected = 0;

        currentHealth = maxHealth;
        HealthBar.maxValue = maxHealth;
        easeHealthBar.value = HealthBar.value;

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
        HealthBar.value = currentHealth;

        if (HealthBar.value != easeHealthBar.value)
        {
            easeHealthBar.value = Mathf.Lerp(easeHealthBar.value, currentHealth, lerpSpeed);
        }

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

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        damageFlash.CallDamageFlash();

        if (currentHealth < 0)
        {
            PlayerDie();
        }

        if (!pAttack.isAttacking && !pMovement.isWallSliding && !pMovement.isClimbingLedge)
        {
            isHurt = true;
            StartCoroutine("IsHurtAnimStop");
            Debug.Log("ishurt");
        }
        ShowDamagePopup(damageAmount);
    }

    private IEnumerator IsHurtAnimStop()
    {
        yield return new WaitForSeconds(.25f);
        isHurt = false;
    }

    public void PlayerDie()
    {
        Debug.Log("playerDie");
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

    protected void ShowDamagePopup(float damageAmount)
    {

        // Generate random offset within maxOffsetDistance
        float offsetX = Random.Range(-maxOffsetDistanceX, maxOffsetDistanceX);
        float offsetY = Random.Range(-maxOffsetDistanceY, maxOffsetDistanceY);
        Vector3 offset = new Vector3(offsetX, offsetY, 0f);
        Vector3 startPosition = transform.position + offset;

        // Instantiate the damage popup prefab with random offset
        GameObject damagePopup = Instantiate(damagePopupPrefab, startPosition, Quaternion.identity);

        // Get the damage popup script
        DamagePopup damagePopupScript = damagePopup.GetComponent<DamagePopup>();

        // Pass the damage amount to the damage popup script
        damagePopupScript.SetDamageText(damageAmount);

        // Pass a reference of the enemy object to the damage popup script
        damagePopupScript.ShowDamageAmount(damageAmount, gameObject);
    }

}
