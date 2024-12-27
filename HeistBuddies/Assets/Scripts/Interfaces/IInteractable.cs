using System;
using UnityEngine;

public interface IInteractable
{
    public bool CanInteract(PlayerController player);
    public string InteractionName{ get;}
    public GameObject GetGameObject{ get;}
    public void Interact(PlayerController player);

    public String WhoIsInteracting { get ;}
    public void StopInteraction();
}
