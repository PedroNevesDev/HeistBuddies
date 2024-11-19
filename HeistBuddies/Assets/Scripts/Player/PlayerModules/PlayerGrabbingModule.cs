using UnityEngine;
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
    bool isThrowing = false;
    private PlayerBackpackModule backpack = null;

    [SerializeField] Rigidbody rbBoxOrigin;

    [SerializeField] Transform pointTarget;

    [SerializeField] private float throwForce;

    [Range(0,1)] private float currentPower = 0;
    
    public void OnGrab(InputAction.CallbackContext context) => isGrabbing = context.performed;

    public void OnThrow(InputAction.CallbackContext context) => isThrowing = context.performed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backpack = GetComponent<PlayerBackpackModule>();
    }

    private void Update()
    {
        CheckForGrabbable();
        Grab();
        Throw();
        PointHands();
    }

    private void PointHands()
    {
        if(isGrabbing == false) return;
        foreach(Rigidbody arm in armsRigidbodies)
        {

        }
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

    public void Throw()
    {
        if (currentGrabbable == null) return;
        if (isThrowing == false) return;

        //Item item = currentGrabbable as Item;
        //backpack.AddItemToBackPack(item);

        if (currentGrabbable.State == ItemState.Grabbed && isThrowing)
        {
            currentGrabbable.Throw(100f, 100f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 boxCenter = rbBoxOrigin.transform.position + rbBoxOrigin.transform.TransformDirection(boxOffset);
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, rbBoxOrigin.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
