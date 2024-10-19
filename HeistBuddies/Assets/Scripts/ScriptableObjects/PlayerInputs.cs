
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;



[CreateAssetMenu(fileName = "PlayerInputs", menuName = "InputsSO/PlayerInputs")]
public class PlayerInputs : ScriptableObject
{
    public InputSystem_Actions playerInputs;
    public InputActionMap actionMap;  // Reference the generated class
    public Vector2 moveInput;

    public void Init()
    {
        playerInputs = new InputSystem_Actions();
        if(this.name == "Player1InputSO")
            playerInputs.SinglePlayer1Map.Enable();
        else 
            playerInputs.SinglePlayer2Map.Enable();
        actionMap = GetActiveActionMap(playerInputs);
        actionMap.FindAction("Move").performed+=OnMove;
        actionMap.FindAction("Move").canceled+=OnMove;
     }

    public void Cancel()
    {
        actionMap.Disable();
        actionMap.FindAction("Move").performed-=OnMove;
        actionMap.FindAction("Move").canceled-=OnMove;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveInput = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            moveInput = Vector2.zero;
        }
    }

    InputActionMap GetActiveActionMap(InputSystem_Actions inputActions)
    {
        // Check for active action maps
        if (inputActions.SinglePlayer1Map.enabled)
        {
            return inputActions.SinglePlayer1Map;
        }
        else if (inputActions.SinglePlayer2Map.enabled)
        {
            return inputActions.SinglePlayer2Map;
        }
        else if (inputActions.Player1Map.enabled)
        {
            return inputActions.Player1Map;
        }
        else if (inputActions.Player2Map.enabled)
        {
            return inputActions.Player2Map;
        }

        // Return null if no action map is active
        return null;
    }
    public void ChangeType(string actionMapName)
    {   
        InputActionAsset asset = playerInputs.asset;
        foreach(var newActionMap in asset.actionMaps)
        {
            if(newActionMap.name == actionMap.name)
            {
                newActionMap.Disable();
            }
        }

        foreach(var newActionMap in asset.actionMaps)
        {
            if(newActionMap.name == actionMapName)
            {
                newActionMap.Enable();
            }
        }
        actionMap = GetActiveActionMap(playerInputs);
        actionMap.FindAction("Move").performed+=OnMove;
    }
}
