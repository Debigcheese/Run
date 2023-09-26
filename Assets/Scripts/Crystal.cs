using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Crystal : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerState playerState;
    private Collider2D crystalCollider;

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
    public float crystalMs = 9f;
    public float stopMovingDelay = .8f;
    private Vector3 offset;
    

    // Start is called before the first frame update
    void Start()
    {
        crystalCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        playerState = FindAnyObjectByType<PlayerState>();
        playerState.canPickup = true;
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
        if (stopMoving && magnetize)
        {
            Vector2 direction = (playerState.transform.position - rb.transform.position).normalized;
            rb.velocity = direction * crystalMs;
        }
    }

    public IEnumerator StopMoving()
    {
        yield return new WaitForSeconds(stopMovingDelay);
        stopMoving = true;
        magnetize = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

            if (collision.CompareTag("Player") && stopMoving && playerState.canPickup)
            {
                pickedUp = true;
                magnetize = false;
                rb.velocity = Vector2.zero;
                SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
                renderer.enabled = false;
                crystalCollider.enabled = false;

                playerState.totalCrystalAmount += crystalValue;
                playerState.tempCrystalAmount += crystalValue;
                playerState.justCollected++;
                firstCollected = true;
                StartCoroutine(StopCounting());
            }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && stopMoving && playerState.canPickup)
        {
            magnetize = false;
            rb.velocity = Vector2.zero;
            SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
            renderer.enabled = false;


        }
    }

    public IEnumerator StopCounting()
    {
        yield return new WaitForSeconds(.9f);
        playerState.previousCollected++;
        if(playerState.justCollected == playerState.previousCollected && firstCollected)
        {
            ShowFloatingTextTotal();
        }
            
        StartCoroutine(destroyObj());
    }

    private void ShowFloatingTextTotal()
    {
        float tempCrystal = playerState.tempCrystalAmount;
        GameObject text = Instantiate(textPrefab, transform.position, Quaternion.identity, transform);
        text.GetComponentInChildren<TextMeshPro>().text = "+" + tempCrystal.ToString();
        playerState.canPickup = false;
        playerState.tempCrystalAmount = 0;
        playerState.justCollected = 0;
    }

    private IEnumerator destroyObj()
    {
        yield return new WaitForSeconds(.8f);
        playerState.canPickup = true;
        playerState.previousCollected = 0;
        Destroy(this.gameObject);
    }
}
