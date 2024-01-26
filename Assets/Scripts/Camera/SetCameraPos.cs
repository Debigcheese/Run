using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SetCameraPos : MonoBehaviour
{
    private Animator anim;
    public Transform cameraTrans;
    private PlayerMovement playerMovement;
    private CinemachineVirtualCamera cinemachineVC;

    public bool isChangingCameraSize;

    // Start is called before the first frame update
    void Start()
    {
        cinemachineVC = FindObjectOfType<CinemachineVirtualCamera>();
        playerMovement = FindObjectOfType<PlayerMovement>();

    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cinemachineVC.Follow = null;
            isChangingCameraSize = true;
            anim.SetBool("changeCameraPos", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cinemachineVC.Follow = playerMovement.transform;
            isChangingCameraSize = false;
            anim.SetBool("changeCameraPos", false);
        }
    }
}
