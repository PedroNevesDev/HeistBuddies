using UnityEngine;

public class Balance : MonoBehaviour
{
    [SerializeField] Rigidbody bodyPart;
    [SerializeField] float upwardForce;
    [SerializeField] bool  balance;

    public bool ShouldBalance { get => balance; set => balance = value; }

    void FixedUpdate()
    {
        ApplyUpwardForce(); // Applies a force on the selected rigidbody. This helps the player to stand up. Aditionally a downwards can be applyed if the player is knocked out.
    }
    void ApplyUpwardForce()
    {
        if(!bodyPart)
            return;
        if(!balance)
            bodyPart.AddForce(upwardForce*2*-Vector3.up*Time.fixedDeltaTime,ForceMode.Force);
        else
            bodyPart.AddForce(upwardForce*Vector3.up*Time.fixedDeltaTime,ForceMode.Force);
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(bodyPart.transform.position,bodyPart.transform.position+(Vector3.up*2));
    }
}
