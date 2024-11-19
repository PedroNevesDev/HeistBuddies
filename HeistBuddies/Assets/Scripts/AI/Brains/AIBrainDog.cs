using UnityEngine;

public class AIBrainDog : AIBrain
{
    protected override void OnEnable()
    {
        //GLOBAL EVENTS

        //LOCAL EVENTS
        EventManager.SubscribeToLocalEvent(LocalEvent.PlayerFound, OnLocalPlayerFound);
        EventManager.SubscribeToLocalEvent(LocalEvent.PlayerLost, OnLocalPlayerLost);
    }

    protected override void OnDisable()
    {
        //GLOBAL EVENTS

        //LOCAL EVENTS
        EventManager.UnsubscribeFromLocalEvent(LocalEvent.PlayerFound, OnLocalPlayerFound);
        EventManager.UnsubscribeFromLocalEvent(LocalEvent.PlayerLost, OnLocalPlayerLost);
    }

    #region Local Events Callbacks

    private void OnLocalPlayerFound(EventData eventData)
    {
        if (eventData.TargetBrain == this)
            TransitionToState(AIStateType.Chase);
            //EventManager.InvokeGlobalEvent(GlobalEvent.DogAlert, eventData);
    }

    private void OnLocalPlayerLost(EventData eventData)
    {
        if (eventData.TargetBrain == this)
            TransitionToState(AIStateType.Patrol);
    }

    #endregion
}
