using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractModule : MonoBehaviour
{
    [Header("InteractableDetection")]
    [SerializeField] Transform sphereOrigin;
    [SerializeField] float sphereRadius;
    bool onInteract;
    public void OnInteract(InputAction.CallbackContext context) => onInteract = context.performed;

    PlayerController playerController;

    bool shouldInteract;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void CheckForInteractable()
    {
        if(Physics.SphereCast(sphereOrigin.position,sphereRadius, Vector3.up,out RaycastHit hit))
        {
        }
    }
}
