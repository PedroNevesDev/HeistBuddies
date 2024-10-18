using UnityEngine;

public class DetectionModule : AIModule
{
    [Header("General Settings")]
    [SerializeField] private float fieldOfView = 110f;

    [Header("Player Detection Settings")]
    [SerializeField] private float detectionRadius = 15f;

    [Header("Grabbing Detection Settings")]
    [SerializeField] private float grabbingRadius = 15f;

    [Header("Layers")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    public Transform DetectedPlayer { get; private set; }

    public Vector3 LastKnownPlayerPosition { get; private set; } = Vector3.zero;

    public bool IsPlayerVisible { get; private set; }

    public bool IsPlayerGrabbable { get; private set; }

    private AIBrain brain;

    public override void InitializeModule(AIBrain brain)
    {
        this.brain = brain;
    }

    private void Update()
    {
        HandleDetection();
    }

    private void HandleDetection()
    {
        DetectedPlayer = null;
        IsPlayerVisible = false;
        IsPlayerGrabbable = false;

        float maxRadius = Mathf.Max(detectionRadius, grabbingRadius);
        Collider[] hits = Physics.OverlapSphere(transform.position, maxRadius, playerLayer);

        foreach (Collider hit in hits)
        {
            Transform player = hit.transform;
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            // Check line of sight
            if (CheckLineOfSight(player.position))
            {
                // Check for grabbing range first
                if (distanceToPlayer <= grabbingRadius && angleToPlayer < fieldOfView / 2f)
                {
                    IsPlayerGrabbable = true;
                    DetectedPlayer = player;
                    Debug.Log("Player is within grabbing range!");
                    return; // Early return since grabbing takes precedence
                }

                // Check for detection range and field of view
                if (distanceToPlayer <= detectionRadius && angleToPlayer < fieldOfView / 2f)
                {
                    IsPlayerVisible = true;
                    DetectedPlayer = player;
                    LastKnownPlayerPosition = player.position;
                    Debug.Log("Player detected and visible!");
                    // Continue to check other players if needed
                }
                else
                {
                    Debug.Log("Player is outside the guard's FOV.");
                }
            }
            else
            {
                Debug.Log("Player detected but blocked by an obstacle.");
            }
        }
    }

    private bool CheckLineOfSight(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleLayer))
        {
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        // Draw Detection Radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw Grabbing Radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, grabbingRadius);

        // Draw Field of View
        Gizmos.color = Color.blue;
        Vector3 leftFOV = Quaternion.Euler(0, -fieldOfView / 2f, 0) * transform.forward;
        Vector3 rightFOV = Quaternion.Euler(0, fieldOfView / 2f, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftFOV * detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightFOV * detectionRadius);
    }
}
