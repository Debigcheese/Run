using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement playerMovement;
    private int LevelSelectIndex = 1;
    public int LevelUnlocked;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        playerMovement = FindObjectOfType<PlayerMovement>();

        anim.SetBool("TransitionEnd", true);
        StartCoroutine(EndTransition());
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySound("levelcomplete");
            playerMovement.FinishLevelMovement();
            anim.SetBool("TransitionStart", true);
            StartCoroutine(LoadNextLevel());
        }
    }

    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(1.5f);
        PlayerPrefs.SetInt("LevelsUnlocked", LevelUnlocked);
        SceneManager.LoadScene(LevelSelectIndex);
    }

    IEnumerator EndTransition()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("TransitionEnd", false);
    }


}
