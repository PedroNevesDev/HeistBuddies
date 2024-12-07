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
    [SerializeField] private LayerMask grababbleDetectionLayer;
    [SerializeField] private LayerMask interactableDetectionLayer;
    [SerializeField] private Item currentGrabbable = null;
    [SerializeField] private IInteractable currentInteractable = null;
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
        CheckForGrabbable();
        UpdateObjectInputText();
    }

    void UpdateObjectInputText()
    {
        if(currentGrabbable!=null)
        {
            SetupTextForGrabbables();

        }
        else if(currentInteractable!=null)
        {

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

    public void CheckForGrabbable()
    {
        Vector3 boxCenter = rbBoxOrigin.position + rbBoxOrigin.transform.TransformDirection(boxOffset);
        List<Collider> hits = new List<Collider>(Physics.OverlapBox(boxCenter, boxSize / 2, rbBoxOrigin.transform.rotation, grababbleDetectionLayer));

        if (hits.Count > 0)
        {
            Item grabbable = hits[0].GetComponent<Item>();
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
