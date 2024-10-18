using UnityEngine;

public class HearingModule : AIModule
{
    [Header("Hearing Settings")]
    [SerializeField] private float hearingRadius = 20f;
    [SerializeField] private LayerMask obstacleLayer;

    private AIBrain brain;
    public Vector3 SoundSourcePosition { get; private set; }
    public bool HasHeardSound { get; private set; }

    public override void InitializeModule(AIBrain brain)
    {
        this.brain = brain;
    }

    public void OnSoundHeard(Vector3 soundPosition)
    {
        HasHeardSound = false;
        SoundSourcePosition = Vector3.zero;

        // Check if the soundPosition is within the hearing radius
        float distanceToSound = Vector3.Distance(transform.position, soundPosition);
        if (distanceToSound <= hearingRadius)
        {
            // Check for obstacles blocking the sound
            Vector3 directionToSound = (soundPosition - transform.position).normalized;
            if (!Physics.Raycast(transform.position, directionToSound, distanceToSound, obstacleLayer))
            {
                HasHeardSound = true;
                SoundSourcePosition = soundPosition;
                Debug.Log($"{gameObject.name} heard sound at position: {soundPosition}");
            }
            else
            {
                Debug.Log($"{gameObject.name} sound blocked by obstacle.");
            }
        }
    }

    public void ResetHearing()
    {
        HasHeardSound = false;
        SoundSourcePosition = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        // Draw Hearing Radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}
