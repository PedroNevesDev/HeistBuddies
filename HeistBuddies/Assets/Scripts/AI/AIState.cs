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

    public virtual void OnStateUpdate() 
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 position = new Vector3(agent.destination.x, 0f, agent.destination.z);
            brain.transform.LookAt(agent.destination);
            brain.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }

    public virtual void OnStateExit() { }
}
