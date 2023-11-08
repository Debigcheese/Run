using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsScriptInGame : MonoBehaviour
{
    private PlayerState playerState;

    private bool checkIfEsc = false;
    private bool canRespawn = true;
    public GameObject settingsPanel;


    // Start is called before the first frame update
    void Start()
    {
        playerState = FindObjectOfType<PlayerState>();
        settingsPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (checkIfEsc)
            {
                checkIfEsc = false;
                Time.timeScale = 1f;
                settingsPanel.SetActive(false);
            }
            else
            {
                checkIfEsc = true;
                Time.timeScale = 0f;
                settingsPanel.SetActive(true);
            }
        }
    }

    public void LeaveGame()
    {
        SceneManager.LoadScene(1);
    }
    public void PlayGame()
    {
        checkIfEsc = false;
        Time.timeScale = 1f;
        settingsPanel.SetActive(false);
    }

    public void Respawn()
    {
        if (canRespawn)
        {
            checkIfEsc = false;
            canRespawn = false;
            settingsPanel.SetActive(false);
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
