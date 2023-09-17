using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerState playerState;

    public Transform crystalTrans;

    [Header("Booleans")]
    public bool magnetize = false;
    public bool stopMoving = false;

    [Header("Crystal")]
    public int crystalValue;

    [Header("magnetize")]
    public float magnetizeRadius = 5f;
    public float crystalMs = 6.5f;
    public float delay = .6f;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerState = FindAnyObjectByType<PlayerState>();

        offset = new Vector3(Random.Range(1.5f, -1.5f), Random.Range(0, 2));
        StartCoroutine(StopMoving());
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopMoving)
        {
            crystalTrans.position += offset * Time.deltaTime;
        }
        if (stopMoving)
        {
            rb.velocity = Vector2.zero;
        }
        if (stopMoving && magnetize)
        {
            Vector2 direction = (playerState.transform.position - rb.transform.position).normalized;
            rb.velocity = direction * crystalMs;
        }

        Collider2D[] crystalMagnetize = Physics2D.OverlapCircleAll(transform.position, magnetizeRadius);
        foreach (Collider2D crystalmagnet in crystalMagnetize)
        {
            if (crystalmagnet.CompareTag("Player"))
            {
                magnetize = true; 
                break; 
            }
            else
            {
                magnetize = false;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, magnetizeRadius);
    }
    public IEnumerator StopMoving()
    {
        yield return new WaitForSeconds(delay);
        stopMoving = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && stopMoving)
        {
            Debug.Log(crystalValue);
            playerState.crystalCollected += crystalValue;
            Destroy(gameObject);
        }
    }
}
