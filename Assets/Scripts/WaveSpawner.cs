using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private float countdown;

    public GameObject[] spawnPoints;
    public int spawnPointMin = 0;
    public int spawnPointMax = 1;

    public Wave[] waves;
    public int currentWaveIndex = 0;

    private bool readyToCountDown;

    // Start is called before the first frame update
    void Start()
    {
        readyToCountDown = true;
        for (int i = 0; i < waves.Length; i++)
        {
            waves[i].enemiesLeft = waves[i].enemies.Length;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentWaveIndex >= waves.Length)
        {
            Debug.Log("done");
            return;
        }

        if(readyToCountDown == true)
        {
            countdown -= Time.deltaTime;
        }

        if(countdown <= 0)
        {
            readyToCountDown = false;
            countdown = waves[currentWaveIndex].timeToNextWave;
            StartCoroutine(SpawnWave());
        }
        if(waves[currentWaveIndex].enemiesLeft == 0)
        {
            readyToCountDown = true;
            currentWaveIndex++;
        }
    }

    private IEnumerator SpawnWave()
    {
        if(currentWaveIndex < waves.Length)
        {
            for (int i = 0; i < waves[currentWaveIndex].enemies.Length; i++)
            {
                int random = Random.Range(spawnPointMin, spawnPointMax);
                GameObject enemy = Instantiate(waves[currentWaveIndex].enemies[i], spawnPoints[random].transform);
                enemy.GetComponent<EnemyAI>().waveSpawnerEnemies = true;

                yield return new WaitForSeconds(waves[currentWaveIndex].timeToNextEnemy);
            }
        }
    }

    [System.Serializable]
    public class Wave
    {
        public GameObject[] enemies;
        public float timeToNextEnemy;
        public float timeToNextWave = 2f;

        [HideInInspector] public int enemiesLeft;
    }
}
