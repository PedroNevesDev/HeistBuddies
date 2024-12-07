using UnityEngine;

public interface IInteractable
{
    public bool IsBeingInteractedWith{ get;}
    public bool ShouldBeInteractedWith{ get;}
    public string InteractionName{ get;}
    public GameObject GetGameObject{ get;}
    public void Interact(PlayerController playerController);

    public void StopInteraction();
}
