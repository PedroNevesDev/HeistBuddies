using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : AIState
{
    public override AIStateType StateType => AIStateType.Patrol;

    [Header("State Settings")]
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>();
    private int currentPatrolIndex = 0;

    [Header("Idle Settings")]
    [SerializeField] private Vector2 idleDurationRange = new Vector2(2f, 5f);
    [SerializeField, Range(0f, 1f)] private float idleChance = 0.5f;
    private bool isIdling = false;
    private float idleTimer = 0f;
    private float idleDuration = 0f;

    public override void OnStateEnter()
    {
        brain.SetCanDetect(true);

        isIdling = false;
        MoveToNextPatrolPoint();

        agent.isStopped = false;
    }

    public override void OnStateUpdate()
    {
        if (isIdling)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDuration)
            {
                isIdling = false;
                MoveToNextPatrolPoint();
            }
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                float randomValue = Random.value;
                if (randomValue < idleChance)
                {
                    StartIdling();
                }
                else
                {
                    MoveToNextPatrolPoint();
                }
            }
        }

        if (hearingModule.HasHeardSound)
        {
            brain.TransitionToState(AIStateType.Investigate);
            return;
        }
    }

    public override void OnStateExit()
    {
        isIdling = false;

        agent.isStopped = false;
    }

    private void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Count == 0) return;

        int nextPatrolIndex = Random.Range(0, patrolPoints.Count);

        while (nextPatrolIndex == currentPatrolIndex && patrolPoints.Count > 1)
        {
            nextPatrolIndex = Random.Range(0, patrolPoints.Count);
        }

        currentPatrolIndex = nextPatrolIndex;
        agent.destination = patrolPoints[currentPatrolIndex].position;
        agent.isStopped = false;
    }

    private void StartIdling()
    {
        isIdling = true;
        idleTimer = 0f;
        idleDuration = Random.Range(idleDurationRange.x, idleDurationRange.y);
        agent.isStopped = true;
    }

    public void SetSinglePatrolPosition(Transform patrolPoint)
    {
        patrolPoints.Clear();
        patrolPoints.Add(patrolPoint);

        MoveToNextPatrolPoint();
    }
}
