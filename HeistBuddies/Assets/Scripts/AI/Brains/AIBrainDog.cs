using UnityEngine;

public class AIBrainDog : AIBrain
{
    [Header("Scriptable Events")]
    [SerializeField] private DogAlertEvent DogAlertEvent;
    [SerializeField] private DogBoneReceivedEvent DogBoneReceivedEvent;
    [SerializeField] private PlayerFoundEvent PlayerFoundEvent;
    [SerializeField] private PlayerLostEvent PlayerLostEvent;

    private bool isBoneReceived = false;

    protected override void OnEnable()
    {
        DogBoneReceivedEvent.Subscribe(OnBoneReceived);

        PlayerFoundEvent.Subscribe(OnPlayerFound);
        PlayerLostEvent.Subscribe(OnPlayerLost);
    }

    protected override void OnDisable()
    {
        DogBoneReceivedEvent.Unsubscribe(OnBoneReceived);

        PlayerFoundEvent.Unsubscribe(OnPlayerFound);
        PlayerLostEvent.Unsubscribe(OnPlayerLost);
    }

    #region Events Callbacks

    private void OnBoneReceived(EventData eventData)
    {
        if (eventData is PositionEventData positionData)
        {
            isBoneReceived = true;
            SetSinglePatrolPosition(positionData.TransformPosition);
        }
    }

    private void OnPlayerFound(EventData eventData)
    {
        if (eventData.TargetBrain == this)
        {
            if (isBoneReceived) return;
            TransitionToState(AIStateType.Chase);
            //DogAlertEvent.Invoke(eventData);
        }
    }

    private void OnPlayerLost(EventData eventData)
    {
        if (eventData.TargetBrain == this)
            TransitionToState(AIStateType.Patrol);
    }

    #endregion
}
