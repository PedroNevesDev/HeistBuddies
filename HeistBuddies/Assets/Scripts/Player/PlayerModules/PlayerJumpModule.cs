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
    bool isJumping = false;
    public void OnJump(InputAction.CallbackContext context) => Jump();

    void Start()
    {
        myGroundDetection = GetComponent<GroundDetection>();
    }
    void Jump()
    {
        if(!rb||!myGroundDetection.CheckForGround()) return;
        print("Jumped");
        balanceForDeactivation.gameObject.SetActive(false);
        rb.AddForce(jumpForce*Vector3.up,ForceMode.Impulse);
    }
}
