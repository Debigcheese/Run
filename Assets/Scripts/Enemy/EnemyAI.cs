using System.Collections;
using Pathfinding;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private PlayerMovement playerMovement;

    [Header("Pathfinding")]
    public Transform target;
    private Vector3 targetPos;
    public float detectionRadius = 50f;
    public float pathUpdateSeconds = 0.5f;
    private float originalDetectionRadius;

    [Header("Physics")]
    public float nextWaypointDistance = 3f;
    public float speed = 200f, jumpForce = 100f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpModifier = 0.3f;
    public float jumpCheckOffset = 0.1f;
    private float minVelocity_Y = 8f;

    [Header("Custom Behavior")]
    public bool waveSpawnerEnemies = false;
    public bool followEnabled = true;
    public bool jumpEnabled = true, isJumping, isInAir;
    public bool directionLookEnabled = true;
    public bool canMove = true;
    private bool ledgeDetected = false;
    public bool ledgeCheck;
    public Collider2D ledgeCollider;

    [Header("Custom Behavior Flying Enemy")]
    public bool isFlyingEnemy = false;
    public float abovePlayerYAxis = 4f;
    private float abovePlayerYAxisTimer = 0f;
    private bool abovePlayerYAxisBool = false;

    [Header("Jump Behavior")]
    public Transform jumpCheck;
    public LayerMask GroundLayer;
    public float jumpDetectionRadius = 10f;

    [Space]
    public bool isMoving;
    public bool lookingRight;
    public bool detected = false;
    public bool tookDamageDetect;
    public GameObject enemyDetectionAnim;

    [SerializeField] Vector3 startOffset;

    private Path path;
    private int currentWaypoint = 0;
    [SerializeField] public RaycastHit2D isGrounded;
    Seeker seeker;
    Rigidbody2D rb;
    Animator anim;
    private bool isOnCoolDown;

    public void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        anim = enemyDetectionAnim.GetComponent<Animator>();
        target = FindObjectOfType<PlayerMovement>().transform.GetComponent<Transform>();
        isJumping = false;
        isInAir = false;
        isOnCoolDown = false;
        enemyDetectionAnim.SetActive(false);
        originalDetectionRadius = detectionRadius;

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void Update()
    {
        if (isFlyingEnemy)
        {
            abovePlayerYAxisTimer += Time.deltaTime;
            if(abovePlayerYAxisTimer >= 1.5f && abovePlayerYAxisBool)
            {
                abovePlayerYAxis += .5f;
                abovePlayerYAxisBool = false;
                abovePlayerYAxisTimer = 0f;
            }
            if (abovePlayerYAxisTimer >= 1.5f && !abovePlayerYAxisBool)
            {
                abovePlayerYAxis -= .5f;
                abovePlayerYAxisBool = true;
                abovePlayerYAxisTimer = 0f;
            }
        }

        if (ledgeCheck)
        {
            LedgeCheck();
        }

        targetPos = target.position;

        if ((TargetInDistance() || tookDamageDetect) && followEnabled && !waveSpawnerEnemies)
        {
            detectionRadius = (originalDetectionRadius * 1.3f);
            detected = true;
            enemyDetectionAnim.SetActive(true);
            PathFollow();
        }
        else
        {
            detectionRadius = originalDetectionRadius;
            enemyDetectionAnim.SetActive(false);
            isMoving = false;
            detected = false;
        }

        if (waveSpawnerEnemies && followEnabled)
        {
            detected = true;
            enemyDetectionAnim.SetActive(false);
            PathFollow();
            detectionRadius = 40;
        }
        
        //minimum velocity_Y so enemies dont fall through map
        if(rb.velocity.y <= -minVelocity_Y && !isFlyingEnemy)
        {
            rb.velocity = new Vector2(rb.velocity.x, -minVelocity_Y);
        }

        anim.SetBool("Detected", detected);
    }

    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, targetPos, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        // See if colliding with anything
        startOffset = transform.position - new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset, transform.position.z);
        isGrounded = Physics2D.Raycast(startOffset, -Vector3.up, 0.05f);

        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed;

        // Jump
        if (jumpEnabled && isGrounded && !isInAir && !isOnCoolDown && JumpCheck())
        {
            if (isInAir) return;
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            StartCoroutine(JumpCoolDown());
        }
        if (isGrounded)
        {
            isJumping = false;
            isInAir = false;
        }
        else
        {
            isInAir = true;
        }

        // Movement
        if (!isFlyingEnemy)
        {
            if (canMove && !ledgeDetected)
            {
                rb.velocity = new Vector2(force.x, rb.velocity.y);
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }
        else
        {
            if (canMove && transform.position.y >= playerMovement.transform.position.y + abovePlayerYAxis)
            {
                rb.velocity = new Vector2(force.x, force.y / 2.5f);
                isMoving = true;
            }
            else if (canMove && transform.position.y < playerMovement.transform.position.y + abovePlayerYAxis)
            {
                rb.velocity = new Vector2(force.x, speed / 2.5f);
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }



        // Next Waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // Direction Graphics Handling
        if (directionLookEnabled)
        {
            if (playerMovement.transform.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                lookingRight = true;
            }
            else if (playerMovement.transform.position.x < transform.position.x)
            {
                lookingRight = false;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, targetPos) < detectionRadius;
    }

    private bool JumpCheck()
    {
        return Physics2D.OverlapCircle(jumpCheck.position, jumpDetectionRadius, GroundLayer);
    }

    private void LedgeCheck()
    {
        if (!ledgeCollider.IsTouchingLayers(GroundLayer) && !isFlyingEnemy)
        {
            ledgeDetected = true;
        }
        else if (ledgeCollider.IsTouchingLayers(GroundLayer) && !isFlyingEnemy)
        {
            ledgeDetected = false;
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    IEnumerator JumpCoolDown()
    {
        isOnCoolDown = true;
        yield return new WaitForSeconds(1f);
        isOnCoolDown = false;
    }

    private void OnDrawGizmosSelected()
    {
        // Draws a sphere around the enemy to represent the detection radius
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireSphere(jumpCheck.position, jumpDetectionRadius);
    }
}