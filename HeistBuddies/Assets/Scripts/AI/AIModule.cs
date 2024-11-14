using UnityEngine;

public abstract class AIModule : MonoBehaviour
{
    protected AIBrain brain = null;

    protected virtual void Awake()
    {
        brain = GetComponentInParent<AIBrain>();
    }
}

