using UnityEngine;

public class AIBrainDog : AIBrain
{
    protected override void OnEnable()
    {
        //GLOBAL EVENTS

        //LOCAL EVENTS
        EventManager.SubscribeToLocalEvent(LocalEvent.PlayerFound, OnLocalPlayerFound);
    }

    protected override void OnDisable()
    {
        //GLOBAL EVENTS

        //LOCAL EVENTS
        EventManager.UnsubscribeFromLocalEvent(LocalEvent.PlayerFound, OnLocalPlayerFound);
    }

    #region Local Events Callbacks

    private void OnLocalPlayerFound(EventData eventData)
    {
        if (eventData is PlayerEventData playerData && playerData.TargetBrain == this)
            EventManager.InvokeGlobalEvent(GlobalEvent.DogAlert, eventData);
    }

    #endregion
}
