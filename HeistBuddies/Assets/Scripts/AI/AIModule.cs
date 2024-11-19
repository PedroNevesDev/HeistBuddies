using UnityEngine;

public enum DetectionType
{
    Guard,
    Dog
}

public abstract class AIModule : MonoBehaviour
{
    protected AIBrain brain = null;

    protected virtual void Awake()
    {
        brain = GetComponentInParent<AIBrain>();
    }
}

