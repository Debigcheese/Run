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
    public Animator attackTutorialAnim;
    public GameObject Player;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Queue<string> sentences;
    private Animator anim;
    public GameObject attackTutorialImage;
    public bool showAttackTutorial;
    private bool showAttackCheck;
    public bool isDialogue;
    public bool finishFrogAnim = false;
    public bool dialogueFinished = false;
    public bool startSentence = false;

    public AudioSource frogSound;

    [Header("Booleans")]
    public bool stopCharacterMovement;
    public bool freezeGravity;
    public bool enableDash;
    public bool enableSwitchWeapons;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        playerAttack = FindAnyObjectByType<PlayerAttack>();
        anim = GetComponentInParent<Animator>();
        sentences = new Queue<string>();
        rb = Player.GetComponent<Rigidbody2D>();
        if (attackTutorialImage != null)
        {
            attackTutorialImage.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isDialogue == true)
        {
            DisplayNextSentence();
        }

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
                PlayerPrefs.SetInt("EnableDash", 1);
                PlayerPrefs.Save();
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
        if(attackTutorialImage != null && attackTutorialAnim != null && !showAttackCheck)
        {
            showAttackCheck = true;
            attackTutorialImage.SetActive(true);
            attackTutorialAnim.SetBool("Show", true);
        }
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
        startSentence = true;
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            if (!frogSound.isPlaying)
            {
                frogSound.Play();
            }
            yield return new WaitForSeconds(.015f);
        }
        startSentence = false;

    }

    public void EndDialogue()
    {
        if (attackTutorialImage != null && attackTutorialAnim != null)
        {
            attackTutorialAnim.SetBool("Show", false);
            attackTutorialImage.SetActive(false);
        }
        if (!freezeGravity)
        {
            isDialogue = false;
            anim.SetBool("isOpen", false);
            finishFrogAnim = true;
            playerAttack.dialogueStopAttack = false;
            playerMovement.cantMove = false;
            dialogueFinished = true;

            if (enableSwitchWeapons)
            {
                FindObjectOfType<WeaponHolder>().showSecondWeaponUI.SetActive(true);
                PlayerPrefs.SetInt("ShowSecondWeaponUI", 1);
                PlayerPrefs.Save();
            }
        }
        if (freezeGravity)
        {
            enableDash = true;

        }
    }
}
