using UnityEngine;

public class AIBrainDog : AIBrain
{
    [Header("Scriptable Events")]
    [SerializeField] private DogAlertEvent DogAlertEvent;
    [SerializeField] private PlayerFoundEvent PlayerFoundEvent;
    [SerializeField] private PlayerLostEvent PlayerLostEvent;

    protected override void OnEnable()
    {
        //GLOBAL EVENTS

        //LOCAL EVENTS
        PlayerFoundEvent.Subscribe(OnPlayerFound);
        PlayerLostEvent.Subscribe(OnPlayerLost);
    }

    protected override void OnDisable()
    {
        //GLOBAL EVENTS

        //LOCAL EVENTS
        PlayerFoundEvent.Unsubscribe(OnPlayerFound);
        PlayerLostEvent.Unsubscribe(OnPlayerLost);
    }

    #region Local Events Callbacks

    private void OnPlayerFound(EventData eventData)
    {
        if (eventData.TargetBrain == this)
            //TransitionToState(AIStateType.Chase);
            DogAlertEvent.Invoke(eventData);
    }

    private void OnPlayerLost(EventData eventData)
    {
        if (eventData.TargetBrain == this)
            TransitionToState(AIStateType.Patrol);
    }

    #endregion
}
