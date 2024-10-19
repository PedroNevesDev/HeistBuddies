using UnityEngine;
using UnityEngine.AI;

public class ConfusionState : AIState
{
    public override AIStateType StateType => AIStateType.Confusion;

    [Header("Confusion Settings")]
    [SerializeField] private float confusionDuration = 5f;
    private float confusionTimer = 0f;
    private Vector3 searchPosition;

    private NavMeshAgent agent;
    private DetectionModule detectionModule;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();

        detectionModule = brain.GetModule<DetectionModule>();
        if (detectionModule == null)
        {
            Debug.LogError("DetectionModule not found!");
        }
    }

    public override void OnStateEnter()
    {
        confusionTimer = confusionDuration;
        agent.isStopped = false;

        if (detectionModule != null)
        {
            searchPosition = detectionModule.LastKnownPlayerPosition;
            agent.destination = searchPosition;
        }
        else
        {
            agent.isStopped = true;
        }

        brain.EnableConfusionPanel();
    }

    public override void OnStateUpdate()
    {
        confusionTimer -= Time.deltaTime;

        if (detectionModule != null && detectionModule.IsPlayerVisible)
        {
            brain.SetTargetPlayer(detectionModule.DetectedPlayer);
            brain.TransitionToState(AIStateType.Chase);
            return;
        }

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

        brain.DisableConfusionPanel();
    }
}
