using System.Collections.Generic;
using Mono.Cecil.Cil;
using NUnit.Framework.Constraints;
using Unity.Burst.Intrinsics;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;




public class ControllerListener : MonoBehaviour
{
    [SerializeField] List<Mesh> randomPlayerMeshes = new List<Mesh>();
    public GameObject playerPrefab;

    public CinemachineTargetGroup cameraTargetGroup;

    public Transform mapSpawnPoint;
    
    public bool debugMode;
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
        PlayerController pc1 = p1.GetComponent<PlayerController>();
        cameraTargetGroup.AddMember(pc1.Rb.transform,1,1);
        pc1.SetMesh(GetRandomMesh());
        

        // Instantiating player two
        p2 = PlayerInput.Instantiate(playerPrefab); 
        p2.transform.position = mapSpawnPoint.position + Vector3.left*2;
        PlayerController pc2 = p2.GetComponent<PlayerController>();
        cameraTargetGroup.AddMember(pc2.Rb.transform,1,1);
        pc2.SetMesh(GetRandomMesh());

        SwitchControlSchemes();
    }

    public Mesh GetRandomMesh()
    {
        Mesh mesh = randomPlayerMeshes[Random.Range(0,randomPlayerMeshes.Count)];
        randomPlayerMeshes.Remove(mesh);
        return mesh;
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
        List<InputDevice> devices = new List<InputDevice>();
        // Update numberOfPads so that it doesnt call OnDeviceChange multiple times
        numberOfGamepads=Gamepad.all.Count; 
        // Assigning multiplayer bool for better readability 
        multiplayer=numberOfGamepads>=2; 

        if(multiplayer) //Checks multiplayer has been triggered (condition : having more than one gamepad connected)
        {
            devices.Add(Gamepad.all[playerInputIndex]);
        }
        else 
        {
            if(Keyboard.current!=null) devices.Add(Keyboard.current);
            if(numberOfGamepads==1) devices.Add(Gamepad.all[0]);            
        }
        
        Debugging();
        return devices.ToArray();
    }



    void Debugging()
    {
        if(!debugMode) return;
        print("Gamepad count: " + numberOfGamepads);
        print("Multiplayer Mode: " + multiplayer);
    }
}
