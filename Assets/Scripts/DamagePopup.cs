using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TextMeshPro text;

    private float duration = 1f;
    private float moveSpeed = 1f;
    private float alphaSpeed = 1f;

    private void Start()
    {
        StartCoroutine(DestroyPopup());
    }

    public void SetDamageText(float damage)
    {
        text.text = damage.ToString();
    }

    private IEnumerator DestroyPopup()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private void Update()
    {

        // Move the popup up
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);

        // Fade out the popup
        Color color = text.color;
        color.a -= alphaSpeed * Time.deltaTime;
        text.color = color;
    }

    public void ShowDamageAmount(float damageAmount, GameObject targetObject)
    {
        // Set the damage text to the damage amount
        SetDamageText(damageAmount);

        // Set the text of the damage popup
        text.text = damageAmount.ToString();

        // Generate a random angle within a wide range
        float angle = Random.Range(45f, 135f); // Adjust the range as needed

        // Convert the angle to radians
        float radianAngle = angle * Mathf.Deg2Rad;

        // Calculate the direction vector based on the angle
        Vector3 direction = new Vector3(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle), 0f);

        // Set the position of the damage popup relative to the target's position
        

        // Start the coroutine to destroy the damage popup after 1 second
        StartCoroutine(DestroyPopup());
    }
}
