using UnityEngine;
using UnityEngine.AI;

public class ConfusionState : AIState
{
    public override AIStateType StateType => AIStateType.Confusion;

    [Header("Confusion Settings")]
    [SerializeField] private float confusionDuration = 5f;
    private float confusionTimer = 0f;
    private Vector3 searchPosition = Vector3.zero;

    public override void OnStateEnter()
    {
        confusionTimer = confusionDuration;

        searchPosition = detectionModule.LastKnownPlayerPosition;
        agent.destination = searchPosition;

        var newBrain = brain as AIBrainGuard;
        newBrain.EnableConfusionPanel();

        agent.isStopped = false;
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        confusionTimer -= Time.deltaTime;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            searchPosition = AIHelpers.GetRandomSearchPosition(2f, transform);
            agent.destination = searchPosition;
        }

        if (confusionTimer <= 0f)
        {
            brain.TransitionToState(AIStateType.Patrol);
            return;
        }
    }

    public override void OnStateExit()
    {
        confusionTimer = 0f;

        var newBrain = brain as AIBrainGuard;
        newBrain.DisableConfusionPanel();

        agent.isStopped = false;
    }
}
