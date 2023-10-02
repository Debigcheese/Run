using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SetCameraSize : MonoBehaviour
{
    private Camera mainCamera;

    public float cameraSize = 7.5f;



    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        



        if (mainCamera != null)
        {
            mainCamera.orthographicSize = cameraSize;
        }
    }


}
