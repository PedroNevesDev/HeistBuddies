using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController)),RequireComponent(typeof(GroundDetection))]
public class PlayerJumpModule : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float jumpForce;

    [SerializeField] Balance balanceForDeactivation;
    GroundDetection myGroundDetection;

    bool jump;
    public void OnJump(InputAction.CallbackContext context) => jump = context.performed;

    void Start()
    {
        myGroundDetection = GetComponent<GroundDetection>();
    }
    void Update()
    {
        Jump();        
    }

    void Jump()
    {
        if(!rb||!myGroundDetection.CheckForGround()) return;
        if(jump==false)return;
        balanceForDeactivation.gameObject.SetActive(false);
        rb.AddForce(jumpForce*100*Vector3.up,ForceMode.Impulse);
        jump=false;
    }
}
