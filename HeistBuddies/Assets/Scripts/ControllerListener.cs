using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;



public class ControllerListener : MonoBehaviour
{
    public PlayerInputs[] playerInputs;
    public PlayerController playerPrefab;

    public CinemachineTargetGroup myTargetGroup;

    public Transform mapSpawnPoint;
    PlayerInputs[] inputs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void Start()
    {
        for(int playerCount = 0; playerCount<2; playerCount++)
        {
            PlayerController newCharacterController=Instantiate(playerPrefab,mapSpawnPoint.position+new Vector3(playerCount*1,0,0),mapSpawnPoint.rotation);
            myTargetGroup.AddMember(newCharacterController.transform,1,1);
            newCharacterController.EnableInput(playerInputs[playerCount]);
        }
    }
    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {

        if (change == InputDeviceChange.Added)
        {
            if (Gamepad.all.Count()==2)  // Check if the added device is a Gamepad
            {
                Debug.Log("Gamemode: Singleplayer");
                inputs[0].ChangeType("SinglePlayer1Map");
                inputs[1].ChangeType("SinglePlayer2Map");
            }
        }
        else if (change == InputDeviceChange.Removed)
        {
            if (Gamepad.all.Count()<2)  // Check if the added device is a Gamepad
            {
                Debug.Log("Gamemode: Multiplayer");
                inputs[0].ChangeType("Player1Map");
                inputs[1].ChangeType("Player2Map");
            }
        }
    }
}
