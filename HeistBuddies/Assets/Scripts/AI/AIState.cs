using UnityEngine;

public abstract class AIState : MonoBehaviour
{
    public abstract AIStateType StateType { get; }

    protected AIBrain brain;

    void Awake()
    {
        brain = GetComponent<AIBrain>();
    }

    public virtual void OnStateEnter() { }
    public virtual void StateUpdate() { }
    public virtual void OnStateExit() { }
}
