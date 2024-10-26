using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController; // Using CharacterController for 3D movement

    [SerializeField] Rigidbody mainBody;
    public float moveSpeed = 5f; // Movement speed
    private Vector3 velocity; // To track the player's velocity

    [SerializeField] private Transform mesh;
    

    [Header("GroundCheck")]
    bool isGrounded;
    public PlayerInputs myInputs;

    private Camera myCam;

    [SerializeField] Animator animator;

    private void Start()
    {
        myCam = Camera.main;
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

        Move(); // Call Move method every frame
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
}
