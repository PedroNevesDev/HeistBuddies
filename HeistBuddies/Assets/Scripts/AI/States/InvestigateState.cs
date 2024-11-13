using UnityEngine;
using UnityEngine.AI;

public class InvestigateState : AIState
{
    public override AIStateType StateType => AIStateType.Investigate;

    [Header("Investigate Settings")]
    [SerializeField] private float investigationDuration = 5f;
    private float investigationTimer = 0f;
    private Vector3 investigatePosition = Vector3.zero;

    public override void OnStateEnter()
    {
        investigationTimer = investigationDuration;
        agent.isStopped = false;

        investigatePosition = hearingModule.SoundSourcePosition;
        agent.destination = investigatePosition;

        hearingModule.ResetHearing();
        Debug.Log(gameObject.name + " is investigating sound at position: " + investigatePosition);

        brain.EnableAlertPanel();
    }

    public override void OnStateUpdate()
    {
        investigationTimer -= Time.deltaTime;

        if (detectionModule.IsPlayerVisible)
        {
            brain.SetTargetPlayer(detectionModule.DetectedPlayer);
            brain.TransitionToState(AIStateType.Chase);
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            investigatePosition = AIHelpers.GetRandomSearchPosition(2f, transform);
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
        agent.isStopped = true;

        brain.DisableAlertPanel();
    }
}
