using UnityEngine;
using UnityEngine.AI;

public class ChaseState : AIState
{
    public override AIStateType StateType => AIStateType.Chase;

    private NavMeshAgent agent;
    private DetectionModule detectionModule;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();

        detectionModule = brain.GetModule<DetectionModule>();
        if (detectionModule == null)
        {
            Debug.LogError("AIDetection module not found!");
        }
    }

    public override void OnStateEnter()
    {
        agent.isStopped = false;
    }

    public override void OnStateUpdate()
    {
        if (detectionModule != null)
        {
            if (detectionModule.IsPlayerGrabbable)
            {
                // Transition to GrabState
                Debug.Log("Grabbing Player");
                return;
            }
            else if (detectionModule.IsPlayerVisible)
            {
                agent.destination = detectionModule.DetectedPlayer.position;
            }
            else
            {
                brain.TransitionToState(AIStateType.Confusion);
            }
        }
    }

    public override void OnStateExit()
    {
        agent.isStopped = true;
    }
}

