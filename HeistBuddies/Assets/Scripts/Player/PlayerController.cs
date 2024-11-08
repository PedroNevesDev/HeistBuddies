using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]  float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 1.5f;
    [SerializeField] private float maxSpeedMagnitude;
    [SerializeField] private bool canMove;
    [SerializeField] Rigidbody rb;


    // Specific transform that will be used to follow the movement direction ex:mesh. Could be anything
    [SerializeField] Transform orientation; 
    
    // Animator of the animated rig. Lets you change the animation
    [SerializeField] Animator animator; 

    // All bodyparts can be here if needed. Used to store a reference into player controller owner
    [SerializeField,Tooltip("Assigns this character reference to every body part script added here. Makes searching for collisions easier")] BodyPartOwner[] bodyParts; 
    

    
    private Camera myCam; 
    
	[Header("Grab Detection Box Settings")]
    [SerializeField] private Vector3 boxSize = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 boxOffset = new Vector3(0, 0, 1);
    [SerializeField] private LayerMask detectionLayer;
    
    private IGrabbable currentGrabbable;

    private PlayerBackpack _Backpack;

    public bool CanMove { get => canMove; set => canMove = value; }
    public Rigidbody Rb { get => rb; set => rb = value; }
    public PlayerBackpack Backpack { get => _Backpack; set => _Backpack = value; }


    Vector2 moveInput = Vector2.zero;
    bool isGrabbing = false;
    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    public void OnGrab(InputAction.CallbackContext context) => isGrabbing = context.ReadValue<float>()>0;

    private void Start()
    {
        myCam = Camera.main;
        _Backpack = GetComponent<PlayerBackpack>();
        foreach(BodyPartOwner bodyPart in bodyParts)
        {
            bodyPart.MyOwner = this;
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        Move();
        SpeedControl();
        CheckForGrabbable();
        Grab();
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        if (flatVel.magnitude > maxSpeedMagnitude)
        {
            Vector3 limitVel = flatVel.normalized * maxSpeedMagnitude;
            rb.linearVelocity = new Vector3(limitVel.x, rb.linearVelocity.y, limitVel.z);
        }
    }
    private void Move()
    {
        //Getting the main camera directions
        Quaternion rotation = Quaternion.Euler(0,Camera.main.transform.rotation.eulerAngles.y,0); 
        
        // Applying the camera directions to the the inputs to get a better feel
        Vector3 move = (rotation*new Vector3(moveInput.x, 0, moveInput.y)).normalized; 
        
        //Transform to local space
        move = transform.TransformDirection(move); 

        // Checking if there is indead movement input from the player
        if(move!=Vector3.zero) 
        {

            //Get Target Rotation
            Quaternion targetRotation = Quaternion.LookRotation((orientation.transform.position+move)-orientation.transform.position);

            //Rotate smoothly to this target
            orientation.rotation = Quaternion.Slerp(orientation.rotation, targetRotation, rotationSpeed*Time.fixedDeltaTime);

            // Makes the character go into Walk animation
            animator.SetBool("Moving",true); 
        }
        else
        {
            // Makes the character go into Idle animation
            animator.SetBool("Moving",false); 
        }
        
        // Applying the move direction to the rigidbody
        rb.AddForce(move * moveSpeed,ForceMode.VelocityChange); 
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
        if(currentGrabbable == null)
            return;
        if(isGrabbing == false)
            return;
    
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
