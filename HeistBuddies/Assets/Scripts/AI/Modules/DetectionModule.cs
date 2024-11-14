using UnityEngine;

public enum DetectionType
{
    Guard,
    Dog
}

public class DetectionModule : AIModule
{
    [Header("General Settings")]
    [SerializeField] private DetectionType detectionType;

    [Header("Player Detection Settings")]
    [SerializeField] private float fieldOfView = 110f;
    [SerializeField] private float detectionRadius = 15f;
    private Transform detectedPlayer = null;
    private Vector3 lastKnownPlayerPosition = Vector3.zero;
    private bool isPlayerVisible = false;
    private bool isPlayerGrabbable = false;
    private PlayerController playerController = null;

    [Header("Grabbing Detection Settings")]
    [SerializeField] private float grabbingRadius = 15f;

    [Header("Layers")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    private bool canDetect = true;
    private bool wasPlayerVisible = false;
    private bool wasPlayerGrabbable = false;

    public Transform DetectedPlayer { get => detectedPlayer; private set => detectedPlayer = value; }
    public Vector3 LastKnownPlayerPosition { get => lastKnownPlayerPosition; private set => lastKnownPlayerPosition = value; }
    public bool IsPlayerVisible { get => isPlayerVisible; private set => isPlayerVisible = value; }
    public bool IsPlayerGrabbable { get => isPlayerGrabbable; private set => isPlayerGrabbable = value; }
    public bool CanDetect { get => canDetect; set => canDetect = value; }

    private void Update()
    {
        if (!canDetect) return;

        if (detectionType == DetectionType.Guard)
            HandleDetectionGuard();
        else if (detectionType == DetectionType.Dog)
            HandleDetectionDog();
    }

    private void HandleDetectionGuard()
    {
        detectedPlayer = null;
        isPlayerVisible = false;
        isPlayerGrabbable = false;

        bool playerVisibleThisFrame = false;
        bool playerGrabbableThisFrame = false;

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
                // Check if player is within grabbing range
                if (distanceToPlayer <= grabbingRadius && angleToPlayer < fieldOfView / 2f)
                {
                    playerGrabbableThisFrame = true;

                    isPlayerGrabbable = true;
                    detectedPlayer = player;

                    if (playerController == null)
                    {
                        playerController = player.GetComponentInParent<PlayerController>();
                    }

                    if (!wasPlayerGrabbable)
                    {
                        var eventData = new PlayerEventData(brain, player.position);
                        EventManager.InvokeLocalEvent(LocalEvent.PlayerGrabbed, eventData);
                    }

                    // Exit early since player is grabbed, no need for further checks
                    break;
                }

                // Check if player is within detection range
                if (distanceToPlayer <= detectionRadius && angleToPlayer < fieldOfView / 2f)
                {
                    playerVisibleThisFrame = true;

                    isPlayerVisible = true;
                    detectedPlayer = player;
                    lastKnownPlayerPosition = player.position;
                    brain.SetTargetPlayer(detectedPlayer);

                    if (!wasPlayerVisible)
                    {
                        var eventData = new PlayerEventData(brain, player.position);
                        EventManager.InvokeLocalEvent(LocalEvent.PlayerFound, eventData);
                    }

                    // No need to continue checking other hits once player is detected
                    break;
                }
            }
        }

        if (playerController != null && !playerController.WasGrabbed)
        {
            if (wasPlayerVisible && !playerVisibleThisFrame)
            {
                var eventData = new PlayerEventData(brain, Vector3.zero);
                EventManager.InvokeLocalEvent(LocalEvent.PlayerLost, eventData);
            }
        }

        // Update flags for the next frame
        wasPlayerVisible = playerVisibleThisFrame;
        wasPlayerGrabbable = playerGrabbableThisFrame;
    }

    private void HandleDetectionDog()
    {
        detectedPlayer = null;
        isPlayerVisible = false;
        isPlayerGrabbable = false;

        bool playerVisibleThisFrame = false;

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
                // Check if player is within detection range
                if (distanceToPlayer <= detectionRadius && angleToPlayer < fieldOfView / 2f)
                {
                    playerVisibleThisFrame = true;

                    isPlayerVisible = true;
                    detectedPlayer = player;
                    lastKnownPlayerPosition = player.position;
                    brain.SetTargetPlayer(detectedPlayer);

                    if (!wasPlayerVisible)
                    {
                        var eventData = new PlayerEventData(brain, player.position);
                        EventManager.InvokeLocalEvent(LocalEvent.PlayerFound, eventData);
                    }

                    // No need to continue checking other hits once player is detected
                    break;
                }
            }
        }

        // Update flags for the next frame
        wasPlayerVisible = playerVisibleThisFrame;
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
