using UnityEngine;

public interface IInteractable
{
    public bool IsBeingInteractedWith{ get;}
    public bool ShouldBeInteractedWith{ get;}
    public void Interact(PlayerController playerController);

    public void StopInteraction();
}
