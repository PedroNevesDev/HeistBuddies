using UnityEngine;

public class BodyPartOwner : MonoBehaviour
{
    /* Summary: Created to be able to access body part player controller through colision detection for example*/
    PlayerController myOwner;
    ConfigurableJoint cj;

    [SerializeField] bool adjustJoint = false;
    [SerializeField] GameObject ps;

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
        BodyPartOwner bodyPartOwner = collision.gameObject.GetComponent<BodyPartOwner>();
        if(ps&&bodyPartOwner?.myOwner!=myOwner)
        {
            Vector3 hitPoint = collision.contacts[0].point;
            GameObject go =Instantiate(ps,hitPoint,Quaternion.identity);
            Destroy(go,0.7f);
        }
    }
}
