using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerBackpackModule)),RequireComponent(typeof(PlayerController))]
public class PlayerGrabbingModule : MonoBehaviour
{
    [SerializeField] Rigidbody[] armsRigidbodies;
    bool isGrabbing = false;
    private PlayerBackpackModule backpack = null;
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

    EnvironmentDetectionModule environmentDetectionModule;

    [Header("Arm Settings")]
    
    public Transform target; // The target to aim at
    public ConfigurableJoint rightArmJoint; // The joint controlling the arm
    public ConfigurableJoint leftArmJoint; // The joint controlling the arm

    public float positionSpring = 100f; // Spring strength for rotation
    public float positionDamper = 10f; // Damping for smooth rotation

    PlayerController playerController;

    WeightManager weightManager;

    JointDrive jd1;
    JointDrive jd2;
    public void OnGrab(InputAction.CallbackContext context) => isGrabbing = context.performed;
    public void OnThrow(InputAction.CallbackContext context) => throwingCtx = context;
    
    public Item GetGrabbable()=>environmentDetectionModule.CurrentGrabbable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backpack = GetComponent<PlayerBackpackModule>();
        environmentDetectionModule = GetComponent<EnvironmentDetectionModule>();
        playerController = GetComponent<PlayerController>();
        weightManager = WeightManager.Instance;
    }

    private void Update()
    {
        if(GetGrabbable()&&GetGrabbable().State==ItemState.Grabbed&&isGrabbing)
            Store();
        PointArms();
        Grab();
        CheckThrowingState();
    }
    void AddText(string text)
    {
        grabbableTexts.Add(Instantiate(grabableObjTextPrefab,verticalLayoutGroup.transform));
        grabbableTexts[grabbableTexts.Count-1].text = text;
    }    

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
        if(isGrabbing==true||(GetGrabbable()&&GetGrabbable().State==ItemState.Grabbed)) 
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
        if (GetGrabbable() == null||GetGrabbable().State==ItemState.Idle) return;
        if (throwingCtx.performed)
        {
            GetGrabbable().State = ItemState.Throwing;
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
            GetGrabbable().Throw(dir);
            GetGrabbable().State = ItemState.Idle;
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


    public void Grab()
    {
        if (GetGrabbable() == null) return;
        if (isGrabbing == false)  return;

        GetGrabbable().Grab(pointTarget);
        isGrabbing = false;
    }

    public void Store()
    {
        if(!GetGrabbable().Data.isStorable)return;
        Item item = GetGrabbable() as Item;
        string playerName = playerController.SkinnedMeshRenderer.sharedMesh.name;
        if(!weightManager.CheckIfItemCanBeAdded(item,playerName))return;

        weightManager.AddItemWeight(item,playerName);
        backpack.AddItemToBackPack(item);
    }
}
