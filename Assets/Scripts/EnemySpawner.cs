using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemySpawner : MonoBehaviour
{
    private Animator anim;
    private EnemyHp enemyHp;

    [SerializeField] private bool mustEliminateToProceed = false;
    [SerializeField] private float spawnTimer = 0f;
    [SerializeField] private float spawnDelay = 8f;
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

    // Start is called before the first frame update
    void Start()
    {
        enemyHp = GetComponentInParent<EnemyHp>();
        spawnCollider = GetComponent<BoxCollider2D>();
        spawnTimer = spawnDelay;
        anim = GetComponent<Animator>();

        if (mustEliminateToProceed)
        {
            doorBoundary.SetActive(true);
        }
        else
        {
            doorBoundary.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHp.isDead)
        {
            if (mustEliminateToProceed)
            {
                doorBoundary.GetComponent<Animator>().SetBool("WaveDoorOpen", true);
                foreach (Collider2D c in doorColliders)
                {
                    c.enabled = false;
                }
            }
            anim.SetBool("isDead", true);
            skullLight.enabled = false;

            foreach (GameObject spawnpoint in spawnPoints)
            {
                GameObject[] childObjects = new GameObject[spawnpoint.transform.childCount];
                for (int i = 0; i < childObjects.Length; i++)
                {
                    childObjects[i] = spawnpoint.transform.GetChild(i).gameObject;
                }
                foreach (GameObject childObject in childObjects)
                {
                    Destroy(childObject);
                }
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
            if(spawnTimer <= spawnDelay - 1f)
            {
                for (int i = 0; i < enemiesSpawnDirect.Length; i++)
                {
                    Instantiate(enemiesSpawnDirect[i], spawnPoints[spawnPoints.Length - i - 1].transform.position, Quaternion.identity, spawnPoints[spawnPoints.Length - i - 1].transform);
                    spawnCollider.size = new Vector2(spawnCollider.size.x + 7, spawnCollider.size.y + 7);
                    StartCoroutine(EnemySpawnAllAnim());
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

        if (collision.CompareTag("Player"))
        {
            foreach (GameObject spawnpoint in spawnPoints)
            {
                GameObject[] childObjects = new GameObject[spawnpoint.transform.childCount];
                for (int i = 0; i < spawnpoint.transform.childCount; i++)
                {
                    childObjects[i] = spawnpoint.transform.GetChild(i).gameObject;
                }

                foreach (GameObject childObject in childObjects)
                {
                    Destroy(childObject);
                }
            }
        }
    }



    private IEnumerator StopIsHurtAnim()
    {
        yield return new WaitForSeconds(.4f);
        isHurtAnim = false;
        anim.SetBool("isHurt", false);
    }
}
