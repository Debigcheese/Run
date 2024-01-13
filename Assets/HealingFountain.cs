using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingFountain : MonoBehaviour
{
    private PlayerState playerState;
    public GameObject respawnPos;
    public GameObject checkPointParticles; private bool checkPoint = false;
    public GameObject eKey;
    private bool fountainInRange = false;

    // Start is called before the first frame update
    void Start()
    {
        playerState = FindObjectOfType<PlayerState>();
        if(eKey != null)
        {
            eKey.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((playerState.currentHealth != playerState.maxHealth) && fountainInRange && eKey != null)
        {
            eKey.SetActive(true);
        }
        else
        {
            if (eKey != null)
                eKey.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerState.respawnPosition.transform.position = respawnPos.transform.position;
            fountainInRange = true;

            if (!checkPoint)
            {
                checkPoint = true;
                if(checkPointParticles != null)
                {
                    Instantiate(checkPointParticles, transform.position, Quaternion.identity, transform);
                    AudioManager.Instance.PlaySound("healingfountaincheckpoint");
                }

            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && eKey != null)
        {
            fountainInRange = false;
            eKey.SetActive(false);
        }
    }
}
