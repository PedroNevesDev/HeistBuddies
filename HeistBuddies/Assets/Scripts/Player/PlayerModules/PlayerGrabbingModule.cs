using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Threading;

[RequireComponent(typeof(PlayerBackpackModule)),RequireComponent(typeof(PlayerController))]
public class PlayerGrabbingModule : MonoBehaviour
{
    [Header("Grab Detection Box Settings")]
    [SerializeField] private Vector3 boxSize = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 boxOffset = new Vector3(0, 0, 1);
    [SerializeField] private LayerMask detectionLayer;

    [SerializeField] Rigidbody[] armsRigidbodies;
    [SerializeField] private Item currentGrabbable = null;
    bool isGrabbing = false;
    private PlayerBackpackModule backpack = null;

    [SerializeField] Rigidbody rbBoxOrigin;

    [SerializeField] Transform pointTarget;

    [SerializeField] private float throwForce;

    [SerializeField] private float maxHoldingDuration = 1;

    float currentHoldingDuration;
    [SerializeField] Transform orientation;
    InputAction.CallbackContext throwingCtx;
    
    [SerializeField] Image fillThrowUi;
    [SerializeField] GameObject throwCanvas;
    [SerializeField] AnimationCurve exponentialCurve;

    [SerializeField] VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] TextMeshProUGUI grabableObjTextPrefab;
    [SerializeField] List<TextMeshProUGUI> grabbableTexts = new List<TextMeshProUGUI>();
    public void OnGrab(InputAction.CallbackContext context) => isGrabbing = context.performed;
    public void OnThrow(InputAction.CallbackContext ctx) => throwingCtx = ctx;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backpack = GetComponent<PlayerBackpackModule>();
    }

    private void Update()
    {
        if(currentGrabbable!=null&&currentGrabbable.State==ItemState.Grabbed&&isGrabbing)
            Store();
        CheckForGrabbable();
        Grab();
        PointHands();
        CheckThrowingState();
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
        verticalLayoutGroup.transform.position = currentGrabbable.transform.position + new Vector3(0,currentGrabbable.transform.lossyScale.y,0);
        switch(currentGrabbable.State)
        {
            case ItemState.Idle:
            verticalLayoutGroup.gameObject.SetActive(true);
            grabbableTexts.Add(Instantiate(grabableObjTextPrefab,verticalLayoutGroup.transform));
            grabbableTexts[grabbableTexts.Count-1].text = "Grab";
            break;
            case ItemState.Grabbed:
            verticalLayoutGroup.gameObject.SetActive(true);
            if(currentGrabbable.Data.isThrowable)
            {
                grabbableTexts.Add(Instantiate(grabableObjTextPrefab,verticalLayoutGroup.transform));
                grabbableTexts[grabbableTexts.Count-1].text = "Throw";
            }
            if(currentGrabbable.Data.isStorable)
            {
                grabbableTexts.Add(Instantiate(grabableObjTextPrefab,verticalLayoutGroup.transform));
                grabbableTexts[grabbableTexts.Count-1].text = "Store";
            }

            break;
            case ItemState.Throwing:
            verticalLayoutGroup.gameObject.SetActive(false);
            break;
        }
    }
    private void PointHands()
    {
        if(isGrabbing == false) return;
        foreach(Rigidbody arm in armsRigidbodies)
        {

        }
    }
    public void CheckThrowingState()
    {
        if (currentGrabbable == null||currentGrabbable.State==ItemState.Idle) return;
        if (throwingCtx.performed)
        {
            currentGrabbable.State = ItemState.Throwing;
            throwCanvas.SetActive(true);
            currentHoldingDuration += Time.deltaTime;
            currentHoldingDuration = Math.Clamp(currentHoldingDuration,0,maxHoldingDuration);
        }
        if (!throwingCtx.performed&&currentHoldingDuration!=0)
        {

            Vector3 dir = orientation.forward * currentHoldingDuration * throwForce+ orientation.up/2 * currentHoldingDuration * throwForce;
            currentGrabbable.Throw(dir);
            currentGrabbable.State = ItemState.Idle;
            currentHoldingDuration = 0;
            throwCanvas.SetActive(false);
        }
        fillThrowUi.fillAmount = exponentialCurve.Evaluate(currentHoldingDuration/maxHoldingDuration);
    }

    public void CheckForGrabbable()
    {
        Vector3 boxCenter = rbBoxOrigin.position + rbBoxOrigin.transform.TransformDirection(boxOffset);
        List<Collider> hits = new List<Collider>(Physics.OverlapBox(boxCenter, boxSize / 2, rbBoxOrigin.transform.rotation, detectionLayer));

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

    public void Grab()
    {
        if (currentGrabbable == null) return;
        if (isGrabbing == false)  return;

        currentGrabbable.Grab(pointTarget);
        isGrabbing = false;
    }

    public void Store()
    {
        if(!currentGrabbable.Data.isStorable)return;
        
        Item item = currentGrabbable as Item;
        backpack.AddItemToBackPack(item);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 boxCenter = rbBoxOrigin.transform.position + rbBoxOrigin.transform.TransformDirection(boxOffset);
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, rbBoxOrigin.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
