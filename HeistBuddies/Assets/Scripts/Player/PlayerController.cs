using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody rb; // Rigidbody used for the player movement
    [SerializeField]  float moveSpeed = 5f; // Movement speed
    [SerializeField] private bool canMove;
    [SerializeField] Transform orientation; // Specific transform that will be used to follow the movement direction ex:mesh. Could be anything 
    [SerializeField] Animator animator; // Animator of the animated rig. Lets you change the animation

    [SerializeField]BodyPartOwner[] bodyParts; // All bodyparts can be here if needed. Used to store a reference into player controller owner
    PlayerInputs myInputs; // SO with the player inputs
    private Camera myCam; // Variable mean't to save up the main camera
    
	[Header("Grab Detection Box Settings")]
    [SerializeField] private Vector3 boxSize = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 boxOffset = new Vector3(0, 0, 1);
    [SerializeField] private LayerMask detectionLayer;
    private IGrabbable currentGrabbable;

    private PlayerBackpack _Backpack;

    public bool CanMove { get => canMove; set => canMove = value; }
    public Rigidbody Rb { get => rb; set => rb = value; }
    public PlayerBackpack Backpack { get => _Backpack; set => _Backpack = value; }

    private void Start()
    {
        myCam = Camera.main; // Getting main camera for latter use
        _Backpack = GetComponent<PlayerBackpack>();
        foreach(BodyPartOwner bodyPart in bodyParts)
        {
            bodyPart.MyOwner = this;
        }
    }
    public void EnableInput(PlayerInputs newInputs) // Called by the controller listener to assign and change player inputs when needed. Importent for changing between singleplayer inputs and multyplayer inputs
    {
        if(myInputs)
            myInputs.Cancel(); // Deactivate old one

        myInputs=newInputs; // Assign new one

        myInputs.Init(); // Activate new one
    }

    private void Update()
    {
                if (!canMove) return;

        Move(); // Call Move method every frame
        CheckForGrabbable();

        if (myInputs.isGrabbing && currentGrabbable != null)
        {
            Grab();
        }
    }
    private void Move()
    {
        if(!myInputs) // Leaves the function if there is no input scriptable object linked to this character
            return;

        Quaternion rotation = Quaternion.Euler(0,Camera.main.transform.rotation.eulerAngles.y,0); //Getting the main camera directions
        
        Vector3 move = (rotation*new Vector3(myInputs.moveInput.x, 0, myInputs.moveInput.y)).normalized; // Applying the camera directions to the the inputs to get a better feel

        move = transform.TransformDirection(move); //Transform to local space

        if(move!=Vector3.zero) // Checking if there is indead movement input from the player
        {
            orientation.forward = Vector3.Lerp(orientation.forward ,move,15f*Time.deltaTime); // Applying rotation to the mesh so that it faces the movement direction. lerping by 15f
            animator.SetBool("Moving",true); // Makes the character go into Walk animation
        }
        else
        {
            animator.SetBool("Moving",false); // Makes the character go into Idle animation
        }

        rb.AddForce(move * moveSpeed * Time.deltaTime,ForceMode.VelocityChange); // Applying the move direction to the rigidbody
    }
    private void CheckForGrabbable()
    {
        Vector3 boxCenter = rb.position + rb.transform.TransformDirection(boxOffset);
        Collider[] hits = Physics.OverlapBox(boxCenter, boxSize / 2, rb.transform.rotation, detectionLayer);

        if (hits.Length > 0)
        {
            IGrabbable grabbable = hits[0].GetComponent<IGrabbable>();
            if (grabbable != null)
            {
                if (currentGrabbable != grabbable)
                {
                    if (currentGrabbable != null)
                    {
                        currentGrabbable.DisableUI();
                    }

                    currentGrabbable = grabbable;
                    currentGrabbable.EnableUI();
                }
                return;
            }
        }

        if (currentGrabbable != null)
        {
            currentGrabbable.DisableUI();
            currentGrabbable = null;
        }
    }

    private void Grab()
    {
        Item item = currentGrabbable as Item;
        _Backpack.AddItemToBackPack(item);

        currentGrabbable.Grab();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 boxCenter = rb.transform.position + rb.transform.TransformDirection(boxOffset);
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, rb.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
