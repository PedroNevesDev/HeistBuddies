using UnityEngine;
using UnityEngine.AI;

public class ChaseState : AIState
{
    public override AIStateType StateType => AIStateType.Chase;

    public override void OnStateEnter()
    {
        agent.isStopped = false;

        brain.EnableAlertPanel();
    }

    public override void OnStateUpdate()
    {
        if (detectionModule.IsPlayerGrabbable)
        {
            brain.TransitionToState(AIStateType.Grab);
            return;
        }
        else if (detectionModule.IsPlayerVisible)
        {
            agent.destination = detectionModule.DetectedPlayer.position;
        }
        else
        {
            brain.TransitionToState(AIStateType.Confusion);
            return;
        }
    }

    public override void OnStateExit()
    {
        agent.isStopped = true;

        brain.DisableAlertPanel();
    }
}

