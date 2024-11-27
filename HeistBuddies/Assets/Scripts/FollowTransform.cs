using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] Transform followTargetPosition;
    [SerializeField] Transform followTargetRotation;
    [SerializeField] Vector3 positionOffset;

    Quaternion startRotation;

    void Start()
    {
        startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(followTargetPosition) transform.position = followTargetPosition.position+positionOffset;
        if(followTargetRotation) transform.eulerAngles = new Vector3(transform.eulerAngles.x, followTargetRotation.eulerAngles.y, transform.eulerAngles.z);;
    }
}