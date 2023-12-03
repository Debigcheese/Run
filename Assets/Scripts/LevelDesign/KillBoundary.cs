using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBoundary : MonoBehaviour
{
    private PlayerState playerState;

    // Start is called before the first frame update
    void Start()
    {
        playerState = FindObjectOfType<PlayerState>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerState.TakeDamage(playerState.maxHealth);
        }
    }
}
