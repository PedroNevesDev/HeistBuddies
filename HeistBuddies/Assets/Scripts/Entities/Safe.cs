using UnityEngine;

public class Safe : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject door;
    private bool isOpen = false;

    public string InteractionName { get => "Open Safe"; }

    public GameObject GetGameObject { get => gameObject; }

    public string WhoIsInteracting => null;

    public bool CanInteract(PlayerController player)
    {
        if (isOpen) return false;
        else return true;
    }

    public void Interact(PlayerController player)
    {
        if (!CanInteract(player)) return;

        if (!isOpen)
        {
            isOpen = true;
            door.transform.rotation = new Quaternion(0f, 90f, 0f, 0f);
        }
    }

    public void StopInteraction()
    {
    
    }
}
