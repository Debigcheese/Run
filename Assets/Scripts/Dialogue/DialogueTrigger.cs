using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [TextArea(3, 10)]
    private string[] sentences;

    public Dialogue dialogue;
    private int count;

    public void TriggerDialogue()
    {
        GetComponentInChildren<DialogueManager>().StartDialogue(dialogue);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && count != 1)
        {
            TriggerDialogue();
            count++;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
