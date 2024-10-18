using UnityEngine;
using UnityEngine.AI;

public class PatrolState : AIState
{
    public override AIStateType StateType => AIStateType.Patrol;

    [Header("State Settings")]
    public Transform[] patrolPoints;
    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;

    [Header("Idle Settings")]
    [SerializeField] private Vector2 idleDurationRange = new Vector2(2f, 5f); // Idle between 2 and 5 seconds
    [SerializeField, Range(0f, 1f)] private float idleChance = 0.5f; // 50% chance to idle
    private bool isIdling = false;
    private float idleTimer = 0f;
    private float idleDuration = 0f;

    [Header("Vision Settings")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float fieldOfView = 110f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;
    private Transform player;

    private bool playerDetected = false;
    private bool playerVisible = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public override void OnStateEnter()
    {
        agent.isStopped = false;
        isIdling = false;
        MoveToNextPatrolPoint();
    }

    public override void OnStateUpdate()
    {
        if (isIdling)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDuration)
            {
                isIdling = false;
                MoveToNextPatrolPoint();
            }
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                float randomValue = Random.value; // Random.value returns a float between 0.0 and 1.0
                if (randomValue < idleChance)
                {
                    StartIdling();
                }
                else
                {
                    MoveToNextPatrolPoint();
                }
            }
        }

        HandleDetection();
    }

    public override void OnStateExit()
    {
        agent.isStopped = true;
        isIdling = false;
    }

    private void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        int nextPatrolIndex = Random.Range(0, patrolPoints.Length);

        // Ensure we don't pick the same point we're currently at
        while (nextPatrolIndex == currentPatrolIndex && patrolPoints.Length > 1)
        {
            nextPatrolIndex = Random.Range(0, patrolPoints.Length);
        }

        currentPatrolIndex = nextPatrolIndex;
        agent.destination = patrolPoints[currentPatrolIndex].position;
        agent.isStopped = false;
    }

    private void StartIdling()
    {
        isIdling = true;
        idleTimer = 0f;
        idleDuration = Random.Range(idleDurationRange.x, idleDurationRange.y);
        agent.isStopped = true;
    }

    private void HandleDetection()
    {
        playerDetected = false;
        playerVisible = false;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        foreach (Collider hit in hits)
        {
            player = hit.transform;
            playerDetected = true;

            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer < fieldOfView / 2f)
            {
                if (CheckLineOfSight(player.position))
                {
                    playerVisible = true;
                    Debug.Log("Player detected and visible!");
                }
                else
                {
                    Debug.Log("Player detected but blocked by an obstacle.");
                }
            }
            else
            {
                Debug.Log("Player is outside the guard's FOV.");
            }
        }

        if (playerDetected)
        {
            if (playerVisible)
            {
                Debug.DrawLine(transform.position, player.position, Color.green);
            }
            else
            {
                Debug.DrawLine(transform.position, player.position, Color.red);
            }
        }
    }

    private bool CheckLineOfSight(Vector3 targetPosition)
    {
        Vector3 directionToPlayer = (targetPosition - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, targetPosition);

        if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer))
        {
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.blue;
        Vector3 leftFOV = Quaternion.Euler(0, -fieldOfView / 2f, 0) * transform.forward;
        Vector3 rightFOV = Quaternion.Euler(0, fieldOfView / 2f, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftFOV * detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightFOV * detectionRadius);
    }
}
