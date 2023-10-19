using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    [SerializeField] private RawImage[] img;
    [SerializeField] private float x;
    [SerializeField] private float speed1 = 1f;
    [SerializeField] private float speed2 = 1f;
    [SerializeField] private float speed3 = 1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        img[0].uvRect = new Rect(img[0].uvRect.position + new Vector2(x  , img[0].uvRect.position.y), img[0].uvRect.size);
        img[1].uvRect = new Rect(img[1].uvRect.position + new Vector2(x * speed1, img[1].uvRect.position.y), img[1].uvRect.size);
        img[2].uvRect = new Rect(img[2].uvRect.position + new Vector2(x * speed2, img[2].uvRect.position.y), img[2].uvRect.size);
        img[3].uvRect = new Rect(img[3].uvRect.position + new Vector2(x * speed3, img[3].uvRect.position.y), img[3].uvRect.size);

    }
}