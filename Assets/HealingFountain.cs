using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingFountain : MonoBehaviour
{
    private PlayerState playerState;
    public GameObject respawnPos;
    public GameObject checkPointParticles; private bool checkPoint = false;

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
            playerState.respawnPosition.transform.position = respawnPos.transform.position;
            if (!checkPoint)
            {
                checkPoint = true;
                Instantiate(checkPointParticles, transform.position, Quaternion.identity, transform);
            }
        }
    }
}
