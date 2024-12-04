using UnityEngine;

public class AlertState : AIState
{
    public override AIStateType StateType => AIStateType.Alert;

    [Header("State Settings")]
    [SerializeField] private float timeBeforeAlert = 3f;
    private float timeBeforeAlertReset;

    [Header("Scriptable Events")]
    [SerializeField] private DogAlertEvent DogAlertEvent;

    private AIBrainDog brainDog;
    private bool isWaitingForAlert = false;

    protected override void Awake()
    {
        base.Awake();
        brainDog = brain as AIBrainDog;
    }

    public override void OnStateEnter()
    {
        timeBeforeAlertReset = timeBeforeAlert;
        StartAlertCountdown();

        agent.isStopped = true;
    }

    public override void OnStateUpdate()
    {
        if (brainDog.IsBoneReceived)
        {
            CancelAlert();
            HandleBoneReceived();
        }
    }

    public override void OnStateExit()
    {
        CancelAlert();
        base.OnStateExit();

        agent.isStopped = false;
    }

    private void StartAlertCountdown()
    {
        isWaitingForAlert = true;
        Invoke(nameof(TriggerAlert), timeBeforeAlert);
    }

    private void TriggerAlert()
    {
        if (!brainDog.IsBoneReceived)
        {
            var eventData = new PositionEventData(brain, this.transform.position, this.transform);
            DogAlertEvent.Invoke(eventData);
        }
        isWaitingForAlert = false;
    }

    private void CancelAlert()
    {
        if (isWaitingForAlert)
        {
            CancelInvoke(nameof(TriggerAlert));
            isWaitingForAlert = false;
        }
    }

    private void HandleBoneReceived()
    {
        brainDog.TransitionToState(AIStateType.Patrol);
    }
}
