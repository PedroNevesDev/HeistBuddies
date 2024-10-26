using UnityEngine;
using UnityEngine.AI;

public class ChaseState : AIState
{
    public override AIStateType StateType => AIStateType.Chase;

    private NavMeshAgent agent = null;
    private DetectionModule detectionModule = null;

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

        brain.EnableAlertPanel();
    }

    public override void OnStateUpdate()
    {
        if (detectionModule != null)
        {
            if (detectionModule.IsPlayerGrabbable)
            {
                // TODO: IMPLEMENT GRABBING
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

        brain.DisableAlertPanel();
    }
}

