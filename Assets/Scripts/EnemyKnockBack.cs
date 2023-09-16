using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyKnockback : MonoBehaviour
{
    [SerializeField]
    public Rigidbody2D rb;

    [Space]
    
    [Header("Knockback")]
    [SerializeField]
    private float KnockbackForceX = 10f, KnockbackForceY = 10f, delay = 0.15f;

    public UnityEvent OnBegin, OnDone;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayFeedBack(GameObject sender)
    {
        sender = GameObject.FindGameObjectWithTag("Player");
        StopAllCoroutines();
        OnBegin.Invoke();
		Vector2 direction = (transform.position - sender.transform.position).normalized;
        Vector2 newVelocity = rb.velocity;
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
    }


}
