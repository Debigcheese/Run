using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    private PlayerState playerState;
    [SerializeField] private float countdown;
    public GameObject startButton;
    public GameObject showWaveChest;
    public bool canPressWaveBtn;

    public Wave[] waves;
    public int currentWaveIndex = 0;

    public GameObject[] spawnPoints;

    public bool spawnerActive = false;
    private bool readyToCountDown = false;
    private bool endSpawn = false;

    private Collider2D[] waveBoundaries;
    private string boundaryEndString = "BoundaryEnd";
    private string boundaryStartString = "BoundaryStart";
    private string doorBoundary = "DoorSprite";

    [Space]
    [Header("Canvas")]
    public GameObject Canvas;
    [SerializeField] private TextMeshProUGUI WaveText;
    public GameObject wavesStarting;
    public GameObject nextWaveText;
    public GameObject wavesClearedText;

    public Animator buttonAnim;
    public Animator doorAnim;

    // Start is called before the first frame update
    void Start()
    {
        playerState = FindObjectOfType<PlayerState>();
        waveBoundaries = GetComponentsInChildren<Collider2D>();
        Canvas.SetActive(false);
        nextWaveText.SetActive(false);
        wavesStarting.SetActive(false);
        spawnerActive = false;
        wavesClearedText.SetActive(false);
        startButton.transform.GetChild(0).gameObject.SetActive(false);
        showWaveChest.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        WaveText.text = "Wave: " + (currentWaveIndex + 1).ToString() + " / " + waves.Length.ToString();

        //ending boundary on when spawner not active
        if (!spawnerActive && !endSpawn)
        {
            IterateColliders(false, true, true);
        }

        //finish
        if (currentWaveIndex >= waves.Length && !endSpawn)
        {
            IterateColliders(false, false, false);

            AudioManager.Instance.PlaySound("wavesfinished");
            endSpawn = true;
            EndSpawner();
            wavesClearedText.SetActive(true);
            showWaveChest.SetActive(true);
            showWaveChest.GetComponent<Animator>().SetBool("showWaveChest", true);
            doorAnim.SetBool("WaveDoorOpen", true);
            StartCoroutine(TimerForWaveChest());
            return;
        }

        //Spawner start button pressed
        if (canPressWaveBtn && Input.GetKeyDown(KeyCode.E))
        {
            buttonAnim.SetBool("isPressed", true);
            GetComponent<Collider2D>().enabled = false;
            readyToCountDown = true;
            playerState.isRespawnForSpawner = false;
            startButton.transform.GetChild(0).gameObject.SetActive(false);
            AudioManager.Instance.PlaySound("wavebuttonpressed");
            for (int i = 0; i < waves.Length; i++)
            {
                waves[i].enemiesLeft = waves[i].enemies.Length;
            }
            spawnerActive = true;
        }

        //spawner start
        if (spawnerActive && currentWaveIndex < waves.Length)
        {
            Canvas.SetActive(true);

            if (readyToCountDown == true)
            {
                countdown -= Time.deltaTime;
            }

            if (countdown <= 0)
            {
                readyToCountDown = false;
                countdown = waves[currentWaveIndex].timeToNextWave;
                if(currentWaveIndex < waves.Length)
                {
                    StartCoroutine(SpawnWave());
                }

            }
            if (waves[currentWaveIndex].enemiesLeft == 0)
            {
                readyToCountDown = true;
                currentWaveIndex++;
            }

            IterateColliders(true, true, true);

            //player dies (restart spawner)
            if (playerState.isRespawnForSpawner)
            { 
                foreach(GameObject spawnpoint in spawnPoints)
                {
                    GameObject[] childObjects = new GameObject[spawnpoint.transform.childCount];
                    for(int i = 0; i < spawnpoint.transform.childCount; i++)
                    {
                        childObjects[i] = spawnpoint.transform.GetChild(i).gameObject;
                    }

                    foreach (GameObject childObject in childObjects)
                    {
                        Destroy(childObject);
                    }
                }

                startButton.transform.GetChild(0).gameObject.SetActive(false);
                buttonAnim.SetBool("isPressed", false);
                GetComponent<Collider2D>().enabled = true;

                countdown = 0;
                currentWaveIndex = 0;
                readyToCountDown = false;
                wavesClearedText.SetActive(false);
                EndSpawner();

            }
        } 
    }

    private IEnumerator SpawnWave()
    {
        if(currentWaveIndex < waves.Length)
        {
            if(currentWaveIndex == 0)
            {
                wavesStarting.SetActive(true);
            }
            else
            {
                AudioManager.Instance.PlaySound("nextwave");
                nextWaveText.SetActive(true);
            }
            WaveText.enabled = true;


            for (int i = 0; i < waves[currentWaveIndex].enemies.Length; i++)
            {
                if (!playerState.isRespawnForSpawner)
                {
                    int random = Random.Range(0, spawnPoints.Length);
                    GameObject enemy = Instantiate(waves[currentWaveIndex].enemies[i], spawnPoints[random].transform.position, Quaternion.identity, spawnPoints[random].transform);
                    enemy.GetComponent<EnemyAI>().waveSpawnerEnemies = true;
                    enemy.GetComponent<EnemyHp>().countWaveEnemies = true;

                    yield return new WaitForSeconds(waves[currentWaveIndex].timeToNextEnemy);
                }
            }
            wavesStarting.SetActive(false);
            nextWaveText.SetActive(false);
        }
    }

    private void IterateColliders(bool start, bool end, bool door)
    {
        foreach (Collider2D childCollider in waveBoundaries)
        {
            if (childCollider.name == boundaryStartString)
            {
                childCollider.enabled = start;
            }
            if (childCollider.name == boundaryEndString)
            {
                childCollider.enabled = end;
            }
            if (childCollider.name == doorBoundary)
            {
                childCollider.enabled = door;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            startButton.transform.GetChild(0).gameObject.SetActive(true);
            canPressWaveBtn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            startButton.transform.GetChild(0).gameObject.SetActive(false);
            canPressWaveBtn = false;
        }
    }

    private void EndSpawner()
    {
        nextWaveText.SetActive(false);
        wavesStarting.SetActive(false);
        WaveText.enabled = false;
        spawnerActive = false;
    }

    IEnumerator TimerForWaveChest()
    {
        yield return new WaitForSeconds(2f);
        if(showWaveChest != null)
        {
            showWaveChest.GetComponent<Animator>().SetBool("showWaveChest", false);
            showWaveChest.GetComponent<Collider2D>().enabled = true;
        }

    }

    [System.Serializable]
    public class Wave
    {
        public GameObject[] enemies;
        public float timeToNextEnemy;
        public float timeToNextWave = 2f;

        [HideInInspector] public int enemiesPerWave;
        [HideInInspector] public int enemiesLeft;
    }
}
