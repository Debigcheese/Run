using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : MonoBehaviour
{
    public GameObject weaponPrefab;
    public bool pickedUp;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (pickedUp)
        {
            Destroy(gameObject);
        }
    }



}
