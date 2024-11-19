using UnityEngine;

public class HearingModule : AIModule
{
    [Header("General Settings")]
    [SerializeField] private DetectionType detectionType;

    [Header("Hearing Settings")]
    [SerializeField] private float hearingRadius = 20f;
    [SerializeField] private LayerMask obstacleLayer;

    private Vector3 soundSourcePosition = Vector3.zero;
    private bool hasHeardSound = false;

    public Vector3 SoundSourcePosition { get => soundSourcePosition; private set => soundSourcePosition = value; }
    public bool HasHeardSound { get => hasHeardSound; private set => hasHeardSound = value; }

    public void OnSoundHeard(Vector3 soundPosition)
    {
        hasHeardSound = false;
        soundSourcePosition = Vector3.zero;

        float distanceToSound = Vector3.Distance(transform.position, soundPosition);
        if (distanceToSound <= hearingRadius)
        {
            Vector3 directionToSound = (soundPosition - transform.position).normalized;
            if (!Physics.Raycast(transform.position, directionToSound, distanceToSound, obstacleLayer))
            {
                hasHeardSound = true;
                soundSourcePosition = soundPosition;
                Debug.Log($"{gameObject.name} heard sound at position: {soundPosition}");
            }
            else
            {
                Debug.Log($"{gameObject.name} sound blocked by obstacle.");
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw Hearing Radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}
