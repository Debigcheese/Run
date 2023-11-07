using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SetCameraSize : MonoBehaviour
{
    private Camera mainCamera;
    public PlayerMovement playerMovement;

    public float startingCameraSize = 3f;
    public float cameraSize = 7f;
    public float duration = 2f;
    private float time = 0f;
    public bool check;
    private bool CameraConstantSize;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        StartCoroutine(ZoomOut());
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera != null && !check)
        {
            mainCamera.orthographicSize = startingCameraSize;
        }

        if (mainCamera != null && check && !CameraConstantSize)
        {
            if (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                mainCamera.orthographicSize = Mathf.Lerp(startingCameraSize, cameraSize, t);
            }
            else
            {
                CameraConstantSize = true;
            }

        }

        if (CameraConstantSize)
        {
            mainCamera.orthographicSize = cameraSize;
        }

    }

    private IEnumerator ZoomOut()
    {
        yield return new WaitForSeconds(2f);
        check = true;
    }

}
