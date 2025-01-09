using System.Runtime.InteropServices.WindowsRuntime;
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

    [Header("Door Interact")]
    [SerializeField]bool canInteract;
    [SerializeField] PlayerGrabbedEvent playerGrabbedEvent;
    [SerializeField] LockPicking lockPicking;
    [SerializeField] string doorInteractionName = "Lockpick";

    public string InteractionName{ get => doorInteractionName;}
    public GameObject GetGameObject{ get=> gameObject;}
    public bool UseState { get => useState; set => useState = value; }

    string cantBeLockpickedBy;
    string whoIsInteracting;
    public string WhoIsInteracting { get => whoIsInteracting; set => whoIsInteracting = value; }
    JointLimits openedLimits;
    JointLimits closedLimits;

    public bool CanInteract(PlayerController player)
    {
        string playerName = player.SkinnedMeshRenderer.sharedMesh.name;
        bool shouldPlayerBeBlockedFromInteracting = cantBeLockpickedBy==playerName;
        bool itsTheSamePlayer = whoIsInteracting==null||whoIsInteracting==playerName;
        return doorState!=DoorState.Opened&&!shouldPlayerBeBlockedFromInteracting && itsTheSamePlayer;
    }
    private void Start()
    {
        joint = GetComponent<HingeJoint>();

        InitializeLimits();

        UpdateState();

        playerGrabbedEvent?.Subscribe(LockDoorAfterPlayerWasGrabbed);
    }
    private void InitializeLimits()
    {
            closedLimits = joint.limits;
            closedLimits.min = 0;
            closedLimits.max = 0;
            closedLimits.bounciness = 0;
            closedLimits.bounceMinVelocity = 0;

            openedLimits = joint.limits;
            openedLimits.min = -90;
            openedLimits.max = 90;
            openedLimits.bounciness = 0;
            openedLimits.bounceMinVelocity = 0;
    }
    public void ChangeState(DoorState state)
    {
        doorState = useState?state:DoorState.Opened;
        UpdateState();
    }
    void UpdateState()
    {
        if(joint)
        switch(doorState)
        {
            case DoorState.Closed:
            joint.limits = closedLimits;
            break;
            case DoorState.Opened:
            joint.limits = openedLimits;
            break;
        }
    }
    public void Interact(PlayerController player)
    {
        if(whoIsInteracting==null)
        {
            whoIsInteracting = player.SkinnedMeshRenderer.sharedMesh.name;;
        }
        if(!CanInteract(player))return;
        
        if(lockPicking.gameObject.activeSelf==true)
        {
            lockPicking.CheckForOverlap();
        }
        else
        {
            lockPicking.OnSuccess += OpenDoor;
            lockPicking.gameObject.SetActive(true);
        }
    }

    

    public void StopInteraction()
    {
        lockPicking.OnSuccess -= OpenDoor;
        lockPicking.gameObject.SetActive(false);
        whoIsInteracting = null;
        print("Stop");
    }

    public void OpenDoor()
    {
        ChangeState(DoorState.Opened);
        cantBeLockpickedBy = null;
    }

    public void LockDoorAfterPlayerWasGrabbed(EventData eventData)
    {
        ChangeState(DoorState.Closed);
        if(eventData is PlayerControllerEventData data)
        {
            cantBeLockpickedBy = data.PlayerController.SkinnedMeshRenderer.sharedMesh.name;
        }
    }

}
