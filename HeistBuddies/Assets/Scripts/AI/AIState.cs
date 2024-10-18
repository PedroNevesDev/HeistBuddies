using UnityEngine;

public abstract class AIState : MonoBehaviour
{
    public abstract AIStateType StateType { get; }

    protected AIBrain brain;

    private void Awake()
    {
        brain = GetComponent<AIBrain>();
    }

    public virtual void OnStateEnter() { }
    public virtual void OnStateUpdate() { }
    public virtual void OnStateExit() { }
}
