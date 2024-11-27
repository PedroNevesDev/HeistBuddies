using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractModule : MonoBehaviour
{
    [Header("InteractableDetection")]
    [SerializeField] Transform sphereOrigin;
    [SerializeField] float sphereRadius;
    bool onInteract;
    public void OnInteract(InputAction.CallbackContext context) => onInteract = context.performed;

    PlayerController playerController;

    bool shouldInteract;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateObjectInputText()
    {
        if(currentGrabbable==null)
        {
            verticalLayoutGroup.gameObject.SetActive(false);
            return;
        }
        for(int i = grabbableTexts.Count-1;i>=0;i-- )
        {
            Destroy(grabbableTexts[i].gameObject);
            grabbableTexts.RemoveAt(i);
        }
        verticalLayoutGroup.transform.position = currentGrabbable.transform.position + new Vector3(0,currentGrabbable.transform.localScale.y,0);
        switch(currentGrabbable.State)
        {
            case ItemState.Idle:
            verticalLayoutGroup.gameObject.SetActive(true);
                AddText("Grab");
            break;
            case ItemState.Grabbed:
            verticalLayoutGroup.gameObject.SetActive(true);
            if(currentGrabbable.Data.isThrowable)
            {
                AddText("Throw");
            }
            if(currentGrabbable.Data.isStorable)
            {
                AddText("Store");
            }

            break;
            case ItemState.Throwing:
            verticalLayoutGroup.gameObject.SetActive(false);
            break;
        }
    }
    void CheckForInteractable()
    {
        if(Physics.SphereCast(sphereOrigin.position,sphereRadius, Vector3.up,out RaycastHit hit))
        {
        }
    }
}
