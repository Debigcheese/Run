using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsScriptInGame : MonoBehaviour
{
    private PlayerState playerState;

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
            OpenMenu();
        }
    }

    public void OpenMenu()
    {
        if (checkIfEsc)
        {
            checkIfEsc = false;
            Time.timeScale = 1f;
            menuPanel.SetActive(false);
            settingsPanel.SetActive(false);
        }
        else
        {
            checkIfEsc = true;
            Time.timeScale = 0f;
            menuPanel.SetActive(true);

        }
    }

    public void OpenSettings()
    {
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsInGame()
    {
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        checkIfEsc = false;
        Time.timeScale = 1f;
        menuPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void LeaveGame()
    {
        SceneManager.LoadScene(1);
    }
    public void PlayGame()
    {
        checkIfEsc = false;
        Time.timeScale = 1f;
        menuPanel.SetActive(false);
    }

    public void Respawn()
    {
        if (canRespawn)
        {
            checkIfEsc = false;
            canRespawn = false;
            menuPanel.SetActive(false);
            Time.timeScale = 1f;
            playerState.StartPlayerDie();
            StartCoroutine(RespawnTimer());
        }

    }

    private IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(5f);
        canRespawn = true;
    }

}
