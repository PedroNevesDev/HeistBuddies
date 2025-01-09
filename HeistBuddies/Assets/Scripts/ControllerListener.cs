using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
public class ControllerListener : MonoBehaviour
{
    [SerializeField] Mesh lockMesh;
    [SerializeField] Mesh pickMesh;
    public GameObject playerPrefab;

    public CinemachineTargetGroup cameraTargetGroup;

    SelectionPersistantData spd;

    public Transform mapSpawnPoint;
    
    public bool debugMode;
    PlayerInput p1;
    PlayerInput p2;

    bool multiplayer = false;
    int numberOfGamepads = 0;
    void Start()
    {
        spd = SelectionPersistantData.Instance;
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
        Mesh mesh1;
        Mesh mesh2;
        if(spd.LockModel==1)
        {
            mesh1 = lockMesh;
            mesh2 = pickMesh;
        }
        else
        {
            mesh2 = lockMesh;
            mesh1 = pickMesh;
        }

        // Instantiating player one
        p1 = PlayerInput.Instantiate(playerPrefab); 
        p1.transform.position = mapSpawnPoint.position + mapSpawnPoint.right*1;
        PlayerController pc1 = p1.GetComponent<PlayerController>();
        cameraTargetGroup.AddMember(pc1.Rb.transform,1,1);
        pc1.SetMesh(mesh1);
        

        // Instantiating player two
        p2 = PlayerInput.Instantiate(playerPrefab); 
        p2.transform.position = mapSpawnPoint.position + mapSpawnPoint.right*2;
        PlayerController pc2 = p2.GetComponent<PlayerController>();
        cameraTargetGroup.AddMember(pc2.Rb.transform,1,1);
        pc2.SetMesh(mesh2);

        SwitchControlSchemes();
    }

    void SwitchControlSchemes()
    {
    InputDevice[] p1Devices = GetConnectedDevices(0); // Devices for Player 1
    InputDevice[] p2Devices = GetConnectedDevices(1); // Devices for Player 2

    // Assign control schemes based on multiplayer status
    p1.SwitchCurrentControlScheme(multiplayer ? "Multiplayer" : "Player1", p1Devices);
    p2.SwitchCurrentControlScheme(multiplayer ? "Multiplayer" : "Player2", p2Devices);
    }

    // Just a quick and easy way to get access to the default singleplayer devices without repiting the code
    InputDevice[] GetConnectedDevices(int playerInputIndex)
    {

        List<InputDevice> devices = new List<InputDevice>();
        numberOfGamepads = Gamepad.all.Count;

        if (multiplayer)
        {
            if (playerInputIndex < numberOfGamepads)
            {
                devices.Add(Gamepad.all[playerInputIndex]); // Assign specific gamepad
            }
        }
        else
        {
            if (Keyboard.current != null) devices.Add(Keyboard.current);
            if (numberOfGamepads == 1) devices.Add(Gamepad.all[0]);
        }

        Debugging(devices);
        return devices.ToArray();
    }



    void Debugging(List<InputDevice> devices)
    {
        if (!debugMode) return;

        Debug.Log($"Gamepad count: {numberOfGamepads}");
        Debug.Log($"Multiplayer Mode: {multiplayer}");
        foreach (var device in devices)
        {
            Debug.Log($"Assigned Device: {device.name} ({device.deviceId})");
        }
    }
    }
