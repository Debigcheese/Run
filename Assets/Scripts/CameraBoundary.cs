using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraBoundary : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private Transform playerTransform;  // Reference to the player's transform
    private Vector3 cameraPosBeforeHit;
    private bool hitBoundary = false;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        virtualCamera = GameObject.FindGameObjectWithTag("CineCamera").GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerTransform.position;

        if (hitBoundary)
        {
            // Follow the player's position
            virtualCamera.Follow = null;
        }
        else
        {
            virtualCamera.Follow = playerTransform;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boundary"))
        {
            hitBoundary = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Boundary"))
        {
            hitBoundary = false;
        }
    }
}
