using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;


public class ControllerListener : MonoBehaviour
{
    public PlayerInputs[] playerInputs;
    public PlayerController playerPrefab;

    public Transform mapSpawnPoint;
    PlayerInputs[] inputs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void Start()
    {
        Instantiate(playerPrefab,mapSpawnPoint.position,mapSpawnPoint.rotation).EnableInput(playerInputs[0]);
        Instantiate(playerPrefab,mapSpawnPoint.position,mapSpawnPoint.rotation).EnableInput(playerInputs[1]);

    }
    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        Debug.Log("Changed");
        if (change == InputDeviceChange.Added)
        {
            if (Gamepad.all.Count()==2)  // Check if the added device is a Gamepad
            {
                    inputs[0].ChangeType("SinglePlayer1Map");
                    inputs[1].ChangeType("SinglePlayer2Map");
            }
        }
        else if (change == InputDeviceChange.Removed)
        {
            if (Gamepad.all.Count()<2)  // Check if the added device is a Gamepad
            {
                    inputs[0].ChangeType("Player1Map");
                    inputs[1].ChangeType("Player2Map");
            }
        }
    }
}
