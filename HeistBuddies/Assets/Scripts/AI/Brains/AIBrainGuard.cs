using UnityEngine;

public class AIBrainGuard : AIBrain
{
    [Header("UI Panels")]
    [SerializeField] private GameObject alertPanel;
    [SerializeField] private GameObject confusionPanel;

    [Header("Scriptable Events")]
    [SerializeField] private DogAlertEvent DogAlertEvent;
    [SerializeField] private PlayerFoundEvent PlayerFoundEvent;
    [SerializeField] private PlayerLostEvent PlayerLostEvent;
    [SerializeField] private PlayerGrabbedEvent PlayerGrabbedEvent;

    protected override void OnEnable()
    {
        //GLOBAL EVENTS
        DogAlertEvent.Subscribe(OnDogAlert);
        //EventManager.SubscribeToGlobalEvent(GlobalEvent.SoundAlert, OnGlobalSoundAlert);

        //LOCAL EVENTS
        PlayerFoundEvent.Subscribe(OnPlayerFound);
        PlayerLostEvent.Subscribe(OnPlayerLost);
        PlayerGrabbedEvent.Subscribe(OnPlayerGrabbed);
    }

    protected override void OnDisable()
    {
        //GLOBAL EVENTS
        DogAlertEvent.Unsubscribe(OnDogAlert);
        //EventManager.UnsubscribeFromGlobalEvent(GlobalEvent.SoundAlert, OnGlobalSoundAlert);

        //LOCAL EVENTS
        PlayerFoundEvent.Unsubscribe(OnPlayerFound);
        PlayerLostEvent.Unsubscribe(OnPlayerLost);
        PlayerGrabbedEvent.Unsubscribe(OnPlayerGrabbed);
    }

    public void EnableAlertPanel() => alertPanel.SetActive(true);

    public void DisableAlertPanel() => alertPanel.SetActive(false);

    public void EnableConfusionPanel() => confusionPanel.SetActive(true);

    public void DisableConfusionPanel() => confusionPanel.SetActive(false);

    #region Global Events Callbacks

    private void OnDogAlert(EventData eventData)
    {
        if (eventData is PositionEventData positionData)
        {
            SetInvestigateTarget(positionData.Position);
        }

        TransitionToState(AIStateType.Investigate);
    }

    private void OnGlobalSoundAlert(EventData eventData)
    {
        if (eventData is PositionEventData positionData)
        {
            SetInvestigateTarget(positionData.Position);
        }

        TransitionToState(AIStateType.Investigate);
    }

    #endregion

    #region Local Events Callbacks

    private void OnPlayerFound(EventData eventData)
    {
        if (eventData.TargetBrain == this)
            TransitionToState(AIStateType.Chase);
    }

    private void OnPlayerLost(EventData eventData)
    {
        if (eventData.TargetBrain == this)
            TransitionToState(AIStateType.Confusion);
    }

    private void OnPlayerGrabbed(EventData eventData)
    {
        if (eventData.TargetBrain == this)
        {
            SetCanDetect(false);
            TransitionToState(AIStateType.Grab);
        }
    }

    #endregion
}
