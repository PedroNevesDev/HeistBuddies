using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractModule : MonoBehaviour
{
    EnvironmentDetectionModule environmentDetectionModule;

    PlayerController playerController;
    bool shouldInteract;
    public void OnInteract(InputAction.CallbackContext context) => shouldInteract = context.performed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        environmentDetectionModule = GetComponent<EnvironmentDetectionModule>();

    }


    void Update()
    {
        Interact();
    }
    public void Interact() 
    {
        if(!shouldInteract)return;
        shouldInteract=false;
        if(environmentDetectionModule.CurrentInteractable==null)return;
        
        environmentDetectionModule.CurrentInteractable.Interact(playerController);
    }
}
