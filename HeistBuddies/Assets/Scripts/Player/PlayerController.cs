using System.Collections;
using System.Collections.Generic;
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

    [Header("Player Conditions")]
    [SerializeField] private bool wasTeleported = false;
    [SerializeField] private bool wasGrabbed = false;

    public bool WasGrabbed { get => wasGrabbed; set => wasGrabbed = value; }
    public bool WasTeleported { get => wasTeleported; set => wasTeleported = value; }

    // Specific transform that will be used to follow the movement direction ex:mesh. Could be anything
    [SerializeField] Transform orientation; 
    
    // Animator of the animated rig. Lets you change the animation
    [SerializeField] Animator animator; 

    // All bodyparts can be here if needed. Used to store a reference into player controller owner
    [SerializeField,Tooltip("Assigns this character reference to every body part script added here. Makes searching for collisions easier")] 
    BodyPartOwner[] bodyParts;

    private List<float> playerBodyParts = new List<float>();
    

    
    private Camera myCam; 
    
	[Header("Grab Detection Box Settings")]
    [SerializeField] private Vector3 boxSize = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 boxOffset = new Vector3(0, 0, 1);
    [SerializeField] private LayerMask detectionLayer;

    private Transform teleportPoint = null;
    private Coroutine teleportCoroutine = null;

    private IGrabbable currentGrabbable = null;

    private PlayerBackpack backpack = null;


    public bool CanMove { get => canMove; set => canMove = value; }
    public Rigidbody Rb { get => rb; set => rb = value; }
    public PlayerBackpack Backpack { get => backpack; set => backpack = value; }
    public BodyPartOwner[] BodyParts { get => bodyParts; set => bodyParts = value; }

    Vector2 moveInput = Vector2.zero;
    bool isGrabbing = false;

    public Balance balance;
    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    public void OnGrab(InputAction.CallbackContext context) => isGrabbing = context.ReadValue<float>()>0;

    private void Start()
    {
        myCam = Camera.main;
        backpack = GetComponent<PlayerBackpack>();

        teleportPoint = GameObject.Find("TeleportPoint").transform;

        foreach(BodyPartOwner bodyPart in BodyParts)
        {
            bodyPart.MyOwner = this;

            // Cache rigidbody mass
            Rigidbody rigidbody = bodyPart.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                playerBodyParts.Add(rigidbody.mass);
            }
        }
    }

    private void Update()
    {
        CheckForGrabbable();
        Grab();

        if (wasTeleported)
        {
            StopCoroutine(teleportCoroutine);
            wasTeleported = false;
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        Move();
        SpeedControl();
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
        backpack.AddItemToBackPack(item);

        currentGrabbable.Grab();
    }

    #region Player BodyParts

    public void ResetPlayerBodyParts()
    {
        for (int i = 0; i < bodyParts.Length; i++) 
        {
            Rigidbody rigidbody = bodyParts[i].GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.mass = playerBodyParts[i];
            }
        }
    }

    public void ChangePlayerBodyParts()
    {
        for (int i = 0; i < bodyParts.Length; i++)
        {
            Rigidbody rigidbody = bodyParts[i].GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.mass = 1f;
            }
        }
    }

    #endregion

    #region Player Movement

    public void LaunchPlayer(float forceUp, float forceForward)
    {
        for (int i = 0; i < bodyParts.Length; i++)
        {
            Rigidbody rigidbody = bodyParts[i].GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddForce(Vector3.up * forceUp, ForceMode.Impulse);
                rigidbody.AddForce(Vector3.forward * forceForward, ForceMode.Impulse);
            }
        }
    }

    public void TeleportPlayer()
    {
        wasGrabbed = false;

        teleportCoroutine = StartCoroutine(Teleport());

        canMove = true;
        balance.ShouldBalance = true;
    }
    
    private IEnumerator Teleport()
    {
        wasTeleported = false;
        transform.GetChild(0).gameObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        bodyParts[0].transform.position = teleportPoint.position;

        yield return new WaitForSeconds(0.1f);

        transform.GetChild(0).gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        wasTeleported = true;
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 boxCenter = rb.transform.position + rb.transform.TransformDirection(boxOffset);
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, rb.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
