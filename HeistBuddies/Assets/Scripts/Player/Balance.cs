using UnityEngine;

public class Balance : MonoBehaviour
{
    [SerializeField] Rigidbody bodyPart;
    [SerializeField] float upwardForce;
    [SerializeField]bool  balance;

    // Update is called once per frame
    void Update()
    {
        // Doing this will make the ragdoll try to be pulled by this object in the air because the one of the bones(spine) is connected to the kinematic rigidbody of the balance object through a configurable joint.
        // This would be the main way of balancing the ragdoll but i am not sure about it because 1 - it could be pulling the wrong bone(spine) because this rig doesn't follow the modern naming but its not far off and 2 - there forces applied in spine configurable joint could be wrong () 

        if(balance)
        {
            transform.position=bodyPart.transform.position; // Assigns this objects position to that of the main body part the torso/spine depending on the rig
            transform.rotation=bodyPart.transform.rotation; // Applies the the the rotation pretended piece of the object to this object. Because it would make the character fall sideways if not
        }

    }
    void FixedUpdate()
    {
        ApplyUpwardForce(); // Applies a force on the selected rigidbody. This helps the player to stand up. Aditionally a downwards can be applyed if the player is knocked out.
    }
    void ApplyUpwardForce()
    {
        if(!bodyPart)
            return;
        bodyPart.AddForce(upwardForce*Vector3.up*Time.fixedDeltaTime,ForceMode.Force);
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(bodyPart.transform.position,bodyPart.transform.position+(Vector3.up*2));
    }
}
