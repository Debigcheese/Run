using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class LevelSelectMouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int level;
    [SerializeField] private bool isHovering;
    public LevelSelectBackgroundMoving movingBackground;
    private bool canHover = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnableCanHover());
    }

    // Update is called once per frame
    void Update()
    {
        if (isHovering && canHover)
        {
            movingBackground.SetIsHoveringLevel(level);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        Debug.Log("Pointer entered: " + gameObject.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        Debug.Log("Pointer exited: " + gameObject.name);
    }

    public IEnumerator EnableCanHover()
    {
        yield return new WaitForSeconds(1f);
        canHover = true;
    }

}
