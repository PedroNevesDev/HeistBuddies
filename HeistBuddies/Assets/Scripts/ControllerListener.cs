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
        if(!p1 || !p2) // If players doesnt exist spanw them
            SpawnPlayers();

        if(numberOfGamepads!=Gamepad.all.Count) // Checking for Controllers Count
            OnDeviceChange();
    }
    void OnDeviceChange()
    {
        numberOfGamepads=Gamepad.all.Count; // Update numberOfPads so that it doesnt call OnDeviceChange multiple times

        multiplayer=numberOfGamepads>=2; // Assigning multiplayer bool for better readability 


        //Debugging
        print("Gamepad count: " + numberOfGamepads);
        print("Multiplayer Mode: " + multiplayer);

        if(multiplayer) // Changing Controller Schemes depending on the boolean. true == multiplayer, false == singleplayer  
        {
            p1.SwitchCurrentControlScheme("Multiplayer", new InputDevice[] {Gamepad.all[0]});
            p2.SwitchCurrentControlScheme("Multiplayer", new InputDevice[] {Gamepad.all[1]});
        }
        else        
        {
            p1.SwitchCurrentControlScheme("Player1", new InputDevice[] { Keyboard.current, Mouse.current, Gamepad.all[0]});
            p2.SwitchCurrentControlScheme("Player2", new InputDevice[] { Keyboard.current, Mouse.current, Gamepad.all[0]});
        }

    
    }
    void SpawnPlayers()
    {
        p1 = PlayerInput.Instantiate(playerPrefab); // Instantiating player one

        p1.SwitchCurrentControlScheme("Player1", new InputDevice[] { Keyboard.current, Mouse.current, Gamepad.all[0]});
        p1.transform.position = mapSpawnPoint.position + Vector3.left*1;
        cameraTargetGroup.AddMember(p1.GetComponent<PlayerController>().Rb.transform,1,1);

        p2 = PlayerInput.Instantiate(playerPrefab); // Instantiating player two

        p2.SwitchCurrentControlScheme("Player2", new InputDevice[] { Keyboard.current, Mouse.current, Gamepad.all[0]});
        p2.transform.position = mapSpawnPoint.position + Vector3.left*2;
        cameraTargetGroup.AddMember(p2.GetComponent<PlayerController>().Rb.transform,1,1);
    }
}
