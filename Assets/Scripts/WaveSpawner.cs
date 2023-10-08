using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private float countdown;

    public Wave[] waves;
    public int currentWaveIndex = 0;

    public GameObject[] spawnPoints;

    public bool spawnerActive = false;
    private bool readyToCountDown = false;
    private bool endSpawn = false;

    private Collider2D[] waveBoundaries;
    private bool boundaries = false;

    [Space]
    [Header("Canvas")]
    public GameObject Canvas;
    [SerializeField] private TextMeshProUGUI WaveText;
    public TextMeshProUGUI nextWaveText;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        countdown = 6f;
        waveBoundaries = GetComponentsInChildren<Collider2D>();
        Canvas.SetActive(false);
        nextWaveText.enabled = false;
        spawnerActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawnerActive && !boundaries)
        {
            for (int i = 1; i < waveBoundaries.Length; i++)
            {
                waveBoundaries[i].enabled = false;
            }
        }
        if (currentWaveIndex >= waves.Length && !endSpawn)
        {
            spawnerActive = false;
            boundaries = false;
            Canvas.SetActive(false);
            Debug.Log("done");
            endSpawn = true;
            return;
        }
        if (spawnerActive && currentWaveIndex < waves.Length)
        {
            boundaries = true;
            //enemiesLeftText.text = "Enemies left: " + waves[currentWaveIndex].enemiesLeft.ToString();

            if (readyToCountDown == true)
            {
                countdown -= Time.deltaTime;
            }

            if (countdown <= 0)
            {
                readyToCountDown = false;
                countdown = waves[currentWaveIndex].timeToNextWave;
                StartCoroutine(SpawnWave());
                WaveText.text = "Wave: " + (currentWaveIndex+1).ToString() + " / " + waves.Length.ToString();
            }
            if (waves[currentWaveIndex].enemiesLeft == 0)
            {
                readyToCountDown = true;
                currentWaveIndex++;
            }

            for (int i = 1; i < waveBoundaries.Length; i++)
            {
                if (boundaries)
                {
                    waveBoundaries[i].enabled = true;
                }
                
            }
            Canvas.SetActive(true);
            
        }

        
    }

    private IEnumerator SpawnWave()
    {
        if(currentWaveIndex < waves.Length)
        {
            nextWaveText.enabled = true;
            for (int i = 0; i < waves[currentWaveIndex].enemies.Length; i++)
            {
                int random = Random.Range(0, spawnPoints.Length);
                GameObject enemy = Instantiate(waves[currentWaveIndex].enemies[i], spawnPoints[random].transform.position, Quaternion.identity, spawnPoints[random].transform);
                enemy.GetComponent<EnemyAI>().waveSpawnerEnemies = true;
                enemy.GetComponent<EnemyHp>().countWaveEnemies = true;

                yield return new WaitForSeconds(waves[currentWaveIndex].timeToNextEnemy);
                
            }
            nextWaveText.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            Collider2D collider = GetComponent<Collider2D>();
            collider.enabled = false;
            readyToCountDown = true;
            for (int i = 0; i < waves.Length; i++)
            {
                waves[i].enemiesLeft = waves[i].enemies.Length;
            }
            spawnerActive = true;
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
