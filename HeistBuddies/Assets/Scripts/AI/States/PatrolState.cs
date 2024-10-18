using UnityEngine;
using UnityEngine.AI;

public class PatrolState : AIState
{
    public override AIStateType StateType => AIStateType.Patrol;

    [Header("State Settings")]
    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    [Header("Idle Settings")]
    [SerializeField] private Vector2 idleDurationRange = new Vector2(2f, 5f);
    [SerializeField, Range(0f, 1f)] private float idleChance = 0.5f;
    private bool isIdling = false;
    private float idleTimer = 0f;
    private float idleDuration = 0f;

    private NavMeshAgent agent;
    private DetectionModule detectionModule;
    private HearingModule hearingModule;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();

        detectionModule = brain.GetModule<DetectionModule>();
        if (detectionModule == null)
        {
            Debug.LogError("DetectionModule module not found!");
        }

        hearingModule = brain.GetModule<HearingModule>();
        if (hearingModule == null)
        {
            Debug.LogError("HearingModule not found!");
        }
    }

    public override void OnStateEnter()
    {
        agent.isStopped = false;
        isIdling = false;
        MoveToNextPatrolPoint();
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

        if (detectionModule != null && detectionModule.IsPlayerVisible)
        {
            brain.SetTargetPlayer(detectionModule.DetectedPlayer);
            brain.TransitionToState(AIStateType.Chase);
            return;
        }

        if (hearingModule.HasHeardSound)
        {
            brain.TransitionToState(AIStateType.Investigate);
            return;
        }
    }

    public override void OnStateExit()
    {
        agent.isStopped = true;
        isIdling = false;
    }

    private void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        int nextPatrolIndex = Random.Range(0, patrolPoints.Length);

        while (nextPatrolIndex == currentPatrolIndex && patrolPoints.Length > 1)
        {
            nextPatrolIndex = Random.Range(0, patrolPoints.Length);
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
}
