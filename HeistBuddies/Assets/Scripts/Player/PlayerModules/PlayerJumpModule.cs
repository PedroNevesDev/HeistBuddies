using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerJumpModule : MonoBehaviour
{
    [SerializeField] float jumpForce;

    PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }
    void Jump()
    {
        if(!playerController) return;

        playerController.Rb.AddForce(jumpForce*Vector3.up,ForceMode.Impulse);
    }
}
