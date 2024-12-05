using UnityEngine;

public enum DoorState
{
    Closed,
    Opened
}

public class Door : MonoBehaviour
{
    [Header("Door State")]
    [SerializeField] private DoorState doorState;

    [Header("Door Settings")]
    [SerializeField] private bool useState = false;

    private HingeJoint joint;
    private JointLimits jointLimits;

    private void Start()
    {
        joint = GetComponentInChildren<HingeJoint>();
        jointLimits = joint.limits;

        Initialize();
    }

    private void Update()
    {

    }

    private void Initialize()
    {
        if (useState)
        {
            JointLimits limits = joint.limits;
            limits.min = 0;
            limits.max = 0;
            limits.bounciness = 0;
            limits.bounceMinVelocity = 0;

            joint.limits = limits;
        }
    }

    public void Interact(Vector3 playerPosition)
    {
        if (doorState == DoorState.Closed) 
        {
            OpenDoor(playerPosition);
        }
    }

    private void OpenDoor(Vector3 playerPosition)
    {
        Vector3 doorToPlayer = playerPosition - transform.position;
        doorToPlayer.y = 0;

        float direction = Vector3.Dot(transform.right, doorToPlayer.normalized);

        if (direction > 0)
        {
            // Player is on the right, open door to the left
            jointLimits.min = -90;
            jointLimits.max = 0;
        }
        else
        {
            // Player is on the left, open door to the right
            jointLimits.min = 0;
            jointLimits.max = 90;
        }

        joint.limits = jointLimits;
        doorState = DoorState.Opened;
    }
}
