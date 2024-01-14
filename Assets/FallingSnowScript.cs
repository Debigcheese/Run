using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FallingSnowScript : MonoBehaviour
{
    private PlayerMovement pb;
    public GameObject[] fallingSnow;
    public Collider2D fallingSnowCollider;
    private bool inSnowRange;

    // Start is called before the first frame update
    void Start()
    {
        pb = FindObjectOfType<PlayerMovement>();

        for(int i = 0; i<fallingSnow.Length; i++)
        {
            fallingSnow[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inSnowRange)
        {
            for (int i = 0; i < fallingSnow.Length; i++)
            {
                fallingSnow[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < fallingSnow.Length; i++)
            {
                fallingSnow[i].SetActive(false);
            }
            //StartCoroutine(DisableSnow());
        }
    }

    //private IEnumerator DisableSnow()
    //{
    //    yield return new WaitForSeconds(1.5f);
    //    if (!inSnowRange)
    //    {

    //    }
    //}

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inSnowRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !pb.isClimbingLedge && !pb.isWallJumping)
        {
            inSnowRange = false;
        }
    }

}
