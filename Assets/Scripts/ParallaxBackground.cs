using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Vector3 parallaxEffectMultiplier;
    [SerializeField] private bool infiniteHorizontal;
    [SerializeField] private bool infiniteVertical;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;
    private float textureUnitSizeY;

    public bool permaActive;
    [SerializeField] private bool inRangeActivate;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        Sprite sprite = GetComponentInChildren<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        textureUnitSizeY = texture.height / sprite.pixelsPerUnit;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (permaActive)
        {
            inRangeActivate = true;
        }
        if (inRangeActivate)
        {
            Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
            transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y);
            lastCameraPosition = cameraTransform.position;

            if (infiniteVertical)
            {
                if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
                {
                    float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
                    transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
                }

            }

            if (infiniteHorizontal)
            {
                if (Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUnitSizeY)
                {
                    float offsetPositionY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
                    transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetPositionY);
                }

            }
        }
                
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CamBoundary"))
        {
            lastCameraPosition = cameraTransform.position;
            inRangeActivate = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("CamBoundary"))
        {
            inRangeActivate = false;
        }
    }


}
