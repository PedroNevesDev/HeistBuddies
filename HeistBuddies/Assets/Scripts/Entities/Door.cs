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

    [Header("Door Interact")]
    [SerializeField] int howManyAttempsUntilUnlocked;
    [SerializeField] LockPicking lockPicking;
    [SerializeField] string doorInteractionName = "Lockpick";
    public bool ShouldBeInteractedWith{ get =>doorState == DoorState.Closed;}
    public bool IsBeingInteractedWith{ get =>lockPicking!=null&&lockPicking.gameObject.activeSelf;}
    public string InteractionName{ get => doorInteractionName;}
    public GameObject GetGameObject{ get=> gameObject;}

    int currentAttemps;

    Vector3 currentPlayerPosition;
    private void Start()
    {
        joint = GetComponentInChildren<HingeJoint>();
        jointLimits = joint.limits;

        Initialize();
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

    public void Interact(PlayerController playerController)
    {
        if (ShouldBeInteractedWith) 
        {
            if(IsBeingInteractedWith)
            {
                lockPicking.CheckForOverlap();
            }
            else
            {
                currentPlayerPosition = playerController.transform.position;
                lockPicking.OnSuccess += OpenDoor;
                lockPicking.gameObject.SetActive(true);
            }
        }
    }

    

    public void StopInteraction()
    {
        lockPicking.OnSuccess -= OpenDoor;
        lockPicking.gameObject.SetActive(false);
        print("triggered");
    }

    public void OpenDoor()
    {
        Vector3 doorToPlayer = currentPlayerPosition - transform.position;
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
        currentPlayerPosition = Vector3.zero;
    }

}
