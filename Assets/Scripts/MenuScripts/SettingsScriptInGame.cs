using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsScriptInGame : MonoBehaviour
{
    private PlayerState playerState;
    public Animator anim;

    private bool checkIfEsc = false;
    private bool canRespawn = true;
    public GameObject menuPanel;
    public GameObject settingsPanel;

    // Start is called before the first frame update
    void Start()
    {
        playerState = FindObjectOfType<PlayerState>();
        menuPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AudioManager.Instance.PlaySound("uibutton");
            OpenMenu();
        }
    }

    public void OpenMenu()
    {
        if (checkIfEsc)
        {
            AudioManager.Instance.PlaySound("uibutton");
            checkIfEsc = false;
            Time.timeScale = 1f;
            menuPanel.SetActive(false);
            settingsPanel.SetActive(false);
        }
        else
        {
            AudioManager.Instance.PlaySound("uibutton");
            checkIfEsc = true;
            Time.timeScale = 0f;
            menuPanel.SetActive(true);

        }
    }

    public void OpenSettings()
    {
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);

        AudioManager.Instance.PlaySound("uibutton");
    }

    public void CloseSettingsInGame()
    {
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        AudioManager.Instance.PlaySound("uibutton");
    }

    public void CloseSettings()
    {
        checkIfEsc = false;
        Time.timeScale = 1f;
        menuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        AudioManager.Instance.PlaySound("uibutton");
    }

    public void LeaveGame()
    {
        Time.timeScale = 1f;
        StartCoroutine(LeaveGameCoroutine());
        AudioManager.Instance.PlaySound("uibutton");
        anim.SetBool("TransitionStart", true);
    }

    public IEnumerator LeaveGameCoroutine()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
    }

    public void Respawn()
    {
        if (canRespawn)
        {
            AudioManager.Instance.PlaySound("uibutton");
            checkIfEsc = false;
            canRespawn = false;
            menuPanel.SetActive(false);
            Time.timeScale = 1f;
            playerState.StartPlayerDie();
            StartCoroutine(RespawnTimer());
        }
        AudioManager.Instance.PlaySound("uibuttonwrong");

    }

    private IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(5f);
        canRespawn = true;
    }

}
