using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogNPC : MonoBehaviour
{
    private Animator anim;
    private DialogueManager dialogueManager;
    public bool frogJump;
    public bool frogTalk;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        dialogueManager = GetComponentInChildren<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("FrogJump", frogJump);
        anim.SetBool("FrogTalk", frogTalk);

        if (dialogueManager.finishFrogAnim)
        {
            
            frogJump = true;
        }

        if(dialogueManager.startSentence)
        {
            frogTalk = true;
        }
        else
        {
            frogTalk = false;
        }
    }
}
