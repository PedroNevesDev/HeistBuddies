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
    var step = 5 * Time.deltaTime;
    // Calculate direction to target for upper arms
    Vector3 rightDirectionToTarget = target.position - rightArmJoint.transform.position;
    Vector3 leftDirectionToTarget = target.position - leftArmJoint.transform.position;

    Quaternion rightUpperArmTargetRotation = Quaternion.Inverse(rightArmJoint.transform.rotation) * Quaternion.LookRotation(rightDirectionToTarget, rightArmJoint.transform.right);
    Quaternion leftUpperArmTargetRotation = Quaternion.Inverse(leftArmJoint.transform.rotation) * Quaternion.LookRotation(leftDirectionToTarget, leftArmJoint.transform.right);

    rightArmJoint.targetRotation = Quaternion.Slerp(rightArmJoint.targetRotation, rightUpperArmTargetRotation, step);
    leftArmJoint.targetRotation = Quaternion.Slerp(leftArmJoint.targetRotation, leftUpperArmTargetRotation, step);

    // If forearms exist, calculate direction and rotation for them too
    if (rightForearmJoint != null && leftForearmJoint != null)
    {
        Vector3 rightForearmDirectionToTarget = target.position - rightForearmJoint.transform.position;
        Vector3 leftForearmDirectionToTarget = target.position - leftForearmJoint.transform.position;

        Quaternion rightForearmTargetRotation = Quaternion.Inverse(rightForearmJoint.transform.rotation) * Quaternion.LookRotation(rightForearmDirectionToTarget, rightForearmJoint.transform.right);
        Quaternion leftForearmTargetRotation = Quaternion.Inverse(leftForearmJoint.transform.rotation) * Quaternion.LookRotation(leftForearmDirectionToTarget, leftForearmJoint.transform.right);

        rightForearmJoint.targetRotation = Quaternion.Slerp(rightForearmJoint.targetRotation, rightForearmTargetRotation, step);
        leftForearmJoint.targetRotation = Quaternion.Slerp(leftForearmJoint.targetRotation, leftForearmTargetRotation, step);
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
        backpack.RemoveItemWeight(GetGrabbable());
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
        if(!backpack.CheckIfItemCanBeAdded(item))return;
        audioManager.PlaySoundEffect(storingItemsSound);
        backpack.AddItemWeight(item);
        backpack.AddItemToBackPack(item);
    }
    void OnDrawGizmos()
{
    if (!target)
    {
        return; // No target to draw lines to
    }

    // Check if the joints are assigned
    if (rightArmJoint != null && leftArmJoint != null)
    {
        // Draw line from right arm to target
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rightArmJoint.transform.position, target.position);

        // Draw line from left arm to target
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(leftArmJoint.transform.position, target.position);

        // If forearms exist, visualize them as well
        if (rightForearmJoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(rightForearmJoint.transform.position, target.position);
        }

        if (leftForearmJoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftForearmJoint.transform.position, target.position);
        }
    }
    else
    {
        Debug.LogWarning("One or more arm joints are not assigned.");
    }
}
}
