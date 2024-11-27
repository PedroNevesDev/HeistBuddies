using UnityEngine;

public enum DoorState
{
    Closed,
    Opened
}

public class Door : MonoBehaviour, IInteractable
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

    public void Interact()
    {
        if (doorState == DoorState.Closed) 
        { 
            doorState = DoorState.Opened;
            joint.limits = jointLimits;
        }
    }
}
