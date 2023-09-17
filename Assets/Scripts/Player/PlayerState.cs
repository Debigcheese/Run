using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    private Crystal crystal;
    private PlayerMovement playerMovement;

    [Header("Respawn")]
    private GameObject RespawnPosition;
    [SerializeField] private GameObject startPosition;
    [SerializeField] private bool useStartPosition = true;

    [Header("Crystals")]
    public int crystalCollected;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        crystalCollected = 0;
        //RespawnPosition = startPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }

}
