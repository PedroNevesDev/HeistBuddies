using UnityEngine;
using UnityEngine.AI;

public class ChaseState : AIState
{
    public override AIStateType StateType => AIStateType.Chase;

    public override void OnStateEnter()
    {
        var newBrain = brain as AIBrainGuard;
        if (newBrain is AIBrainGuard)
        {
            newBrain.EnableAlertPanel();
        }

        agent.isStopped = false;
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        if (detectionModule.DetectedPlayer != null)
        {
            agent.destination = detectionModule.DetectedPlayer.position;
        }
    }

    public override void OnStateExit()
    {
        var newBrain = brain as AIBrainGuard;
        if (newBrain is AIBrainGuard)
        {
            newBrain.DisableAlertPanel();
        }

        agent.isStopped = false;
    }
}

