using UnityEngine;

public class DetectionModule : AIModule
{
    [Header("General Settings")]
    [SerializeField] private DetectionType detectionType;

    [Header("Scriptable Events")]
    [SerializeField] private DogBoneReceivedEvent DogBoneReceivedEvent;
    [SerializeField] private PlayerFoundEvent PlayerFoundEvent;
    [SerializeField] private PlayerLostEvent PlayerLostEvent;
    [SerializeField] private PlayerGrabbedEvent PlayerGrabbedEvent;

    [Header("Player Detection Settings")]
    [SerializeField] private float fieldOfView = 110f;
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float grabbingRadius = 15f;

    [Header("Layers")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask itemLayer;

    [Header("Detection Timing")]
    [SerializeField] private float detectionCooldown = 0.1f;
    private float detectionTimer = 0f;

    private Transform detectedPlayer = null;
    private Vector3 lastKnownPlayerPosition = Vector3.zero;
    private PlayerController playerController = null;

    private bool isPlayerVisible = false;
    private bool isPlayerGrabbable = false;

    private bool wasPlayerVisible = false;
    private bool wasPlayerGrabbable = false;
    private bool canDetect = true;

    public Transform DetectedPlayer { get => detectedPlayer; private set => detectedPlayer = value; }
    public Vector3 LastKnownPlayerPosition { get => lastKnownPlayerPosition; private set => lastKnownPlayerPosition = value; }
    public bool CanDetect { get => canDetect; set => canDetect = value; }

    private void Update()
    {
        if (!canDetect) return;

        detectionTimer += Time.deltaTime;
        if (detectionTimer >= detectionCooldown)
        {
            detectionTimer = 0f;
            HandleDetection();
        }
    }

    private void HandleDetection()
    {
        ResetDetectionState();

        LayerMask detectionLayers;
        if (detectionType == DetectionType.Dog)
        {
            detectionLayers = playerLayer | itemLayer;
        }
        else
        {
            detectionLayers = playerLayer;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, Mathf.Max(detectionRadius, grabbingRadius), detectionLayers);

        foreach (Collider hit in hits)
        {
            if (detectionType == DetectionType.Guard)
            {
                if (TryDetectPlayer(hit, out PlayerController detectedPlayerController, out bool isGrabbable))
                {
                    if (isGrabbable)
                    {
                        OnPlayerGrabbable(detectedPlayerController);
                    }
                    else
                    {
                        OnPlayerVisible(detectedPlayerController);
                    }

                    break; // Exit after handling the first detected player
                }
            }
            else if (detectionType == DetectionType.Dog)
            {
                if (TryDetectBone(hit, out Item detectedItem))
                {
                    OnBoneDetected(detectedItem);
                    break; // Exit after handling the first detected bone
                }
                else if (TryDetectPlayer(hit, out PlayerController detectedPlayerController, out _))
                {
                    OnPlayerVisible(detectedPlayerController);
                    break; // Exit after the first detection for Dog
                }
            }
        }

        if (playerController != null && !playerController.WasGrabbed)
        {
            if (wasPlayerVisible && !isPlayerVisible)
            {
                OnPlayerLost();
            }
        }

        UpdateFlagsForNextFrame();
    }

    private void ResetDetectionState()
    {
        detectedPlayer = null;
        isPlayerVisible = false;
        isPlayerGrabbable = false;
    }

    private bool TryDetectPlayer(Collider hit, out PlayerController player, out bool isGrabbable)
    {
        Transform playerTransform = hit.transform;
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        player = playerTransform.GetComponentInParent<PlayerController>();
        isGrabbable = distanceToPlayer <= grabbingRadius && angleToPlayer < fieldOfView / 2f;

        bool isDetected = distanceToPlayer <= detectionRadius && angleToPlayer < fieldOfView / 2f &&
                          AIHelpers.CheckLineOfSight(playerTransform.position, transform, obstacleLayer);

        return isDetected;
    }

    private bool TryDetectBone(Collider hit, out Item item)
    {
        item = null;
        if (hit == null) return false;

        if (((1 << hit.gameObject.layer) & itemLayer.value) == 0)
            return false;

        item = hit.GetComponent<Item>();
        if (item != null && item.Data.name == "Bone")
        {
            if (AIHelpers.CheckLineOfSight(item.transform.position, transform, obstacleLayer))
            {
                return true;
            }
        }
        return false;
    }

    private void OnPlayerGrabbable(PlayerController player)
    {
        isPlayerGrabbable = true;
        detectedPlayer = player.Rb.transform;

        if (!wasPlayerGrabbable)
        {
            var eventData = new PositionEventData(brain, player.Rb.position, player.Rb.transform);
            PlayerGrabbedEvent.Invoke(eventData);
        }
    }

    private void OnPlayerVisible(PlayerController player)
    {
        isPlayerVisible = true;
        detectedPlayer = player.Rb.transform;
        lastKnownPlayerPosition = player.Rb.position;
        brain.SetTargetPlayer(detectedPlayer);

        if (playerController == null)
        {
            playerController = player;
        }

        if (!wasPlayerVisible)
        {
            var eventData = new PositionEventData(brain, player.Rb.position, player.Rb.transform);
            PlayerFoundEvent.Invoke(eventData);
        }
    }

    private void OnPlayerLost()
    {
        var eventData = new PositionEventData(brain, Vector3.zero, null);
        PlayerLostEvent.Invoke(eventData);

        playerController = null;
    }

    private void OnBoneDetected(Item item)
    {
        var eventData = new PositionEventData(brain, item.gameObject.transform.position, item.gameObject.transform);
        DogBoneReceivedEvent.Invoke(eventData);
    }

    private void UpdateFlagsForNextFrame()
    {
        wasPlayerVisible = isPlayerVisible;
        wasPlayerGrabbable = isPlayerGrabbable;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, grabbingRadius);

        Gizmos.color = Color.blue;
        Vector3 leftFOV = Quaternion.Euler(0, -fieldOfView / 2f, 0) * transform.forward;
        Vector3 rightFOV = Quaternion.Euler(0, fieldOfView / 2f, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftFOV * detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightFOV * detectionRadius);
    }
}
