using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassScript : MonoBehaviour
{
    private Animator anim;

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
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy") || collision.IsTouchingLayers(4))
        {
            anim.gameObject.GetComponent<Animator>().enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy") || collision.CompareTag("Water"))
        {
            anim.gameObject.GetComponent<Animator>().enabled = false;
        }
    }


}
