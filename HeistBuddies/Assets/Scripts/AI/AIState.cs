using UnityEngine;
using UnityEngine.AI;

public abstract class AIState : MonoBehaviour
{
    public abstract AIStateType StateType { get; }

    protected AIBrain brain = null;
    protected NavMeshAgent agent = null;

    protected HearingModule hearingModule = null;
    protected DetectionModule detectionModule = null;

    protected virtual void Awake()
    {
        brain = GetComponent<AIBrain>();
        if (brain == null)
        {
            Debug.LogError("Brain not found!");
        }

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("Agent not found!");
        }

        detectionModule = brain.GetModule<DetectionModule>();
        if (detectionModule == null)
        {
            Debug.LogError("DetectionModule not found!");
        }

        hearingModule = brain.GetModule<HearingModule>();
        if (hearingModule == null)
        {
            Debug.LogError("HearingModule not found!");
        }
    }

    public virtual void OnStateEnter() { }
    public virtual void OnStateUpdate() { }
    public virtual void OnStateExit() { }
}
