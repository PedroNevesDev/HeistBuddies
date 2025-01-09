using UnityEngine;

public class Safe : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject door;
    private bool isOpen = false;

    [SerializeField] private LockPicking lockPicking;
    public string InteractionName { get => "Open Safe"; }

    public GameObject GetGameObject { get => gameObject; }

    public string WhoIsInteracting => null;

    public bool CanInteract(PlayerController player)
    {

        return !isOpen;
    }
    public void Interact(PlayerController player)
    {
        if(lockPicking.gameObject.activeSelf==true)
        {
            lockPicking.CheckForOverlap();
        }
        else
        {
            lockPicking.OnSuccess += OpenSafe;
            lockPicking.gameObject.SetActive(true);
        }
    }
    void OpenSafe()
    {
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
