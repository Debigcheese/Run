using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    private PlayerMovement playerMovement;

    [Header("Respawn")]
    private GameObject RespawnPosition;
    [SerializeField] private GameObject startPosition;
    [SerializeField] private bool useStartPosition = true;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        RespawnPosition = startPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }
 
}
