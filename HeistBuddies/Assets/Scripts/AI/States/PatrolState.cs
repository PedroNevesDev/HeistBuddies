using UnityEngine;
using UnityEngine.AI;

public class PatrolState : AIState
{
    public override AIStateType StateType => AIStateType.Patrol;
    public Transform[] patrolPoints;
    public float speed = 3.5f;
    private int currentPatrolIndex = 0;
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public override void OnStateEnter()
    {
        agent.speed = speed;
        agent.isStopped = false;
        MoveToNextPatrolPoint();
    }

    public override void StateUpdate()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            MoveToNextPatrolPoint();
        }

        // Detection logic
        if (DetectPlayer())
        {
            
        }
    }

    public override void OnStateExit()
    {
        agent.isStopped = true;
    }

    private void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private bool DetectPlayer()
    {
        // Implement detection logic
        return false; // Placeholder
    }
}
