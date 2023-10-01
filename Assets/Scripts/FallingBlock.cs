using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D[] blockCollider;
    public GameObject fallingBlock;
    private Vector3 startPos;
    public bool startFallingAnim;
    public float timeBeforeFalling = 2f;
    public float fallingSpeed;
    public float timeToMakeNewBlock;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        startPos = transform.position;
        blockCollider = GetComponents<Collider2D>();
        foreach (Collider2D collider in blockCollider)
        {
            collider.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("startFallingAnim", startFallingAnim);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(BlockShake());

        }
    }

    private IEnumerator BlockShake()
    {
        
        yield return new WaitForSeconds(timeBeforeFalling);
        startFallingAnim = true;
        anim.Play("FallingBlock");

        rb.constraints = RigidbodyConstraints2D.None;
        timer += Time.deltaTime;
        rb.velocity = new Vector2(rb.velocity.x, -fallingSpeed * timer);

        StartCoroutine(MakeNewBlock());
    }

    private IEnumerator MakeNewBlock()
    {
        yield return new WaitForSeconds(2f);

        foreach (Collider2D collider in blockCollider)
        {
            collider.enabled = false;
        }

        yield return new WaitForSeconds(timeToMakeNewBlock);
        startFallingAnim = false;
        Instantiate(fallingBlock, startPos, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
