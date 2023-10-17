using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLevel : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerMovement.FinishLevelMovement();
            anim.SetBool("StartTransition", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("StartTransition", false);
        }
    }
}
