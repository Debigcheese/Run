using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class EnemySFX
{
    [Header("SFX")]
    public string EnemyHurtSFX;
    public string EnemyDieSFX;
    public string EnemyAttackSFX;

    public void PlayEnemySound(string enemySFX)
    {
        AudioManager.Instance.PlaySound(enemySFX);
    }
}

public class EnemyHp : MonoBehaviour
{
    public EnemySFX enemySFX;
    private Rigidbody2D rb;
    private SpriteRenderer sRenderer;
    private Collider2D coll;
    private Animator anim;
    private WaveSpawner waveSpawner;
    private PlayerAttack playerAttack;
    private EnemyAttack enemyAttack;
    private EnemyAI enemyAi;
    private CrystalDropper crystalDropper;
    private DamageFlash damageFlash;
    public UnityEvent EnemyKnockback;
    public bool countWaveEnemies;
    public bool isDead = false;
    public bool tookDamage;
    public bool dontInstaKill = false;
    public bool canTakeDamage = true;
    public bool dontDestroyOnDeath = false;
    public float destroyEnemyOnDeathTimer = 0.66f;

    [Header("Balancing")]
    public int maxHealth = 100;
    public int currentHealth;
    public int crystalDropAmount;

    [Header("EnemyHealthBar")]
    public Slider enemyHealthBar;
    public Slider easeHealthBar;
    private float lerpSpeed = 0.01f;
    private RectTransform rectHealthbar;
    private RectTransform rectEaseHealthbar;
    public GameObject EnemyUi;

    [Space]
    [Header("DamagePopup")]
    public GameObject damagePopupPrefab;
    [SerializeField] private float maxOffsetDistanceX = 0.1f;
    [SerializeField] private float maxOffsetDistanceY = 0.2f;

    //[Space]
    //[Header("Particles")]
    //public ParticleSystem isDeadParticles;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        waveSpawner = GetComponentInParent<WaveSpawner>();
        playerAttack = FindAnyObjectByType<PlayerAttack>();
        enemyAi = GetComponent<EnemyAI>();
        enemyAttack = GetComponent<EnemyAttack>();
        crystalDropper = GetComponentInChildren<CrystalDropper>();

        sRenderer = GetComponentInChildren<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        damageFlash = GetComponent<DamageFlash>();
        rectHealthbar = enemyHealthBar.GetComponent<RectTransform>();
        rectEaseHealthbar = easeHealthBar.GetComponent<RectTransform>();
        currentHealth = maxHealth;
        enemyHealthBar.maxValue = maxHealth;
        easeHealthBar.maxValue = maxHealth;
        TextMeshPro damageTextComponent = damagePopupPrefab.GetComponentInChildren<TextMeshPro>();
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        enemyHealthBar.value = currentHealth;
        if (enemyHealthBar.value != easeHealthBar.value)
        {
            easeHealthBar.value = Mathf.Lerp(easeHealthBar.value, currentHealth, lerpSpeed);
        }

        if (enemyAi.lookingRight)
        {
            rectHealthbar.localScale = new Vector2(-1* Mathf.Abs(rectHealthbar.localScale.x), rectHealthbar.localScale.y);
            rectEaseHealthbar.localScale = new Vector2(-1 * Mathf.Abs(rectEaseHealthbar.localScale.x), rectEaseHealthbar.localScale.y);
        }

        else
        {
            rectHealthbar.localScale = new Vector2(1 * Mathf.Abs(rectHealthbar.localScale.x), rectHealthbar.localScale.y);
            rectEaseHealthbar.localScale = new Vector2(1 * Mathf.Abs(rectEaseHealthbar.localScale.x), rectEaseHealthbar.localScale.y);
        }
        
        anim.SetBool("isDead", isDead);
    }

    public void TakeDamage(int damageAmount)
    {
        if (!tookDamage)
        {
            if (canTakeDamage)
            {
                if (currentHealth > 0 && enemySFX.EnemyHurtSFX != null)
                {
                    enemySFX.PlayEnemySound(enemySFX.EnemyHurtSFX);
                }

                tookDamage = true;
                enemyAi.tookDamageDetect = true;
                currentHealth -= damageAmount;
                //play hurt animation
                if (currentHealth <= 0 && !dontInstaKill)
                {
                    StartCoroutine(Die());
                }
                if (GetComponent<EnemyIceBoss>() == null || !GetComponent<EnemyIceBoss>().longAttackDisableKnockback) 
                {
                    EnemyKnockback.Invoke();
                }

                damageFlash.CallDamageFlash();
                ShowDamagePopup(damageAmount);
                StartCoroutine(CanTakeDamage());
            }
            else
            {
                tookDamage = true;
                enemyAi.tookDamageDetect = true;
                damageFlash.CallDamageFlash();
                ShowDamagePopup(0);
                StartCoroutine(CanTakeDamage());
            }
        }
    }

    private IEnumerator CanTakeDamage()
    {
        yield return new WaitForSeconds(.01f);
        tookDamage = false;
        yield return new WaitForSeconds(1.5f);
        enemyAi.tookDamageDetect = false;
    }

    public void CallEnemyDeath()
    {
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        if (!isDead)
        {
            if (enemySFX.EnemyDieSFX != null)
            {
                enemySFX.PlayEnemySound(enemySFX.EnemyDieSFX);
            }
            if (countWaveEnemies)
            {
                waveSpawner.waves[waveSpawner.currentWaveIndex].enemiesLeft--;
            }
            crystalDropper.DropCrystal(crystalDropAmount);

            isDead = true;
            coll.enabled = false;
            enemyAi.canMove = false;
            if (GetComponent<EnemyMinotaur>() != null)
            {
                GetComponent<EnemyMinotaur>().canAttack = false;
            }
            else
            {
                if (enemyAttack != null)
                    enemyAttack.canAttack = false;
            }
            rb.simulated = false;

            if (EnemyUi != null)
                EnemyUi.SetActive(false);
            yield return new WaitForSeconds(destroyEnemyOnDeathTimer);

            if (!dontDestroyOnDeath)
                Destroy(this.gameObject);
        }
    }

    protected void ShowDamagePopup(float damageAmount)
    {
        
        // Generate random offset within maxOffsetDistance
        float offsetX = Random.Range(-maxOffsetDistanceX, maxOffsetDistanceX);
        float offsetY = Random.Range(-maxOffsetDistanceY, maxOffsetDistanceY);
        Vector3 offset = new Vector3(offsetX, offsetY, 0f);
        Vector3 startPosition = transform.position + offset;

        TextMeshPro damageTextComponent = damagePopupPrefab.GetComponentInChildren<TextMeshPro>();
        if (playerAttack.critAttack)
        {
            damageTextComponent.color = Color.yellow;
        }
        else
        {
            damageTextComponent.color = Color.white;
        }
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
