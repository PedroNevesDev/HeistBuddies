using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ActiveRagdoll : MonoBehaviour
{
    [SerializeField] List<AnimatedJoint> joints = new List<AnimatedJoint>();
    List<Quaternion> startRots = new List<Quaternion>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(AnimatedJoint j in joints)
        {
            startRots.Add(j.AnimatedCounterpart.localRotation);

        }

    }



    void FixedUpdate() 
    {
        foreach(AnimatedJoint j in joints)
        {
            j.Joint.targetRotation = j.AnimatedCounterpart.transform.localRotation * Quaternion.Inverse(startRots[joints.IndexOf(j)]);
            j.Joint.transform.localScale = j.AnimatedCounterpart.localScale;

        }
    }

	

    [System.Serializable]
    public struct AnimatedJoint
    {
        [SerializeField] ConfigurableJoint joint;
        [SerializeField] Transform animatedCounterpart;
        public ConfigurableJoint Joint { get => joint; set => joint = value; }
        public Transform AnimatedCounterpart { get => animatedCounterpart; set => animatedCounterpart = value; }
    }
    
}
