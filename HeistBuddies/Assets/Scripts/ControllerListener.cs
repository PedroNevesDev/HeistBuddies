using Mono.Cecil.Cil;
using Unity.Burst.Intrinsics;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;




public class ControllerListener : MonoBehaviour
{
    public GameObject playerPrefab;

    public CinemachineTargetGroup myTargetGroup;

    public Transform mapSpawnPoint;
    
    PlayerInput p1;
    PlayerInput p2;

    bool multiplayer = false;
    int numberOfGamepads = 0;
    string controlScheme;

    void Update()
    {
        if(!p1 || !p2)
            SpawnPlayers();

        if(numberOfGamepads!=Gamepad.all.Count)
            OnDeviceChange();
    }
    void OnDeviceChange()
    {
        numberOfGamepads=Gamepad.all.Count;

        multiplayer=!(numberOfGamepads<2);

        controlScheme = "KeyboardMouseGamepad";

        print("Gamepad count: " + numberOfGamepads);
        print("Multiplayer Mode: " + multiplayer);
        if(multiplayer)
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
        p1 = PlayerInput.Instantiate(playerPrefab);
        p1.transform.position = mapSpawnPoint.position + Vector3.right*1;
        myTargetGroup.AddMember(p1.GetComponent<PlayerController>().Rb.transform,1,1);

        p2 = PlayerInput.Instantiate(playerPrefab);
        p2.transform.position = mapSpawnPoint.position + Vector3.right*2;
        myTargetGroup.AddMember(p2.GetComponent<PlayerController>().Rb.transform,1,1);
    }
}
