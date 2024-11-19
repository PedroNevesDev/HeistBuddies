using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerBackpackModule)),RequireComponent(typeof(PlayerController))]
public class PlayerGrabbingModule : MonoBehaviour
{
    [Header("Grab Detection Box Settings")]
    [SerializeField] private Vector3 boxSize = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 boxOffset = new Vector3(0, 0, 1);
    [SerializeField] private LayerMask detectionLayer;

    private IGrabbable currentGrabbable = null;
    bool isGrabbing = false;
    private PlayerBackpackModule backpack = null;

    [SerializeField] Rigidbody rbBoxOrigin;
    
    public void OnGrab(InputAction.CallbackContext context) => isGrabbing = context.ReadValue<float>()>0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backpack = GetComponent<PlayerBackpackModule>();
    }

    private void Update()
    {
        CheckForGrabbable();
        Grab();
    }
    public void CheckForGrabbable()
    {
        Vector3 boxCenter = rbBoxOrigin.position + rbBoxOrigin.transform.TransformDirection(boxOffset);
        Collider[] hits = Physics.OverlapBox(boxCenter, boxSize / 2, rbBoxOrigin.transform.rotation, detectionLayer);

        if (hits.Length > 0)
        {
            IGrabbable grabbable = hits[0].GetComponent<IGrabbable>();
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
        if(currentGrabbable == null)
            return;
        if(isGrabbing == false)
            return;
    
        Item item = currentGrabbable as Item;
        backpack.AddItemToBackPack(item);

        currentGrabbable.Grab();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 boxCenter = rbBoxOrigin.transform.position + rbBoxOrigin.transform.TransformDirection(boxOffset);
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, rbBoxOrigin.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
