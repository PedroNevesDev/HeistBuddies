using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    public void OnGrab(InputAction.CallbackContext context) => isGrabbing = context.performed;
    public void OnThrow(InputAction.CallbackContext ctx) => throwingCtx = ctx;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backpack = GetComponent<PlayerBackpackModule>();
    }

    private void Update()
    {
        CheckForGrabbable();
        Grab();
        PointHands();
        CheckThrowingState();
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
        if (currentGrabbable == null) return;
        if (throwingCtx.performed)
        {
            throwCanvas.SetActive(true);
            print(throwingCtx);
            currentHoldingDuration += Time.deltaTime;
            currentHoldingDuration = Math.Clamp(currentHoldingDuration,0,maxHoldingDuration);
        }
        if (!throwingCtx.performed&&currentHoldingDuration!=0)
        {

            Vector3 dir = orientation.forward * currentHoldingDuration * throwForce+ orientation.up/3 * currentHoldingDuration * throwForce;
            currentGrabbable.Throw(dir);
            currentHoldingDuration =0;
            throwCanvas.SetActive(false);
            print("Throw");
        }
        fillThrowUi.fillAmount = currentHoldingDuration/maxHoldingDuration;
    }

    public void CheckForGrabbable()
    {
        Vector3 boxCenter = rbBoxOrigin.position + rbBoxOrigin.transform.TransformDirection(boxOffset);
        Collider[] hits = Physics.OverlapBox(boxCenter, boxSize / 2, rbBoxOrigin.transform.rotation, detectionLayer);

        if (hits.Length > 0)
        {
            Item grabbable = hits[0].GetComponent<Item>();
            if (grabbable != null)
            {
                if (currentGrabbable != grabbable)
                {
                    if (currentGrabbable != null)
                    {
                        currentGrabbable.DisableUI();
                    }

                    currentGrabbable = grabbable;

                    currentGrabbable.EnableUI();
                }
                return;
            }
        }

        if (currentGrabbable != null)
        {
            currentGrabbable.DisableUI();
            currentGrabbable = null;
        }
    }

    public void Grab()
    {
        if (currentGrabbable == null) return;
        if (isGrabbing == false)  return;
        
        //Item item = currentGrabbable as Item;
        //backpack.AddItemToBackPack(item);

        currentGrabbable.Grab(pointTarget);
    }

    public void Store()
    {
        //Item item = currentGrabbable as Item;
        //backpack.AddItemToBackPack(item);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 boxCenter = rbBoxOrigin.transform.position + rbBoxOrigin.transform.TransformDirection(boxOffset);
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, rbBoxOrigin.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
