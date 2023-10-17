using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockTransform : MonoBehaviour
{
    private Transform objectTransform;
    private Transform startingTransform;

    // Start is called before the first frame update
    void Start()
    {
        objectTransform = GetComponent<Transform>();
        startingTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        objectTransform = startingTransform;
    }
}
