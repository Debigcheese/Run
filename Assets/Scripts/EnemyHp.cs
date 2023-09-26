using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyHp : MonoBehaviour
{
    private EnemyAI enemyAi;
    private CrystalDropper crystalDropper;
    private DamageFlash damageFlash;
    public UnityEvent EnemyKnockback;
    private bool tookDamage;

    [Header("Balancing")]
    public int maxHealth = 100;
    int currentHealth;

    [Header("EnemyHealthBar")]
    public Slider enemyHealthBar;
    public Slider easeHealthBar;
    private float lerpSpeed = 0.01f;
    private RectTransform rectHealthbar;
    private RectTransform rectEaseHealthbar;

    [Space]
    [Header("DamagePopup")]
    public GameObject damagePopupPrefab;
    [SerializeField] private float maxOffsetDistanceX = 0.1f;
    [SerializeField] private float maxOffsetDistanceY = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        enemyAi = GetComponent<EnemyAI>();
        crystalDropper = GetComponentInChildren<CrystalDropper>();
        damageFlash = GetComponent<DamageFlash>();
        rectHealthbar = enemyHealthBar.GetComponent<RectTransform>();
        rectEaseHealthbar = easeHealthBar.GetComponent<RectTransform>();
        currentHealth = maxHealth;
        enemyHealthBar.maxValue = maxHealth;
        easeHealthBar.maxValue = maxHealth;
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
        
    }

    public void TakeDamage(int damageAmount)
    {
        if (!tookDamage)
        {
            tookDamage = true;
            currentHealth -= damageAmount;
            //play hurt animation
            if (currentHealth <= 0)
            {
                Die();
            }
            EnemyKnockback.Invoke();
            damageFlash.CallDamageFlash();
            ShowDamagePopup(damageAmount);
            StartCoroutine(CanTakeDamage());

        }
    }

    private IEnumerator CanTakeDamage()
    {
        yield return new WaitForSeconds(.01f);
        tookDamage = false;
    }

    void Die()
    {
        crystalDropper.DropCrystal();
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
