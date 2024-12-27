using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Animations.Rigging;

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


    PlayerController playerController;

    [SerializeField] TwoBoneIKConstraint lefttbikc;
    [SerializeField] TwoBoneIKConstraint righttbikc;
    [SerializeField] Transform target;

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


void PointArms()
{
    Item item = GetGrabbable();
    Transform realTarget = item?item.transform:target;
    lefttbikc.data.target = target;
    righttbikc.data.target = target;




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
    public void DeactivateThrowUI()
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
        GetGrabbable().Grab(pointTarget,playerController);
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
}
