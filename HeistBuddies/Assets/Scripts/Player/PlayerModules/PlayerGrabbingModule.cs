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
    bool shouldDecrease = false;
    public void OnGrab(InputAction.CallbackContext context) => isGrabbing = context.performed;
    public void OnThrow(InputAction.CallbackContext context) => throwingCtx = context;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backpack = GetComponent<PlayerBackpackModule>();
    }

    private void Update()
    {
        if(currentGrabbable!=null&&currentGrabbable.State==ItemState.Grabbed&&isGrabbing)
            Store();
        PointArms();
        CheckForGrabbable();
        Grab();
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
    void AddText(string text)
    {
        grabbableTexts.Add(Instantiate(grabableObjTextPrefab,verticalLayoutGroup.transform));
        grabbableTexts[grabbableTexts.Count-1].text = text;
    }    
    public Transform target; // The target to aim at
    public ConfigurableJoint rightArmJoint; // The joint controlling the arm
    public ConfigurableJoint leftArmJoint; // The joint controlling the arm

    public float positionSpring = 100f; // Spring strength for rotation
    public float positionDamper = 10f; // Damping for smooth rotation

    JointDrive jd1;
    JointDrive jd2;
    void Awake()
    {
        // Set up joint drive for smooth motion
        jd1 = new JointDrive
        {
            positionSpring = positionSpring,
            positionDamper = positionDamper,
            maximumForce = Mathf.Infinity
        };
        jd2 = new JointDrive
        {
            positionSpring = 0,
            positionDamper = 0,
            maximumForce = Mathf.Infinity
        };


    }


    void PointArms()
    {
        if (!target)return;
        print(isGrabbing);
        if(isGrabbing==true||(currentGrabbable&&currentGrabbable.State==ItemState.Grabbed)) 
        {
            leftArmJoint.slerpDrive = jd1;
            rightArmJoint.slerpDrive = jd1;
        }
        else
        {
            leftArmJoint.slerpDrive = jd2;
            rightArmJoint.slerpDrive = jd2;
            return;            
        }


        // Calculate direction to target
        Vector3 rightDirectionToTarget = target.localPosition - rightArmJoint.transform.localPosition;
        Vector3 leftDirectionToTarget = target.localPosition - leftArmJoint.transform.localPosition;
        

        // Create a rotation to face the target
        Quaternion rightTargetRotation = Quaternion.LookRotation(rightDirectionToTarget);
        Quaternion leftTargetRotation = Quaternion.LookRotation(leftDirectionToTarget);

        // Adjust for joint's local space
        rightArmJoint.targetRotation = Quaternion.Inverse(rightArmJoint.transform.localRotation) * rightTargetRotation;
        leftArmJoint.targetRotation = Quaternion.Inverse(leftArmJoint.transform.localRotation) * leftTargetRotation;
    }
    
    public void CheckThrowingState()
    {
        if (currentGrabbable == null||currentGrabbable.State==ItemState.Idle) return;
        if (throwingCtx.performed)
        {
            currentGrabbable.State = ItemState.Throwing;
            throwCanvas.SetActive(true);
            if(shouldDecrease)
            {
                currentHoldingDuration -= Time.deltaTime;
                if(currentHoldingDuration<=0)
                    DeactivateThrowUI();
            }
            else
            {
                currentHoldingDuration += Time.deltaTime;
                if(currentHoldingDuration>=maxHoldingDuration)
                    shouldDecrease = true;
            }

            currentHoldingDuration = Math.Clamp(currentHoldingDuration,0,maxHoldingDuration);
        }
        if (!throwingCtx.performed&&currentHoldingDuration!=0)
        {

            float force = exponentialCurve.Evaluate(currentHoldingDuration/maxHoldingDuration);
            Vector3 dir = orientation.forward * force * throwForce+ orientation.up/2 * force * throwForce;
            currentGrabbable.Throw(dir);
            currentGrabbable.State = ItemState.Idle;
            DeactivateThrowUI();
        }
        fillThrowUi.fillAmount = exponentialCurve.Evaluate(currentHoldingDuration/maxHoldingDuration);
    }
    void DeactivateThrowUI()
    {
        shouldDecrease = false;
        fillThrowUi.fillAmount = 0;
        currentHoldingDuration = 0;
        throwCanvas.SetActive(false);
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
