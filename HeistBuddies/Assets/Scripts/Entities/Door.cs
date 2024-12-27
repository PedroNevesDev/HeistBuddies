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
    [SerializeField] bool useInteraction;
    [SerializeField] int howManyAttempsUntilUnlocked;
    [SerializeField] LockPicking lockPicking;
    [SerializeField] string doorInteractionName = "Lockpick";
    public bool ShouldBeInteractedWith{ get =>useInteraction&&doorState == DoorState.Closed;}
    public bool IsBeingInteractedWith{ get =>lockPicking!=null&&lockPicking.gameObject.activeSelf;}
    public string InteractionName{ get => doorInteractionName;}
    public GameObject GetGameObject{ get=> gameObject;}
    public bool UseState { get => useState; set => useState = value; }

    int currentAttemps;

    Vector3 currentPlayerPosition;
    private void Start()
    {
        joint = GetComponent<HingeJoint>();
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

    public void Interact()
    {
        if(!lockPicking) return;
        if (ShouldBeInteractedWith) 
        {
            if(IsBeingInteractedWith)
            {
                lockPicking.CheckForOverlap();
            }
            else
            {
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


        jointLimits.min = -90;
        jointLimits.max = 90;
        joint.limits = jointLimits;
        doorState = DoorState.Opened;
        currentPlayerPosition = Vector3.zero;
    }

}
