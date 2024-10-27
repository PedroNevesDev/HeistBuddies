using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController; // Using CharacterController for 3D movement

    [Header("Player Movement Settings")]
    public float moveSpeed = 5f; // Movement speed
    [SerializeField] private bool canMove;

    [SerializeField] Rigidbody mainBody;
    private Vector3 velocity; // To track the player's velocity

    [SerializeField] private Transform mesh;

    [Header("GroundCheck")]
    bool isGrounded;
    public PlayerInputs myInputs;

    private Camera myCam;

    [SerializeField] Animator animator;

    [Header("Grab Detection Box Settings")]
    [SerializeField] private Vector3 boxSize = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 boxOffset = new Vector3(0, 0, 1);
    [SerializeField] private LayerMask detectionLayer;
    private IGrabbable currentGrabbable;

    private PlayerBackpack _Backpack;

    public bool CanMove { get => canMove; set => canMove = value; }

    private void Start()
    {
        myCam = Camera.main;

        _Backpack = GetComponent<PlayerBackpack>();
    }
    public void EnableInput(PlayerInputs newInputs)
    {
        myInputs=newInputs;
        myInputs.Init();
    }
    void OnDisable()
    {
        myInputs.Cancel();
    }
    private void Awake()
    {
        characterController = GetComponent<CharacterController>(); // Get CharacterController reference
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

    // This function will be called by the Input System when the move action is triggered for Player 1


    private void Move()
    {
        if(!myInputs)
            return;

        //This line creates a rotation equal to the current rotation value of the main camera
        Quaternion rotation = Quaternion.Euler(0,Camera.main.transform.rotation.eulerAngles.y,0);
        //This line applies the rotation to the input direction, making it relative to the camera direction
        Vector3 move = (rotation*new Vector3(myInputs.moveInput.x, 0, myInputs.moveInput.y)).normalized;

        
        move = transform.TransformDirection(move); // Transform to local space
        if(move!=Vector3.zero)
        {
            mesh.forward = Vector3.Lerp(mesh.forward ,move,15f*Time.deltaTime);

            animator.SetBool("Moving",true);
        }
        else
        {
            animator.SetBool("Moving",false);
        }

        mainBody.AddForce(move * moveSpeed * Time.deltaTime,ForceMode.VelocityChange); // Move the character

 
    }

    private void CheckForGrabbable()
    {
        Vector3 boxCenter = mainBody.position + mainBody.transform.TransformDirection(boxOffset);
        Collider[] hits = Physics.OverlapBox(boxCenter, boxSize / 2, mainBody.transform.rotation, detectionLayer);

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
        Vector3 boxCenter = mainBody.transform.position + mainBody.transform.TransformDirection(boxOffset);
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, mainBody.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
