using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Crystal : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerState playerState;
    private Collider2D collider;

    public Transform crystalTrans;
    public GameObject textPrefab;

    [Header("Booleans")]
    public bool magnetize = false;
    public bool stopMoving = false;
    public bool pickedUp = false;

    [Header("Crystal")]
    public int crystalValue = 1;
    public bool firstCollected;

    [Header("magnetize")]
    public float magnetizeRadius = 5f;
    public float crystalMs = 6.5f;
    public float stopMovingDelay = .6f;
    private Vector3 offset;
    

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        playerState = FindAnyObjectByType<PlayerState>();
        textPrefab.SetActive(true);
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
        if (stopMoving && magnetize && !pickedUp)
        {
            Vector2 direction = (playerState.transform.position - rb.transform.position).normalized;
            rb.velocity = direction * crystalMs;
        }

        Collider2D[] crystalMagnetize = Physics2D.OverlapCircleAll(transform.position, magnetizeRadius);
        foreach (Collider2D crystalmagnet in crystalMagnetize)
        {
            if (crystalmagnet.CompareTag("Player") && !pickedUp)
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
        yield return new WaitForSeconds(stopMovingDelay);
        stopMoving = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && stopMoving)
        {
            Debug.Log(crystalValue);
            pickedUp = true;
            magnetize = false;
            collider.enabled = false;
            rb.velocity = Vector2.zero;
            SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
            renderer.enabled = false;

            playerState.totalCrystalAmount += crystalValue;
            playerState.tempCrystalAmount += crystalValue;
            playerState.justCollected++;
            firstCollected = true;

            StartCoroutine(StopCounting());
        }
    }

    public IEnumerator StopCounting()
    {
        yield return new WaitForSeconds(.3f);
        playerState.previousCollected++;
        StartCoroutine(destroyObj());

        if (playerState.justCollected == playerState.previousCollected && firstCollected)
        {
            ShowFloatingTextTotal();
        }
    }

    private void ShowFloatingTextTotal()
    {
        GameObject text = Instantiate(textPrefab, transform.position, Quaternion.identity, transform);
        text.GetComponentInChildren<TextMeshPro>().text = "+" + playerState.tempCrystalAmount.ToString();
        playerState.tempCrystalAmount = 0;
        playerState.justCollected = 0;
    }

    private IEnumerator destroyObj()
    {
        yield return new WaitForSeconds(.8f);
        playerState.previousCollected = playerState.justCollected;
        Destroy(this.gameObject);
    }
}
