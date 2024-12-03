using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    [SerializeField] LayerMask groundMask;
    [SerializeField] float raycastDistance;
    [SerializeField] Transform origin;
    [SerializeField] Balance balance;
    // Update is called once per frame
    void Update()
    {
        if(!balance)return;
        balance.gameObject.SetActive(CheckForGround());
    }

    public bool CheckForGround()
    {
        return Physics.Raycast(origin.position,Vector3.down,raycastDistance,groundMask);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin.position,origin.position-Vector3.down*raycastDistance);
    }
}
