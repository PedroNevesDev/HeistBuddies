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

    void Update()
    {
        // If players doesnt exist spanw them
        if(!p1 || !p2) 
            SpawnPlayers();

        // Checking for Controllers Count
        if(numberOfGamepads!=Gamepad.all.Count) 
            OnDeviceChange();
    }
    void OnDeviceChange()
    {
        // Update numberOfPads so that it doesnt call OnDeviceChange multiple times
        numberOfGamepads=Gamepad.all.Count; 

        // Assigning multiplayer bool for better readability 
        multiplayer=numberOfGamepads>=2; 

        Debugging();

        // Changing Controller Schemes depending on the boolean. true == multiplayer, false == singleplayer  
        if(multiplayer) 
        {
            p1.SwitchCurrentControlScheme("Multiplayer", new InputDevice[] {Gamepad.all[0]});
            p2.SwitchCurrentControlScheme("Multiplayer", new InputDevice[] {Gamepad.all[1]});
        }
        else        
        {
     

            p1.SwitchCurrentControlScheme("Player1", GetDefaultDevices());
            p2.SwitchCurrentControlScheme("Player2", GetDefaultDevices());
        }
    }

    // Just a quick and easy way to get access to the default singleplayer devices without repiting the code
    InputDevice[] GetDefaultDevices()
    {
            if(Gamepad.all.Count==1)
                return new InputDevice[] { Keyboard.current, Mouse.current, Gamepad.all[0]};

        return new InputDevice[] { Keyboard.current, Mouse.current};
    }
    void SpawnPlayers()
    {
        // Instantiating player one
        p1 = PlayerInput.Instantiate(playerPrefab); 

        p1.SwitchCurrentControlScheme("Player1", GetDefaultDevices());
        p1.transform.position = mapSpawnPoint.position + Vector3.left*1;
        cameraTargetGroup.AddMember(p1.GetComponent<PlayerController>().Rb.transform,1,1);
        
        // Instantiating player two
        p2 = PlayerInput.Instantiate(playerPrefab); 

        p2.SwitchCurrentControlScheme("Player2", GetDefaultDevices());
        p2.transform.position = mapSpawnPoint.position + Vector3.left*2;
        cameraTargetGroup.AddMember(p2.GetComponent<PlayerController>().Rb.transform,1,1);
    }

    void Debugging()
    {
        print("Gamepad count: " + numberOfGamepads);
        print("Multiplayer Mode: " + multiplayer);
    }
}
