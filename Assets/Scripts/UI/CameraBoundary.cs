using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraBoundary : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private Transform playerTransform;  // Reference to the player's transform
    private bool hitBoundary_X = false;
    private bool hitBoundary_Y = false;
    private Vector3 cameraOffset;
    private Camera mainCamera;
    private BoxCollider2D boxCollider;

    private float originalXDamp;
    private float originalYDamp;
    public float transitionDuration = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        virtualCamera = GameObject.FindGameObjectWithTag("CineCamera").GetComponent<CinemachineVirtualCamera>();
        mainCamera = Camera.main;
        cameraOffset = virtualCamera.transform.position - playerTransform.position;

        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        StartCoroutine(SetColliderSize());

        originalXDamp = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping;
        originalYDamp = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping;
    }

    // Update is called once per frame
    void Update()
    {
        // Follow the player's position
        transform.position = playerTransform.position;

        if (hitBoundary_X && !hitBoundary_Y)
        {
            var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            transposer.m_XDamping = 0;
            transposer.m_YDamping = 0;

            virtualCamera.Follow = null;
            Vector3 newPos = playerTransform.position + cameraOffset;
            newPos.x = virtualCamera.transform.position.x;
            virtualCamera.transform.position = newPos;

        }
        else if (hitBoundary_Y && !hitBoundary_X)
        {
            var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            transposer.m_XDamping = 0;
            transposer.m_YDamping = 0;

            virtualCamera.Follow = null;
            Vector3 newPos = playerTransform.position + cameraOffset;
            newPos.y = virtualCamera.transform.position.y;
            virtualCamera.transform.position = newPos;

        }
        else if(hitBoundary_X && hitBoundary_Y)
        {
            var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            transposer.m_XDamping = 0;
            transposer.m_YDamping = 0;
            virtualCamera.Follow = null;
        }
        else
        {

            virtualCamera.Follow = playerTransform;
            var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            transposer.m_XDamping = Mathf.Lerp(transposer.m_XDamping, originalXDamp, Time.deltaTime / transitionDuration);
            transposer.m_YDamping = Mathf.Lerp(transposer.m_YDamping, originalYDamp, Time.deltaTime / transitionDuration);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boundary_X"))
        {
            hitBoundary_X = true;
        }
        if (collision.CompareTag("Boundary_Y")){
            hitBoundary_Y = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Boundary_X"))
        {
            hitBoundary_X = false;
        }
        if (collision.CompareTag("Boundary_Y"))
        {
            hitBoundary_Y = false;
        }
    }

    IEnumerator SetColliderSize()
    {
        yield return new WaitForSeconds(5f);
        if (mainCamera != null && boxCollider != null)
        {
            // Set the collider size to match the camera's orthographic size
            float cameraHeight = mainCamera.orthographicSize * 2;
            float cameraWidth = cameraHeight * mainCamera.aspect;
            boxCollider.size = new Vector2(cameraWidth, cameraHeight);

            // Set the collider's position to match the camera's position 
        }
    }


}
