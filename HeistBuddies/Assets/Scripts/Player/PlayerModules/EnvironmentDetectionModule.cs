using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class EnvironmentDetectionModule : MonoBehaviour
{
    [Header("Detection Box Settings")]
    [SerializeField] private Vector3 boxSize = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 boxOffset = new Vector3(0, 0, 1);
    [SerializeField] private List<LayerMask> listOfGrabbables = new List<LayerMask>();
    [SerializeField] private LayerMask interactableDetectionLayer;
    [SerializeField] private Item currentGrabbable = null;
    private IInteractable currentInteractable = null;
    [SerializeField] Rigidbody rbBoxOrigin;

    [SerializeField] VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] TextMeshProUGUI grabableObjTextPrefab;
    [SerializeField] List<TextMeshProUGUI> uiImputTexts = new List<TextMeshProUGUI>();

    public Item CurrentGrabbable { get => currentGrabbable; set => currentGrabbable = value; }
    public IInteractable CurrentInteractable { get => currentInteractable; set => currentInteractable = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CheckInFront();
        UpdateObjectInputText();
    }

    void UpdateObjectInputText()
    {
        if(currentGrabbable!=null)
        {
            SetupTextForGrabbables();

        }
        else if(currentInteractable!=null&&currentInteractable.ShouldBeInteractedWith)
        {
            SetupTextForInteractables();
        }
        else
        {
            verticalLayoutGroup.gameObject.SetActive(false);            
        }
    }
    void ResetUITexts()
    {
        for(int i = uiImputTexts.Count-1;i>=0;i-- )
        {
            Destroy(uiImputTexts[i].gameObject);
            uiImputTexts.RemoveAt(i);
        }
    }

    void SetupTextForGrabbables()
    {
        ResetUITexts();
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
    void SetupTextForInteractables()
    {
            ResetUITexts();
            verticalLayoutGroup.transform.position = currentInteractable.GetGameObject.transform.position + new Vector3(0,currentInteractable.GetGameObject.transform.localScale.y,0);
            verticalLayoutGroup.gameObject.SetActive(true);
            AddText(currentInteractable.InteractionName);
    }

    public void CheckInFront()
    {
        Vector3 boxCenter = rbBoxOrigin.position + rbBoxOrigin.transform.TransformDirection(boxOffset);

        LayerMask grababbleDetectionLayer = listOfGrabbables[0];
        if(listOfGrabbables.Count>1)
        for(int i = 1;i<listOfGrabbables.Count;i++)
        {
            grababbleDetectionLayer = grababbleDetectionLayer|listOfGrabbables[i];
        }
        List<Collider> grabbables = new List<Collider>(Physics.OverlapBox(boxCenter, boxSize / 2, rbBoxOrigin.transform.rotation, grababbleDetectionLayer));

        if (grabbables.Count > 0)
        {
            if(currentGrabbable!=null)
            {
                Collider myCol = currentGrabbable.GetComponent<Collider>();
                if(currentInteractable!=null)
                {
                    if(grabbables.Contains(myCol))
                        return;
                }
            }

            Item grabbable = grabbables[0].GetComponent<Item>();
            if (grabbable != null)
            {
                if (currentGrabbable != grabbable)
                {
                    currentGrabbable = grabbable;
                }
                return;
            }
        }

        if (currentGrabbable != null)
        {
            currentGrabbable = null;
        }
        
        List<Collider> interactables = new List<Collider>(Physics.OverlapBox(boxCenter, boxSize / 2, rbBoxOrigin.transform.rotation, interactableDetectionLayer));

        if (interactables.Count > 0)
        {
            IInteractable interactable = interactables[0].GetComponent<IInteractable>();
            if (interactable != null&&interactable.ShouldBeInteractedWith&&!interactable.IsBeingInteractedWith)
            {
                if (currentInteractable != interactable)
                {
                    currentInteractable = interactable;
                }
                return;
            }
            
        }



        if (currentInteractable != null)
        {
            currentInteractable = null;
        }
    }

    void AddText(string text)
    {
        uiImputTexts.Add(Instantiate(grabableObjTextPrefab,verticalLayoutGroup.transform));
        uiImputTexts[uiImputTexts.Count-1].text = text;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 boxCenter = rbBoxOrigin.transform.position + rbBoxOrigin.transform.TransformDirection(boxOffset);
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, rbBoxOrigin.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }    
}
