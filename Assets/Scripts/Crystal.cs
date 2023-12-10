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

    [Header("Crystal")]
    public int crystalValue = 1;
    public bool counting;

    [Header("magnetize")]
    public float crystalMs = 9f;
    public float stopMovingDelay = 1.8f;
    private Vector3 offset;
    

    // Start is called before the first frame update
    void Start()
    {
        crystalCollider = GetComponent<Collider2D>();
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

        if (collision.CompareTag("Player") && stopMoving && magnetize)
        {
            AudioManager.Instance.PlayLoopingSound("playercrystalcollect");
            magnetize = false;
            rb.velocity = Vector2.zero;
            SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
            renderer.enabled = false;
            crystalCollider.enabled = false;
            playerState.totalCrystalAmount += crystalValue;
            playerState.tempCrystalAmount += crystalValue;
            if (!playerState.isCounting)
            {
                StartCoroutine(WaitSeconds());
            }
            playerState.isCounting = true;

        }
    }

    IEnumerator WaitSeconds()
    {
        yield return new WaitForSeconds(.9f);
        playerState.isCounting = false;
        if (!playerState.isCounting)
        {
            ShowFloatingText();
        }

    }

    private void ShowFloatingText()
    {
        GameObject text = Instantiate(textPrefab, transform.position, Quaternion.identity, transform);
        text.GetComponentInChildren<TextMeshPro>().text = "+" + playerState.tempCrystalAmount.ToString();
        playerState.tempCrystalAmount = 0;
    }


}
