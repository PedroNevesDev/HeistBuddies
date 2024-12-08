using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerBackpackModule)),RequireComponent(typeof(PlayerController)),RequireComponent(typeof(GroundDetection))]
public class PlayerMovementModule : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]  float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 1.5f;
    [SerializeField] private float maxSpeedMagnitude;
    [SerializeField] Rigidbody rb;
    
    // Specific transform that will be used to follow the movement direction ex:mesh. Could be anything
    [SerializeField] Transform orientation;

    // Animator of the animated rig. Lets you change the animation 
    [SerializeField] Animator animator; 

    Vector2 moveInput = Vector2.zero;

    float overcumberedPercentage = 0f;
    [SerializeField,Range(0,100)] float maxOvercumberedPercentage = 80;
    public Vector2 MoveInput { get => moveInput; set => moveInput = value; }

    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();

    GroundDetection myGroundDetection;

    PlayerController playerController;

    PlayerBackpackModule backpack;
    void Start()
    {
        myGroundDetection = GetComponent<GroundDetection>();
        playerController = GetComponent<PlayerController>();
        backpack= GetComponent<PlayerBackpackModule>();
    }
    private void FixedUpdate()
    {
        CheckOvercumberValue();
        Move();
        SpeedControl();
    }

    void CheckOvercumberValue()
    {
        string playerName = playerController.SkinnedMeshRenderer.sharedMesh.name;
        overcumberedPercentage = backpack.GetWeightPercentage();
    }
    public void Move()
    {
        float speedPlusOvercumbered = moveSpeed - (overcumberedPercentage*maxOvercumberedPercentage/100*moveSpeed);
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

            if(myGroundDetection.CheckForGround())
            {
                animator.SetBool("Moving",true);
            }
            else
            {
                animator.SetBool("Moving",false);
            }
        }
        else
        {
            // Makes the character go into Idle animation
            animator.SetBool("Moving",false); 
        }
        
        // Applying the move direction to the rigidbody
        rb.AddForce(move * speedPlusOvercumbered,ForceMode.VelocityChange); 
    }
    public void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float overcumberedAffectedMagnitude = maxSpeedMagnitude-(overcumberedPercentage*maxOvercumberedPercentage/100*maxSpeedMagnitude);

        if (flatVel.magnitude > maxSpeedMagnitude)
        {
            Vector3 limitVel = flatVel.normalized * maxSpeedMagnitude;
            rb.linearVelocity = new Vector3(limitVel.x, rb.linearVelocity.y, limitVel.z);
        }
    }
}
