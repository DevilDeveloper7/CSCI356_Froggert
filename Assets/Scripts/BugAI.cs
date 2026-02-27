using UnityEngine;

public class BugAI : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player; // Reference to the frog
    
    [Header("Movement Settings")]
    public float flySpeed = 3f;
    public float escapeSpeed = 6f;
    public float rotationSpeed = 5f;
    
    [Header("Behavior Settings")]
    public float detectionRadius = 5f; // How close the frog needs to be to trigger escape
    public float escapeDistance = 8f; // How far to flee before returning to wandering
    public float wanderRadius = 10f; // How far from spawn point to wander
    public float wanderChangeInterval = 3f; // How often to change wander direction
    
    [Header("Height Settings")]
    public float minHeight = 0.5f;  // Much lower - easier to catch
    public float maxHeight = 1.5f;  // Lower max height
    public float heightChangeSpeed = 2f;
    
    [Header("Animation")]
    private Animator animator;
    
    private Vector3 spawnPoint;
    private Vector3 targetPosition;
    private float wanderTimer;
    private bool isEscaping = false;
    private float currentHeight;
    
    void Start()
    {
        spawnPoint = transform.position;
        currentHeight = Random.Range(minHeight, maxHeight);
        wanderTimer = wanderChangeInterval;

        animator = GetComponent<Animator>();

        // Start with takeoff animation, then fly
        if (animator != null)
        {
            animator.SetBool("idle", false);
            animator.SetBool("walk", false);
            animator.SetBool("takeoff", true);
            // After a short delay, switch to fly
            Invoke("StartFlying", 1f);
        }

        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                // Try to find by name
                playerObj = GameObject.Find("Frog");
                if (playerObj == null)
                {
                    playerObj = GameObject.Find("FrogKing");
                }
                if (playerObj != null)
                {
                    player = playerObj.transform;
                }
            }
        }

        ChooseNewWanderTarget();
    }

    void StartFlying()
    {
        if (animator != null)
        {
            animator.SetBool("takeoff", false);
            animator.SetBool("fly", true);
        }
    }
    
void Update()
    {
        // Check if player exists and is close
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Check if player is too close
            if (distanceToPlayer < detectionRadius)
            {
                // ESCAPE MODE
                if (!isEscaping)
                {
                    isEscaping = true;
                }
                Escape();
            }
            else if (isEscaping && distanceToPlayer > escapeDistance)
            {
                // Safe distance reached, return to wandering
                isEscaping = false;
                ChooseNewWanderTarget();
            }
            else if (!isEscaping)
            {
                // WANDER MODE - move around independently
                Wander();
            }
        }
        else
        {
            // No player found, just wander independently
            Wander();
        }

        // Keep bug at appropriate height
        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, currentHeight, heightChangeSpeed * Time.deltaTime);
        transform.position = pos;
    }
    
    void Escape()
    {
        // Fly away from player
        Vector3 escapeDirection = (transform.position - player.position).normalized;
        targetPosition = transform.position + escapeDirection * 5f;
        targetPosition.y = currentHeight;
        
        MoveTowards(targetPosition, escapeSpeed);
    }
    
    void Wander()
    {
        wanderTimer -= Time.deltaTime;
        
        if (wanderTimer <= 0f || Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            ChooseNewWanderTarget();
            wanderTimer = wanderChangeInterval;
        }
        
        MoveTowards(targetPosition, flySpeed);
    }
    
    void ChooseNewWanderTarget()
    {
        // Choose random point within wander radius of spawn point
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        targetPosition = spawnPoint + new Vector3(randomCircle.x, 0f, randomCircle.y);
        currentHeight = Random.Range(minHeight, maxHeight);
        targetPosition.y = currentHeight;
    }
    
    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        
        // Rotate to face movement direction
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        // Move towards target
        transform.position += direction * speed * Time.deltaTime;
    }
    
    // Draw detection and escape radius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, escapeDistance);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(spawnPoint, wanderRadius);
    }
}
