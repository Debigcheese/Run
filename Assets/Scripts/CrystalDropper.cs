using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalDropper : MonoBehaviour
{
    public GameObject[] crystalPrefabs;
    public int crystalDropAmount;
    private int crystalLeftAmount;

    // Start is called before the first frame update
    void Start()
    {
        crystalLeftAmount = crystalDropAmount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DropCrystal()
    {
        while (crystalLeftAmount >= 840)
        {
            InstantiateCrystal(3);
            crystalLeftAmount -= 840;
        }
        //max 6 crystals
        while (crystalLeftAmount >= 140)
        {
            InstantiateCrystal(2);
            crystalLeftAmount -= 140;
        }
        //max 9 crystals
        while (crystalLeftAmount >= 14)
        {
            InstantiateCrystal(1);
            crystalLeftAmount -= 14;
        }
        //max 13 crystals
        while (crystalLeftAmount > 0)
        {
            InstantiateCrystal(0);
            crystalLeftAmount -= 1;
        }
    }

    private void InstantiateCrystal(int crystalIndex)
    {
        Instantiate(crystalPrefabs[crystalIndex], transform.position, Quaternion.identity);
    }
}
