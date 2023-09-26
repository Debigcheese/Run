using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    private PlayerMovement pMovement;
    private PlayerAttack pAttack;
    private DamageFlash damageFlash;
    public Slider HealthBar;
    public Slider easeHealthBar;
    public int maxHealth = 100;
    public int currentHealth;
    public bool isHurt;
    private float lerpSpeed = 0.014f;

    [Space]
    [Header("DamagePopup")]
    public GameObject damagePopupPrefab;
    [SerializeField] private float maxOffsetDistanceX = 0.1f;
    [SerializeField] private float maxOffsetDistanceY = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        pMovement = GetComponent<PlayerMovement>();
        pAttack = GetComponent<PlayerAttack>();
        damageFlash = GetComponent<DamageFlash>();
        currentHealth = maxHealth;
        HealthBar.maxValue = maxHealth;
        easeHealthBar.value = HealthBar.value;
    }

    // Update is called once per frame
    void Update()
    {
        HealthBar.value = currentHealth;

        if(HealthBar.value != easeHealthBar.value)
        {
            easeHealthBar.value = Mathf.Lerp(easeHealthBar.value, currentHealth, lerpSpeed);
        }
        
        
    }

    public void takeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        damageFlash.CallDamageFlash();
        
        if(currentHealth < 0)
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
