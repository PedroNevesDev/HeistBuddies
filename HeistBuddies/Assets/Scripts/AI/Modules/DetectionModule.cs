using UnityEngine;

public class DetectionModule : AIModule
{
    [Header("General Settings")]
    [SerializeField] private float fieldOfView = 110f;

    [Header("Player Detection Settings")]
    [SerializeField] private float detectionRadius = 15f;
    private Transform detectedPlayer = null;
    private Vector3 lastKnownPlayerPosition = Vector3.zero;
    private bool isPlayerVisible = false;
    private bool isPlayerGrabbable = false;

    [Header("Grabbing Detection Settings")]
    [SerializeField] private float grabbingRadius = 15f;

    [Header("Layers")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    private AIBrain brain = null;

    public Transform DetectedPlayer { get => detectedPlayer; private set => detectedPlayer = value; }
    public Vector3 LastKnownPlayerPosition { get => lastKnownPlayerPosition; private set => lastKnownPlayerPosition = value; }
    public bool IsPlayerVisible { get => isPlayerVisible; private set => isPlayerVisible = value; }
    public bool IsPlayerGrabbable { get => isPlayerGrabbable; private set => isPlayerGrabbable = value; }


    public override void Initialize(AIBrain brain)
    {
        this.brain = brain;
    }

    private void Update()
    {
        HandleDetection();
    }

    private void HandleDetection()
    {
        detectedPlayer = null;
        isPlayerVisible = false;
        isPlayerGrabbable = false;

        float maxRadius = Mathf.Max(detectionRadius, grabbingRadius);
        Collider[] hits = Physics.OverlapSphere(transform.position, maxRadius, playerLayer);

        foreach (Collider hit in hits)
        {
            Transform player = hit.transform;
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (AIHelpers.CheckLineOfSight(player.position, transform, obstacleLayer))
            {
                if (distanceToPlayer <= grabbingRadius && angleToPlayer < fieldOfView / 2f)
                {
                    isPlayerGrabbable = true;
                    detectedPlayer = player;
                    Debug.Log("Player is within grabbing range!");
                    return;
                }

                if (distanceToPlayer <= detectionRadius && angleToPlayer < fieldOfView / 2f)
                {
                    isPlayerVisible = true;
                    detectedPlayer = player;
                    lastKnownPlayerPosition = player.position;
                    Debug.Log("Player detected and visible!");
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
