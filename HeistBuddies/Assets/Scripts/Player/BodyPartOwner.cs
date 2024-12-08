using UnityEditor.Callbacks;
using UnityEngine;

public class BodyPartOwner : MonoBehaviour
{
    /* Summary: Created to be able to access body part player controller through colision detection for example*/
    PlayerController myOwner;
    ConfigurableJoint cj;

    [SerializeField]bool adjustJoint = false;

    void Start()
    {
        cj = GetComponent<ConfigurableJoint>();
        if(cj && adjustJoint)
        {
            JointDrive angularDrive = new JointDrive
            {
                positionSpring = 1000f, // Increase spring force
                positionDamper = 50f,  // Increase damping
                maximumForce = 1000f   // Ensure sufficient force
            };
            cj.angularXDrive = angularDrive;
            cj.angularYZDrive = angularDrive;
        }
    }

    void Update()
    {
    
    }
    public PlayerController MyOwner { get => myOwner; set => myOwner = value; }

    private void OnCollisionEnter(Collision collision)
    {
        if (myOwner.WasGrabbed && !myOwner.WasTeleported)
        {
            myOwner.TeleportPlayer();
        }
    }
}
