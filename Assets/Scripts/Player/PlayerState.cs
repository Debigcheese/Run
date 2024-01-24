using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerState : MonoBehaviour
{
    private Collider2D coll;
    private SpriteRenderer sRenderer;
    private Rigidbody2D rb;

    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private WeaponHolder weaponHolder;
    private DamageFlash damageFlash;
    public Animator[] anim;

    [Header("Respawn")]
    public GameObject respawnPosition;
    public bool isRespawnForSpawner;

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
    private bool inRangeHealingFountain = false;
    private float healingFountainTimer = 0f;
    private bool tookDamage = false;

    [Space]
    [Header("GuardianAbility")]
    public bool guardianEnabled;
    public float guardianDamageReduction = .5f;
    public float msReduction = 0.5f;
    public bool checkGuardianCooldown;
    public float guardianCooldown;
    public float guardianDuration;
    private float guardianTimer;
    public GameObject GuardianIcon;
    public Image GuardianCDImage;
    public Color guardianColor;
    public Color originalPlayerColor;

    [Space]
    [Header("BloodyScreen")]
    public Image bloodyScreen;
    public float transitionDuration = 1f;
    public bool bloodyScreenActivateOnFeedback = false;
    public float transitionDurationFeedback = 0.1f;
    private Color originalColor;
    private Color transparentColor;
    private bool waitColorChange;

    [Space]
    [Header("Stamina")]
    public Slider staminaBar;
    public float maxStamina = 100;
    public float currentStamina;
    public float staminaRegenTimer = 0.075f;

    [Space]
    [Header("Mana")]
    public Slider manaBar;
    public float maxMana = 100;
    public float currentMana;
    public float manaRegenTimer = 0.075f;

    [Header("Crystals")]
    public int totalCrystalAmount;
    public int tempCrystalAmount;
    public int crystalsLostOnDeath;
    public bool isCounting;
    public TextMeshProUGUI crystalText;

    [Space]
    [Header("TextPopup")]
    public GameObject damagePopupPrefab;
    [SerializeField] private float maxOffsetDistanceX = 0.1f;
    [SerializeField] private float maxOffsetDistanceY = 0.2f;

    [SerializeField] private GameObject lowManaPopupPrefab;
    [SerializeField] private GameObject lowStaminaPopupPrefab;
    [SerializeField] private GameObject healingPopupPrefab;
    public bool lowMana;
    private bool lowEnergyTimer;

    [Space]
    [Header("Particles")]
    public GameObject isDeadParticles;
    public GameObject isRegeningHpParticles;

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

        totalCrystalAmount = PlayerPrefs.GetInt("TotalCrystal", 0);
        crystalsLostOnDeath = 0;
        maxHealth = PlayerPrefs.GetInt("TotalHealth", maxHealth);

        currentHealth = maxHealth;
        HealthBar.maxValue = maxHealth;
        HealthBar.value = maxHealth;
        easeHealthBar.maxValue = maxHealth;
        easeHealthBar.value = maxHealth;

        originalColor = bloodyScreen.color;
        transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        bloodyScreen.color = transparentColor;
        bloodyScreen.enabled = true;

        manaBar.maxValue = maxMana;
        currentMana = maxMana;

        staminaBar.maxValue = maxStamina;
        currentStamina = maxStamina;

        isRegeningHpParticles.SetActive(false);

        StartCoroutine(RespawnPos());

        InvokeRepeating("RefillMana", 0f, PlayerPrefs.GetFloat("ManaRegen", manaRegenTimer));
        InvokeRepeating("RefillStamina", 0f, PlayerPrefs.GetFloat("StaminaRegen", staminaRegenTimer));
    }

    private IEnumerator RespawnPos()
    {
        yield return new WaitForSeconds(0.05f);
        transform.position = new Vector3(respawnPosition.transform.position.x, respawnPosition.transform.position.y, respawnPosition.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetInt("TotalCrystal", totalCrystalAmount);
        PlayerPrefs.Save();
        HealthBar.value = currentHealth;

        if (HealthBar.value != easeHealthBar.value)
        {
            easeHealthBar.value = Mathf.Lerp(easeHealthBar.value, currentHealth, lerpSpeed);
        }

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (isRegeningHp)
        {
            AudioManager.Instance.PlayLoopingSound("healingfountainregen");
        } 

        if(Input.GetKeyDown(KeyCode.LeftShift) && PlayerPrefs.GetInt("Ability") == 1 && !checkGuardianCooldown)
        {
            ActivateGuardian();
        }
        if (guardianEnabled)
        {
            AudioManager.Instance.PlayLoopingSound("playerduringguardian");
        }

        if (PlayerPrefs.GetInt("Ability") == 1)
        {
            GuardianIcon.SetActive(true);
            if (!guardianEnabled && checkGuardianCooldown)
            {
                guardianTimer += Time.deltaTime;
                float fillPercentage = 1f - (guardianTimer / (guardianCooldown));
                GuardianCDImage.fillAmount = fillPercentage;
                if (GuardianCDImage.fillAmount == 0)
                {
                    guardianTimer = 0;
                }
            }
        }
        else
        {
            GuardianIcon.SetActive(false);
        }

        GuardianChangeColor(GetComponentInChildren<SpriteRenderer>());

        if (weaponHolder.attackUseStamina )
        {
            staminaBar.value = currentStamina;
            manaBar.gameObject.SetActive(false);
            staminaBar.gameObject.SetActive(true);
        }

        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }

        if (weaponHolder.attackUseMana)
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
            if(!waitColorChange)
            {
                StartCoroutine(ActivateBloodyScreen());
                waitColorChange = true;
            }
        }

        if (bloodyScreenActivateOnFeedback && (!(currentHealth <= (0.18 * maxHealth))))
        {
            StartCoroutine(ActivateBloodyScreenOnFeedback());
            bloodyScreenActivateOnFeedback = false;
        }

        crystalText.text = totalCrystalAmount.ToString();

        for(int i = 0; i<anim.Length; i++)
        {
            anim[i].SetBool("GuardianActive", guardianEnabled);
        }

        if (Input.GetButtonDown("Fire1") && lowMana && !lowEnergyTimer && !playerAttack.isAttacking)
        {

            if (weaponHolder.attackUseStamina)
            {
                ShowTextPopup("Low stamina", lowStaminaPopupPrefab);
            }
            else if (weaponHolder.attackUseMana)
            {
                ShowTextPopup("Low mana", lowManaPopupPrefab);
            }
            lowEnergyTimer = true;
            StartCoroutine(LowEnergyTimer());
        }

        if (inRangeHealingFountain && Input.GetKey(KeyCode.E) && playerMovement.GroundCheck() && currentHealth != maxHealth)
        {

            HealingStopMovement();
            RegenHp();
        }
        if ((inRangeHealingFountain && Input.GetKeyUp(KeyCode.E) && playerMovement.GroundCheck()) || (inRangeHealingFountain && currentHealth == maxHealth))
        {
            healingFountainTimer = 0f;
            playerMovement.cantMove = false;
            playerAttack.canAttack = true;
            isRegeningHp = false;
            isRegeningHpParticles.SetActive(false);
            AudioManager.Instance.DisableSound("healingfountainregen");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("HealingFountain"))
        {
            inRangeHealingFountain = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("HealingFountain"))
        {
            inRangeHealingFountain = false;
        }
    }

    private void HealingStopMovement()
    {
        playerMovement.cantMove = true;
        rb.velocity = Vector2.zero;
        playerAttack.canAttack = false;
    }

    public void RegenHp()
    {
        isRegeningHp = true;
        isRegeningHpParticles.SetActive(true);
        healingFountainTimer += Time.deltaTime;
        if ((healingFountainTimer >= 0.75f) && isRegeningHp)
        {
            float tempCurrHp = currentHealth;
            float tempMaxHp = maxHealth;
            tempCurrHp += tempMaxHp * 0.1f;
            currentHealth = Mathf.RoundToInt(tempCurrHp);
            ShowTextPopup("Healing", healingPopupPrefab);
            healingFountainTimer = 0f;
        }
    }

    private IEnumerator LowEnergyTimer()
    {
        yield return new WaitForSeconds(1f);
        lowEnergyTimer = false;
    }

    private void ActivateGuardian()
    {
        playerMovement.speed *= msReduction;
        playerAttack.meleeAttackMoveSpeed *= msReduction;
        guardianEnabled = true;
        checkGuardianCooldown = true;
        GuardianCDImage.fillAmount = 1f;

        AudioManager.Instance.PlaySound("playeractivateguardian");
        AudioManager.Instance.PlayLoopingSound("smallfootsteps");

        StartCoroutine(GuardianDuration());
    }

    private IEnumerator GuardianDuration()
    {
        yield return new WaitForSeconds(guardianDuration);
        playerMovement.speed = playerMovement.originalSpeed;
        playerAttack.meleeAttackMoveSpeed = playerAttack.originalAttackMoveSpeed;
        guardianEnabled = false;

        AudioManager.Instance.DisableSound("playerduringguardian");
        AudioManager.Instance.PlaySound("playerdisableguardian");

        StartCoroutine(StartGuardianCooldown());
    }

    private IEnumerator StartGuardianCooldown()
    {
        yield return new WaitForSeconds(guardianCooldown);
        checkGuardianCooldown = false;
    }

    public void GuardianChangeColor(SpriteRenderer sprite)
    {
        if(PlayerPrefs.GetInt("Ability") == 1)
        {
            if (guardianEnabled)
            {
                sprite.color = guardianColor;
            }
            else
            {
                sprite.color = originalPlayerColor;
            }
        }
    }

    public IEnumerator ActivateBloodyScreenOnFeedback()
    {
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsedTime / transitionDuration);
            bloodyScreen.color = Color.Lerp(originalColor, transparentColor, t);
            yield return null;
        }

        yield return new WaitForSeconds(.15f);

        // Reverse the transition.

        waitColorChange = false;
        bloodyScreen.color = transparentColor;
        bloodyScreenActivateOnFeedback = false;
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
        yield return new WaitForSeconds(.3f);

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
        if (!tookDamage)
        {
            if (guardianEnabled)
            {
                float damage = damageAmount * guardianDamageReduction;
                int roundedDamage = Mathf.RoundToInt(damage);
                currentHealth -= roundedDamage;
                ShowDamagePopup(roundedDamage);
            }
            else
            {
                currentHealth -= damageAmount;
                ShowDamagePopup(damageAmount);
            }
            FindObjectOfType<CameraShake>().ShakeCameraFlex(3f, 0.25f);
            AudioManager.Instance.PlaySound("playerhurt");
            damageFlash.CallDamageFlash();
            bloodyScreenActivateOnFeedback = true;
            if (currentHealth <= 0)
            {
                StartCoroutine(PlayerDie());
            }

            if (!playerAttack.isAttacking && !playerMovement.isWallSliding && !playerMovement.isClimbingLedge)
            {
                isHurt = true;
                StartCoroutine("IsHurtAnimStop");
            }
            StartCoroutine(CanTakeDamage());
            tookDamage = true;
        }
    }

    private IEnumerator CanTakeDamage()
    {
        yield return new WaitForSeconds(.01f);
        tookDamage = false;
    }

    private IEnumerator IsHurtAnimStop()
    {
        yield return new WaitForSeconds(.25f);
        isHurt = false;
    }

    public void StartPlayerDie()
    {
        StartCoroutine(PlayerDie());
    }

    private IEnumerator PlayerDie()
    {
        isDead = true;
        coll.enabled = false;
        sRenderer.enabled = false;
        playerMovement.cantMove = true;
        playerAttack.dialogueStopAttack = true;
        playerAttack.isAttacking = false;
        rb.simulated = false;

        totalCrystalAmount -= Mathf.RoundToInt(crystalsLostOnDeath / 2f);
        crystalsLostOnDeath = 0;
        Instantiate(isDeadParticles, transform.position, Quaternion.identity, transform);
        StartCoroutine(Respawn());
        Time.timeScale = .4f;
        yield return new WaitForSeconds(.8f);
        Time.timeScale = 1f;
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1.3f);
        isRespawnForSpawner = true;
        isDead = false;
        coll.enabled = true;
        sRenderer.enabled = true;
        playerMovement.cantMove = false;
        playerAttack.dialogueStopAttack = false;
        rb.simulated = true;
        transform.position = respawnPosition.transform.position;
        currentHealth = maxHealth;

        //bloodyscreen
        waitColorChange = false;
        bloodyScreen.color = transparentColor;
        bloodyScreenActivateOnFeedback = false;
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
        currentMana += 1f;
    }

    protected void ShowDamagePopup(float damageAmount)
    {

        // Generate random offset within maxOffsetDistance
        float offsetX = UnityEngine.Random.Range(-maxOffsetDistanceX, maxOffsetDistanceX);
        float offsetY = UnityEngine.Random.Range(-maxOffsetDistanceY, maxOffsetDistanceY);
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

    protected void ShowTextPopup(string text, GameObject popupPrefab)
    {

        // Generate random offset within maxOffsetDistance
        float offsetX = UnityEngine.Random.Range(-0, 0);
        float offsetY = UnityEngine.Random.Range(-0, 0.5f);
        Vector3 offset = new Vector3(offsetX, offsetY, 0f);
        Vector3 startPosition = transform.position + offset;

        // Instantiate the damage popup prefab with random offset
        GameObject damagePopup = Instantiate(popupPrefab, startPosition, Quaternion.identity);

        // Get the damage popup script
        DamagePopup damagePopupScript = damagePopup.GetComponent<DamagePopup>();

        // Pass a reference of the enemy object to the damage popup script
        damagePopupScript.ShowLowMana(text, gameObject);
    }


}
