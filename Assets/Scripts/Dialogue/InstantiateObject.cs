using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateObject : MonoBehaviour
{
    public GameObject itemToInstantiate;
    private DialogueManager dialogueManager;
    private Vector2 instantiatePos;
    private bool stopInstantiate;

    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = GetComponentInChildren<DialogueManager>();
        instantiatePos = new Vector2(transform.position.x + 1, transform.position.y +1);
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogueManager.dialogueFinished && !stopInstantiate )
        {
            Instantiate(itemToInstantiate, instantiatePos, Quaternion.identity);
            stopInstantiate = true;
        }
    }
}
