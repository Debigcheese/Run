using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAnimSpeedWhenCollide : MonoBehaviour
{
    private Animator anim;
    public float maxSpeed = 1.2f;
    public float minSpeed = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.gameObject.GetComponent<Animator>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.gameObject.GetComponent<Animator>().enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.gameObject.GetComponent<Animator>().enabled = false;
        }
    }


}
