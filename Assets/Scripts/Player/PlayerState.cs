using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerState : MonoBehaviour
{
    private Collider2D coll;
    private SpriteRenderer sRenderer;
    private Rigidbody2D rb;

    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private WeaponHolder weaponHolder;
    private DamageFlash damageFlash;

    [Header("Respawn")]
    [SerializeField] private GameObject startPosition;
    [SerializeField] private GameObject respawnPosition;

    [Space]
    [Header("Health")]
    public Slider HealthBar;
    public Slider easeHealthBar;
    public int maxHealth = 100;
    public int currentHealth;
    public bool isHurt;
    public bool isDead;
    public bool isRegeningHp;
    private float lerpSpeed = 0.014f;
    private bool canRegen = true;

    [Space]
    [Header("BloodyScreen")]
    public Image bloodyScreen;
    public float transitionDuration = 1f;
    private Color originalColor;
    private Color transparentColor;
    private bool waitColorChange;

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
    public bool isCounting;
    public TextMeshProUGUI crystalText;

    [Space]
    [Header("DamagePopup")]
    public GameObject damagePopupPrefab;
    [SerializeField] private float maxOffsetDistanceX = 0.1f;
    [SerializeField] private float maxOffsetDistanceY = 0.2f;

    [Space]
    [Header("Particles")]
    public GameObject isDeadParticles;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
        damageFlash = GetComponent<DamageFlash>();
        weaponHolder = GetComponent<WeaponHolder>();

        coll = GetComponent<Collider2D>();
        sRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponentInChildren<Rigidbody2D>();

        transform.position = new Vector3(startPosition.transform.position.x, startPosition.transform.position.y, startPosition.transform.position.z);
        
        totalCrystalAmount = 0;

        currentHealth = maxHealth;
        HealthBar.maxValue = maxHealth;
        easeHealthBar.value = HealthBar.value;

        originalColor = bloodyScreen.color;
        transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        bloodyScreen.color = transparentColor;
        bloodyScreen.enabled = false;

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

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (weaponHolder.meleeEquipped )
        {
            staminaBar.value = currentStamina;
            manaBar.gameObject.SetActive(false);
            staminaBar.gameObject.SetActive(true);
        }

        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }

        if (weaponHolder.magicEquipped)
        {
            manaBar.value = currentMana;
            staminaBar.gameObject.SetActive(false);
            manaBar.gameObject.SetActive(true);
        }

        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }

        if(currentHealth <= (0.18 * maxHealth))
        {
            bloodyScreen.enabled = true;

            if(!waitColorChange)
            {
                StartCoroutine(ActivateBloodyScreen());
                waitColorChange = true;
            }

        }
        else
        {
            bloodyScreen.enabled = false;
            bloodyScreen.color = transparentColor;
        }

        crystalText.text = totalCrystalAmount.ToString();

        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerDie();
            Respawn();
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("HealingFountain"))
        {
            if (canRegen)
            {
                StartCoroutine(RegenHp());
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("HealingFountain"))
        {
            isRegeningHp = false;
            canRegen = true;
        }
    }

    public IEnumerator RegenHp()
    {
        isRegeningHp = true;
        canRegen = false;
        yield return new WaitForSeconds(.4f);
        if (currentHealth < maxHealth && isRegeningHp )
        {
            Debug.Log("works");
            float tempCurrHp = currentHealth;
            float tempMaxHp = maxHealth;
            tempCurrHp += tempMaxHp * 0.1f;
            currentHealth = Mathf.RoundToInt(tempCurrHp);
            canRegen = true;
        }
    }

    public IEnumerator ActivateBloodyScreen()
    {
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsedTime / transitionDuration);
            bloodyScreen.color = Color.Lerp(originalColor, transparentColor, t);
            yield return null;
        }

        // Pause for a moment with the target alpha.
        yield return new WaitForSeconds(.5f);

        // Reverse the transition.
        elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsedTime / transitionDuration);
            bloodyScreen.color = Color.Lerp(transparentColor, originalColor, t);
            yield return null;
        }
        waitColorChange = false;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        damageFlash.CallDamageFlash();

        if (currentHealth <= 0)
        {
            PlayerDie();
        }

        if (!playerAttack.isAttacking && !playerMovement.isWallSliding && !playerMovement.isClimbingLedge)
        {
            isHurt = true;
            StartCoroutine("IsHurtAnimStop");
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
        isDead = true;
        coll.enabled = false;
        sRenderer.enabled = false;
        playerMovement.cantMove = true;
        playerAttack.dialogueStopAttack = true;
        playerAttack.isAttacking = false;
        rb.simulated = false;
        Instantiate(isDeadParticles, transform.position, Quaternion.identity, transform);
        
    }

    public void Respawn()
    {
        isDead = false;
        coll.enabled = true;
        sRenderer.enabled = true;
        playerMovement.cantMove = false;
        playerAttack.dialogueStopAttack = false;
        rb.simulated = true;
        transform.position = respawnPosition.transform.position;
        currentHealth = maxHealth;
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
