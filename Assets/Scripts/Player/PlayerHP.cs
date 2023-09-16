using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    private DamageFlash damageFlash;
    public Slider HealthBar;
    public int maxHealth = 100;
    public int currentHealth;
    public bool isHurt;

    [Space]
    [Header("DamagePopup")]
    public GameObject damagePopupPrefab;
    [SerializeField] private float maxOffsetDistanceX = 0.1f;
    [SerializeField] private float maxOffsetDistanceY = 0.2f;

    [Space]
    [Header("Particles")]
    public ParticleSystem damageFeedbackParticles;

    // Start is called before the first frame update
    void Start()
    {
        damageFlash = GetComponent<DamageFlash>();
        currentHealth = maxHealth;
        HealthBar.maxValue = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        HealthBar.value = currentHealth;
        damageFlash.CallDamageFlash();
        isHurt = true;
        if(currentHealth < 0)
        {
            playerDie();
        }
        ShowDamagePopup(damageAmount);
        damageFeedbackParticles.Play();
        StartCoroutine("isHurtAnimStop");
    }

    private IEnumerator isHurtAnimStop()
    {
        yield return new WaitForSeconds(.25f);
        isHurt = false;
    }

    public void playerDie()
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
