using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassAnim : MonoBehaviour
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
        if (GetComponent<Collider2D>().IsTouchingLayers(5))
        {
            anim.gameObject.GetComponent<Animator>().enabled = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy") || collision.CompareTag("Water") )
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
