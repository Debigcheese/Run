using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemySpawner : MonoBehaviour
{
    private Animator anim;
    private EnemyHp enemyHp;

    [SerializeField] private bool mustEliminateToProceed = false;
    [SerializeField] private bool killEnemiesWhenDestroyed = true;
    [SerializeField] private float spawnTimer = 0f;
    [SerializeField] private float spawnDelay = 8f;
    [SerializeField] private float extraTimeToSpawn = 1f;
    [SerializeField] private GameObject doorBoundary;
    [SerializeField] private Collider2D[] doorColliders;

    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private Light2D skullLight;

    [SerializeField] private GameObject[] enemiesSpawnDirect;
    [SerializeField] private GameObject[] enemiesSpawnConstantly;

    private bool startConstantSpawnAfterDelay = false;
    private bool startConstantSpawn = false;
    private bool stopConstantSpawn = false;

    private bool isHurtAnim;
    private BoxCollider2D spawnCollider;

    [Space]
    [Header("BossSpawners")]
    [SerializeField] public bool spawnedByBoss = false;
    [SerializeField] public bool spawnerDestroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        enemyHp = GetComponentInParent<EnemyHp>();
        spawnCollider = GetComponent<BoxCollider2D>();
        spawnTimer = spawnDelay;
        anim = GetComponent<Animator>();
        anim.SetBool("isSpawning", true);

        if(doorBoundary != null)
        {
            if (mustEliminateToProceed)
            {
                doorBoundary.SetActive(true);
            }
            else
            {
                doorBoundary.SetActive(false);
            }
        }
        StartCoroutine(IsSpawning());
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHp.isDead)
        {
            if (mustEliminateToProceed && doorBoundary != null)
            {
                doorBoundary.GetComponent<Animator>().SetBool("WaveDoorOpen", true);
                foreach (Collider2D c in doorColliders)
                {
                    c.enabled = false;
                }
            }
            anim.SetBool("isDead", true);
            skullLight.enabled = false;

            if (killEnemiesWhenDestroyed)
            {
                foreach (GameObject spawnpoint in spawnPoints)
                {
                    for (int i = 0; i < spawnpoint.transform.childCount; i++)
                    {
                        GameObject childObject = spawnpoint.transform.GetChild(i).gameObject;
                        EnemyHp enemy = childObject.GetComponent<EnemyHp>();
                        if (enemy != null)
                        {
                            enemy.CallEnemyDeath();
                        }
                    }
                }
            }

            if (spawnedByBoss)
            {
                spawnerDestroyed = true;
            }
        }
        if (enemyHp.tookDamage)
        {
            isHurtAnim = true;
            StartCoroutine(StopIsHurtAnim());
        }
        if (isHurtAnim)
        {
            anim.SetBool("isHurt", true);
        }

        if (startConstantSpawn && !stopConstantSpawn && !enemyHp.isDead)
        {
            spawnTimer -= Time.deltaTime;
            if(spawnTimer <= 0)
            {
                for (int i = 0; i < enemiesSpawnConstantly.Length; i++)
                {
                    int randomSpawnPoint = Random.Range(0, spawnPoints.Length);
                    Instantiate(enemiesSpawnConstantly[i], spawnPoints[spawnPoints.Length - i - 1].transform.position, Quaternion.identity, spawnPoints[randomSpawnPoint].transform);
                    enemiesSpawnConstantly[i].GetComponent<EnemyAI>().waveSpawnerEnemies = true;
                    StartCoroutine(EnemySpawnAnim(spawnPoints.Length - i - 1));
                    AudioManager.Instance.PlaySound("enemyspawn");
                }
                StartCoroutine(IncreaseSkullIntensity());
                FindObjectOfType<CameraShake>().ShakeCameraFlex(1.5f, .25f);
                spawnTimer = spawnDelay;
            }
        }

        if(startConstantSpawnAfterDelay && !startConstantSpawn && !enemyHp.isDead)
        {
            spawnTimer -= Time.deltaTime;
            if(spawnTimer <= spawnDelay - extraTimeToSpawn)
            {
                for (int i = 0; i < enemiesSpawnDirect.Length; i++)
                {
                    Instantiate(enemiesSpawnDirect[i], spawnPoints[spawnPoints.Length - i - 1].transform.position, Quaternion.identity, spawnPoints[spawnPoints.Length - i - 1].transform);
                    enemiesSpawnDirect[i].GetComponent<EnemyAI>().waveSpawnerEnemies = true;
                    spawnCollider.size = new Vector2(spawnCollider.size.x + 7, spawnCollider.size.y + 7);
                    StartCoroutine(EnemySpawnAnim(spawnPoints.Length - i - 1));
                    AudioManager.Instance.PlaySound("enemyspawn");
                }
                FindObjectOfType<CameraShake>().ShakeCameraFlex(3f, .25f);
                StartCoroutine(IncreaseSkullIntensity());
                startConstantSpawn = true;
            }
        }
    }

    private IEnumerator EnemySpawnAllAnim()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPoints[spawnPoints.Length - i - 1].GetComponent<Animator>().SetBool("isEnemySpawning", true);
        }
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPoints[spawnPoints.Length - i - 1].GetComponent<Animator>().SetBool("isEnemySpawning", false);
        }
    }

    private IEnumerator EnemySpawnAnim(int spawnPoint)
    {
        spawnPoints[spawnPoint].GetComponent<Animator>().SetBool("isEnemySpawning", true);
        yield return new WaitForSeconds(1f);
        spawnPoints[spawnPoint].GetComponent<Animator>().SetBool("isEnemySpawning", false);
    }

    private IEnumerator IncreaseSkullIntensity()
    {
        anim.SetBool("changeSkullIntensity", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("changeSkullIntensity", false);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !startConstantSpawn)
        {
            startConstantSpawnAfterDelay = true;
        }

        if (collision.CompareTag("Player") && stopConstantSpawn)
        {
            stopConstantSpawn = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            stopConstantSpawn = true;
            spawnCollider.size = new Vector2(spawnCollider.size.x, spawnCollider.size.y);
        }
    }

    private IEnumerator StopIsHurtAnim()
    {
        yield return new WaitForSeconds(.4f);
        isHurtAnim = false;
        anim.SetBool("isHurt", false);
    }

    private IEnumerator IsSpawning()
    {
        yield return new WaitForSeconds(2.33f);
        anim.SetBool("isSpawning", false);
    }
}
