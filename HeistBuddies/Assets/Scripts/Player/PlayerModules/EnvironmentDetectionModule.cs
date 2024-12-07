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
    [SerializeField] private Item currentGrabbable = null;
    [SerializeField] Rigidbody rbBoxOrigin;

    [SerializeField] VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] TextMeshProUGUI grabableObjTextPrefab;
    [SerializeField] List<TextMeshProUGUI> grabbableTexts = new List<TextMeshProUGUI>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateObjectInputText();
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
        grabbableTexts.Add(Instantiate(grabableObjTextPrefab,verticalLayoutGroup.transform));
        grabbableTexts[grabbableTexts.Count-1].text = text;
    }    
}
