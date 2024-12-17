using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Variables

    [SerializeField,Tooltip("All the player modules can be found here. Each one works individually in order to decrease dependencies and increase abstraction.")] 
    Modules playerModules;
    [SerializeField] Rigidbody rb;

    [Header("Player Conditions")]
    [SerializeField] private bool wasTeleported = false;
    [SerializeField] private bool wasGrabbed = false;
    [SerializeField] Balance balance;

    // All bodyparts can be here if needed. Used to store a reference into player controller owner
    [SerializeField,Tooltip("Assigns this character reference to every body part script added here. Makes searching for collisions easier")] 
    BodyPartOwner[] bodyParts;

    private List<float> playerBodyParts = new List<float>();
    
    private Transform teleportPoint = null;
    private Coroutine teleportCoroutine = null;

    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;

    [SerializeField] GameObject puffParticle;

    public void SetMesh(Mesh mesh)=> skinnedMeshRenderer.sharedMesh = mesh;

    #endregion

    #region Getters/Setters
    public bool WasGrabbed { get => wasGrabbed; set => wasGrabbed = value; }
    public bool WasTeleported { get => wasTeleported; set => wasTeleported = value; }

    public Rigidbody Rb { get => rb; set => rb = value; }
    public BodyPartOwner[] BodyParts { get => bodyParts; set => bodyParts = value; }
    public Modules PlayerModules { get => playerModules; set => playerModules = value; }
    public Balance Balance { get => balance; set => balance = value; }
    public SkinnedMeshRenderer SkinnedMeshRenderer { get => skinnedMeshRenderer; set => skinnedMeshRenderer = value; }

    public void OnPause(InputAction.CallbackContext context)
    {
        GameManager.Instance.TogglePause();
        Debug.Log("Pause Triggered");
    }

    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        playerModules.FindModules(this);

        teleportPoint = GameObject.Find("TeleportPoint").transform;

        foreach(BodyPartOwner bodyPart in BodyParts)
        {
            bodyPart.MyOwner = this;

            // Cache rigidbody mass
            Rigidbody rigidbody = bodyPart.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                playerBodyParts.Add(rigidbody.mass);
            }
        }
    }

    private void Update()
    {
        if (wasTeleported)
        {
            StopCoroutine(teleportCoroutine);
            wasTeleported = false;
        }
    }
    #endregion

    #region Player BodyParts

    public void ResetPlayerBodyParts()
    {
        for (int i = 0; i < bodyParts.Length; i++) 
        {
            Rigidbody rigidbody = bodyParts[i].GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.mass = playerBodyParts[i];
            }
        }
    }

    public void ChangePlayerBodyParts()
    {
        for (int i = 0; i < bodyParts.Length; i++)
        {
            Rigidbody rigidbody = bodyParts[i].GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.mass = 1f;
            }
        }
    }

    #endregion

    #region Player Movement

    public void LaunchPlayer(float forceUp, float forceForward)
    {
        
        for (int i = 0; i < bodyParts.Length; i++)
        {
            Rigidbody rigidbody = bodyParts[i].GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddForce(Vector3.up * forceUp, ForceMode.Impulse);
                rigidbody.AddForce(Vector3.forward * forceForward, ForceMode.Impulse);
            }
        }
    }

    public void TeleportPlayer()
    {
        wasGrabbed = false;

        teleportCoroutine = StartCoroutine(Teleport());
    }
    
    private IEnumerator Teleport()
    {
        wasTeleported = false;
        transform.GetChild(0).gameObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        bodyParts[0].transform.position = teleportPoint.position;

        yield return new WaitForSeconds(0.1f);

        transform.GetChild(0).gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        if(playerModules.MovementModule!=null)
        {
            playerModules.MovementModule.enabled = true;
        }
        balance.ShouldBalance = true;
        wasTeleported = true;
        Instantiate(puffParticle,rb.transform.position,Quaternion.identity);
    }

    #endregion
    
    #region Structs (Modules)
    [System.Serializable]
    public struct Modules
    {
        [SerializeField] PlayerMovementModule movementModule;
        [SerializeField] PlayerGrabbingModule grabbingModule;
        [SerializeField] PlayerBackpackModule backpackModule;

        public PlayerMovementModule MovementModule { get => movementModule; set => movementModule = value; }
        public PlayerGrabbingModule GrabbingModule { get => grabbingModule; set => grabbingModule = value; }
        public PlayerBackpackModule BackpackModule { get => backpackModule; set => backpackModule = value; }

        public void FindModules(PlayerController pc)
        {
            movementModule = pc.GetComponent<PlayerMovementModule>();
            grabbingModule = pc.GetComponent<PlayerGrabbingModule>();
            backpackModule = pc.GetComponent<PlayerBackpackModule>();
        }
    }
    #endregion
}
