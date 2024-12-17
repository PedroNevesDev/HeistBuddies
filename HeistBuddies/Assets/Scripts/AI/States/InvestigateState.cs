using UnityEngine;
using UnityEngine.AI;

public class InvestigateState : AIState
{
    public override AIStateType StateType => AIStateType.Investigate;

    [Header("Investigate Settings")]
    [SerializeField] private float investigationDuration = 5f;
    [SerializeField] private bool isInLocation = false;
    private float investigationTimer = 0f;
    private Vector3 investigatePosition = Vector3.zero;

    public override void OnStateEnter()
    {
        investigationTimer = investigationDuration;

        agent.destination = investigatePosition;

        var newBrain = brain as AIBrainGuard;
        newBrain.EnableAlertPanel();

        agent.isStopped = false;
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        if (isInLocation)
            investigationTimer -= Time.deltaTime;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            isInLocation = true;
            investigatePosition = AIHelpers.GetRandomSearchPosition(1f, transform);
            agent.destination = investigatePosition;
        }

        if (investigationTimer <= 0f)
        {
            brain.TransitionToState(AIStateType.Patrol);
            return;
        }
    }

    public override void OnStateExit()
    {
        isInLocation = false;
        investigationTimer = 0f;

        var newBrain = brain as AIBrainGuard;
        newBrain.DisableAlertPanel();

        agent.isStopped = false;
    }

    public void SetTargetToInvestigate(Vector3 position)
    {
        investigatePosition = position;
    }
}
