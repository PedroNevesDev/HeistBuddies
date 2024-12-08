using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractModule : MonoBehaviour
{
    EnvironmentDetectionModule environmentDetectionModule;
    bool onInteract;
    public void OnInteract(InputAction.CallbackContext context) => onInteract = context.performed;

    PlayerController playerController;

    bool shouldInteract;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        environmentDetectionModule = GetComponent<EnvironmentDetectionModule>();

    }

    // Update is called once per frame
    void Update()
    {
        Interact();
    }

    void Interact()
    {
        if(!onInteract)return;
        if(environmentDetectionModule.CurrentInteractable==null)return;
        environmentDetectionModule.CurrentInteractable.Interact();
    }
}
