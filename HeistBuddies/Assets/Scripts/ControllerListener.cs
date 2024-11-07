using Mono.Cecil.Cil;
using Unity.Burst.Intrinsics;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;




public class ControllerListener : MonoBehaviour
{
    public GameObject playerPrefab;

    public CinemachineTargetGroup cameraTargetGroup;

    public Transform mapSpawnPoint;
    
    PlayerInput p1;
    PlayerInput p2;

    bool multiplayer = false;
    int numberOfGamepads = 0;
    void Start()
    {
        SpawnPlayers();
    }
    void Update()
    {
        if(!p1 || !p2) 
            return;

        // Checking for Controllers Count
        if(numberOfGamepads!=Gamepad.all.Count) 
            SwitchControlSchemes();
    }
    void SpawnPlayers()
    {
        // Instantiating player one
        p1 = PlayerInput.Instantiate(playerPrefab); 
        p1.transform.position = mapSpawnPoint.position + Vector3.left*1;
        cameraTargetGroup.AddMember(p1.GetComponent<PlayerController>().Rb.transform,1,1);

        // Instantiating player two
        p2 = PlayerInput.Instantiate(playerPrefab); 
        p2.transform.position = mapSpawnPoint.position + Vector3.left*2;
        cameraTargetGroup.AddMember(p2.GetComponent<PlayerController>().Rb.transform,1,1);

        SwitchControlSchemes();
    }
    void SwitchControlSchemes()
    {
        // Quick summary here: the controlSchemes "Player1" or "Player2" are the control shemes for the singleplayer
        // as for the multiplayer they can have the control scheme "Multiplayer" since they are connected to different devices(gamepads)

        // Changing Controller Schemes depending on the boolean. true == multiplayer, false == singleplayer  
        p1.SwitchCurrentControlScheme(multiplayer?"Multiplayer":"Player"+(p1.playerIndex+1), GetConnectedDevices(p1.playerIndex));
        p2.SwitchCurrentControlScheme(multiplayer?"Multiplayer":"Player"+(p2.playerIndex+1), GetConnectedDevices(p1.playerIndex));
    }

    // Just a quick and easy way to get access to the default singleplayer devices without repiting the code
    InputDevice[] GetConnectedDevices(int playerInputIndex)
    {
        // Update numberOfPads so that it doesnt call OnDeviceChange multiple times
        numberOfGamepads=Gamepad.all.Count; 
        // Assigning multiplayer bool for better readability 
        multiplayer=numberOfGamepads>=2; 

        if(multiplayer) //Checks multiplayer has been triggered (condition : having more than one gamepad connected)
        {
            return new InputDevice[] {Gamepad.all[playerInputIndex]};
        }
        else if(numberOfGamepads==1) //Checks if it should add the gamepad
        {
            return new InputDevice[] { Keyboard.current, Gamepad.all[0]};                
        }
        else if(Keyboard.current!=null) //Checks if there is a keyboard connected
        {
            return new InputDevice[] { Keyboard.current};
        }
        Debugging();
        return null;
    }



    void Debugging()
    {
        print("Gamepad count: " + numberOfGamepads);
        print("Multiplayer Mode: " + multiplayer);
    }
}
