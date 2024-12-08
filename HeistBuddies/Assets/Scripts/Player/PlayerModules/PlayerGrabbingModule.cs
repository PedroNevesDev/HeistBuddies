using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.VisualScripting;

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

    [Header("Audio")]
    [SerializeField] AudioClip storingItemsSound;

    AudioManager audioManager;

    [Header("Arm Settings")]
    
    public Transform target; // The target to aim at
    public ConfigurableJoint rightArmJoint; // The joint controlling the arm
    public ConfigurableJoint leftArmJoint; // The joint controlling the arm

    public ConfigurableJoint rightForearmJoint; // The joint controlling the arm
    public ConfigurableJoint leftForearmJoint; // The joint controlling the arm
    public float positionSpring = 100f; // Spring strength for rotation
    public float positionDamper = 10f; // Damping for smooth rotation

    PlayerController playerController;

    WeightManager weightManager;
    

    JointDrive jd1;
    JointDrive jd2;


    public string GetPlayerName()=>playerController.SkinnedMeshRenderer.sharedMesh.name;
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
        audioManager = AudioManager.Instance;
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
    if (!target) return;

    // Check if grabbing or holding a grabbed item
    if (isGrabbing || (GetGrabbable() && GetGrabbable().State == ItemState.Grabbed))
    {
        leftArmJoint.slerpDrive = jd1;
        rightArmJoint.slerpDrive = jd1;
        leftForearmJoint.slerpDrive = jd1;
        rightForearmJoint.slerpDrive = jd1;
        print("Pointing");
    }
    else
    {
        leftArmJoint.slerpDrive = jd2;
        rightArmJoint.slerpDrive = jd2;
        leftForearmJoint.slerpDrive = jd2;
        rightForearmJoint.slerpDrive = jd2;
        return;
    }

    // Calculate direction to target for upper arms
    Vector3 rightDirectionToTarget = target.localPosition - rightArmJoint.transform.localPosition;
    Vector3 leftDirectionToTarget = target.localPosition - leftArmJoint.transform.localPosition;

    // Create a rotation for upper arms
    Quaternion rightUpperArmTargetRotation = Quaternion.LookRotation(rightDirectionToTarget);
    Quaternion leftUpperArmTargetRotation = Quaternion.LookRotation(leftDirectionToTarget);

    // Adjust for upper arm joints' local space
    rightArmJoint.targetRotation = Quaternion.Inverse(rightArmJoint.transform.localRotation) * rightUpperArmTargetRotation;
    leftArmJoint.targetRotation = Quaternion.Inverse(leftArmJoint.transform.localRotation) * leftUpperArmTargetRotation;

    // If forearms exist, calculate direction and rotation for them too
    if (rightForearmJoint != null && leftForearmJoint != null)
    {
        Vector3 rightForearmDirectionToTarget = target.localPosition - rightForearmJoint.transform.localPosition;
        Vector3 leftForearmDirectionToTarget = target.localPosition - leftForearmJoint.transform.localPosition;

        Quaternion rightForearmTargetRotation = Quaternion.LookRotation(rightForearmDirectionToTarget);
        Quaternion leftForearmTargetRotation = Quaternion.LookRotation(leftForearmDirectionToTarget);

        rightForearmJoint.targetRotation = Quaternion.Inverse(rightForearmJoint.transform.localRotation) * rightForearmTargetRotation;
        leftForearmJoint.targetRotation = Quaternion.Inverse(leftForearmJoint.transform.localRotation) * leftForearmTargetRotation;
    }

    print("Arms pointing to target");
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
            throwingCtx = new InputAction.CallbackContext();
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

    public void ReleaseUncumberingFromGrabbable()
    {
        weightManager.RemoveItemWeight(GetGrabbable(),GetPlayerName());
    }

    public void Grab()
    {
        if (GetGrabbable() == null) return;
        if (isGrabbing == false)  return;
        if (GetGrabbable().State == ItemState.Grabbed)  return;
        GetGrabbable().Grab(pointTarget);
        isGrabbing = false;
    }

    public void Store()
    {
        if(!GetGrabbable().Data.isStorable)return;
        Item item = GetGrabbable() as Item;
        if(!weightManager.CheckIfItemCanBeAdded(item,GetPlayerName()))return;
        audioManager.PlaySoundEffect(storingItemsSound);
        weightManager.AddItemWeight(item,GetPlayerName());
        backpack.AddItemToBackPack(item);
    }
}
