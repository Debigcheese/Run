using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyKnockback : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private WeaponHolder weaponHolder;

    [Space]
    [Header("Knockback")]
    [SerializeField] private float KnockbackForceX = 0f;
    [SerializeField] private float decreaseknockbackForceX = 0f;
    [SerializeField] private float KnockbackForceY = 3f;
    [SerializeField] private float delay = 0.15f;

    public UnityEvent OnBegin, OnDone;

    // Start is called before the first frame update
    void Start()
    {
        weaponHolder = FindObjectOfType<WeaponHolder>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
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
        int randomForceX = Random.Range(8, 12);
        if (!weaponHolder.isUsingMagic || !weaponHolder.isUsingRanged)
        {
            randomForceX = Random.Range(12, 15);
        }
        KnockbackForceX = randomForceX - decreaseknockbackForceX;
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
