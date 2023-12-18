using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyKnockback : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;

    [Space]
    [Header("Knockback")]
    [SerializeField] private float KnockbackForceX = 10f;
    [SerializeField] private float KnockbackForceY = 3f;
    [SerializeField] private float delay = 0.15f;
    private float originalKnockBackForceX;

    public UnityEvent OnBegin, OnDone;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        originalKnockBackForceX = KnockbackForceX;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayFeedBack()
    {

        StopAllCoroutines();
        OnBegin.Invoke();
        Vector2 direction = (transform.position - playerMovement.transform.position).normalized;
        Vector2 newVelocity = rb.velocity;
        int randomForce = Random.Range(-2, 3);
        KnockbackForceX += randomForce;
        newVelocity.x = direction.x * KnockbackForceX;
        newVelocity.y = rb.velocity.y + KnockbackForceY;
        rb.velocity = newVelocity;
        StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(delay);
        rb.velocity = Vector3.zero;
        OnDone.Invoke();
        KnockbackForceX = originalKnockBackForceX;
    }


}
