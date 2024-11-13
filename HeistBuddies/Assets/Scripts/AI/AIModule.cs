using UnityEngine;

public abstract class AIModule : MonoBehaviour
{
    protected AIBrain brain = null;

    public void Initialize(AIBrain brain)
    {
        this.brain = brain;
    }
}

