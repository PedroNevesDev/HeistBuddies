using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class OnCancelEvent : MonoBehaviour
{
    private InputSystemUIInputModule inputModule;
    [SerializeField]UnityEvent onCancel = new UnityEvent();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        var eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        inputModule = eventSystem?.GetComponent<InputSystemUIInputModule>();
    }

    private void OnDisable() 
    {
        inputModule.cancel.action.performed -= DeactivatePannel;
    }

    private void OnEnable() 
    {
        inputModule.cancel.action.performed += DeactivatePannel;     
    }
    void DeactivatePannel(InputAction.CallbackContext context)
    {
        onCancel.Invoke();
    }
}
