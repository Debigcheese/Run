using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHp : MonoBehaviour
{
    [Header("Balancing")]
    public float maxHealth = 100;
    float currentHealth;

    [Space]
    [Header("DamagePopup")]
    public GameObject damagePopupPrefab;
    [SerializeField] private float maxOffsetDistanceX = 0.1f;
    [SerializeField] private float maxOffsetDistanceY = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        //play hurt animation
        if(currentHealth <= 0)
        {
            Die();
        }

         ShowDamagePopup(damageAmount);
        
    }

    void Die()
    {
        Debug.Log("Enemy died!");
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
