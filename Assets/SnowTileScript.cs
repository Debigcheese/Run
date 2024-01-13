using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowTileScript : MonoBehaviour
{
    public GameObject normalSnowTile;
    public GameObject walkedOnSnowTile;
    public bool snowTileReset = false;
    public bool playerOnSnowTile;

    // Start is called before the first frame update
    void Start()
    {
        normalSnowTile.SetActive(true);
        walkedOnSnowTile.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            walkedOnSnowTile.SetActive(true);
            normalSnowTile.SetActive(false);
            playerOnSnowTile = true;
            if (snowTileReset)
            {
                StartCoroutine(ResetSnowTile());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerOnSnowTile = false;
        }
    }

    private IEnumerator ResetSnowTile()
    {
        yield return new WaitForSeconds(7f);
        if (!playerOnSnowTile)
        {
            walkedOnSnowTile.SetActive(false);
            normalSnowTile.SetActive(true);
        }
    }
}
