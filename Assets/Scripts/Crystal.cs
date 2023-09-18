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
    public int crystalValue;
    
    [Header("magnetize")]
    public float magnetizeRadius = 5f;
    public float crystalMs = 6.5f;
    public float delay = .6f;
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
        yield return new WaitForSeconds(delay);
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
            renderer.color = new Color(0f, 0f, 0f, 1f);

            playerState.totalCrystalAmount += crystalValue;
            playerState.totalcrystalCollected += 1;
            playerState.justCollected += 1;
            if (playerState.justCollected <= 1)
            {
                StartCoroutine(showFloatingText());
            }
            else
            {
                StopCoroutine("showFloatingText");
                playerState.addCrystalValue += crystalValue;
                StartCoroutine(showFloatingTextTotal());
            }
            
            StartCoroutine(destroyObj());  
        }
    }

    private IEnumerator showFloatingText()
    {
        yield return new WaitForSeconds(.5f);
        GameObject text = Instantiate(textPrefab, transform.position, Quaternion.identity, transform);
        text.GetComponentInChildren<TextMeshPro>().text = "+" + crystalValue.ToString();
    }

    private IEnumerator showFloatingTextTotal()
    {
        yield return new WaitForSeconds(1f);
        GameObject text = Instantiate(textPrefab, transform.position, Quaternion.identity, transform);
        text.GetComponentInChildren<TextMeshPro>().text = "+" + playerState.addCrystalValue.ToString();
        playerState.justCollected = 0;
    }

    private IEnumerator destroyObj()
    {
        yield return new WaitForSeconds(4f);
        Destroy(this.gameObject);
    }
}
