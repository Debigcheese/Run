using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private Rigidbody2D rb;
    public GameObject Player;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Queue<string> sentences;
    private Animator anim;
    public bool isDialogue;
    public bool finishFrogAnim = false;

    public AudioSource frogSound;

    [Header("Booleans")]
    public bool stopCharacterMovement;
    public bool freezeGravity;
    private bool enableDash;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        playerAttack = FindAnyObjectByType<PlayerAttack>();
        anim = GetComponentInParent<Animator>();
        sentences = new Queue<string>();
        rb = Player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDialogue && stopCharacterMovement)
        {
            playerAttack.dialogueStopAttack = true;
            playerMovement.cantMove = true;

            rb.velocity = Vector2.zero;
            playerMovement.isMoving = false;
            if (freezeGravity)
            {
                rb.gravityScale = 0;
            }
           
        }
        if (!stopCharacterMovement && isDialogue)
        {
            StartCoroutine(DialogueEnd());
        }

        if (enableDash)
        {

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                playerMovement.enableDashUponCollision = true;
                playerMovement.cantMove = false;
                isDialogue = false;
                anim.SetBool("isOpen", false);
                finishFrogAnim = true;
                playerAttack.dialogueStopAttack = false;

                rb.gravityScale = playerMovement.originalGravity;
                StartCoroutine(GoDash());
                enableDash = false;
            }
        }
    }
    IEnumerator GoDash()
    {
        yield return new WaitForSeconds(.1f);
        playerMovement.Dash(new Vector2(1f, 1f));

    }

    IEnumerator DialogueEnd()
    {
        yield return new WaitForSeconds(3f);
        isDialogue = false;
        anim.SetBool("isOpen", false);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogue = true;
        anim.SetBool("isOpen", true);
        nameText.text = dialogue.name;
        sentences.Clear();
        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
            if (!frogSound.isPlaying)
            {
                frogSound.Play();
            }
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        if(freezeGravity && sentences.Count == 1)
        {
            EndDialogue();
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            if (!frogSound.isPlaying)
            {
                frogSound.Play();
            }
            yield return new WaitForSeconds(.02f);
        }
    }

    public void EndDialogue()
    {
        if (!freezeGravity)
        {
            isDialogue = false;
            anim.SetBool("isOpen", false);
            finishFrogAnim = true;
            playerAttack.dialogueStopAttack = false;
            playerMovement.cantMove = false;
        }
        if (freezeGravity)
        {
            enableDash = true;
            
        }
    }
}
