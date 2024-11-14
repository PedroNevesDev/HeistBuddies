using UnityEngine;

public class AIBrainGuard : AIBrain
{
    [Header("UI Panels")]
    [SerializeField] private GameObject alertPanel;
    [SerializeField] private GameObject confusionPanel;

    protected override void OnEnable()
    {
        //GLOBAL EVENTS
        EventManager.SubscribeToGlobalEvent(GlobalEvent.DogAlert, OnGlobalDogAlert);
        EventManager.SubscribeToGlobalEvent(GlobalEvent.SoundAlert, OnGlobalSoundAlert);

        //LOCAL EVENTS
        EventManager.SubscribeToLocalEvent(LocalEvent.PlayerFound, OnLocalPlayerFound);
        EventManager.SubscribeToLocalEvent(LocalEvent.PlayerLost, OnLocalPlayerLost);
        EventManager.SubscribeToLocalEvent(LocalEvent.PlayerGrabbed, OnLocalPlayerGrabbed);
    }

    protected override void OnDisable()
    {
        //GLOBAL EVENTS
        EventManager.UnsubscribeFromGlobalEvent(GlobalEvent.DogAlert, OnGlobalDogAlert);
        EventManager.UnsubscribeFromGlobalEvent(GlobalEvent.SoundAlert, OnGlobalSoundAlert);

        //LOCAL EVENTS
        EventManager.UnsubscribeFromLocalEvent(LocalEvent.PlayerFound, OnLocalPlayerFound);
        EventManager.UnsubscribeFromLocalEvent(LocalEvent.PlayerLost, OnLocalPlayerLost);
        EventManager.UnsubscribeFromLocalEvent(LocalEvent.PlayerGrabbed, OnLocalPlayerGrabbed);
    }

    public void EnableAlertPanel() => alertPanel.SetActive(true);

    public void DisableAlertPanel() => alertPanel.SetActive(false);

    public void EnableConfusionPanel() => confusionPanel.SetActive(true);

    public void DisableConfusionPanel() => confusionPanel.SetActive(false);

    #region Global Events Callbacks

    private void OnGlobalDogAlert(EventData eventData)
    {
        if (eventData is PlayerEventData playerData)
        {
            SetInvestigateTarget(playerData.Position);
        }

        TransitionToState(AIStateType.Investigate);
    }

    private void OnGlobalSoundAlert(EventData eventData)
    {
        if (eventData is PlayerEventData playerData)
        {
            SetInvestigateTarget(playerData.Position);
        }

        TransitionToState(AIStateType.Investigate);
    }

    #endregion

    #region Local Events Callbacks

    private void OnLocalPlayerFound(EventData eventData)
    {
        if (eventData is PlayerEventData playerData && playerData.TargetBrain == this)
            TransitionToState(AIStateType.Chase);
    }

    private void OnLocalPlayerLost(EventData eventData)
    {
        if (eventData is PlayerEventData playerData && playerData.TargetBrain == this)
            TransitionToState(AIStateType.Confusion);
    }

    private void OnLocalPlayerGrabbed(EventData eventData)
    {
        if (eventData is PlayerEventData playerData && playerData.TargetBrain == this)
        {
            SetCanDetect(false);
            TransitionToState(AIStateType.Grab);
        }
    }

    #endregion
}
