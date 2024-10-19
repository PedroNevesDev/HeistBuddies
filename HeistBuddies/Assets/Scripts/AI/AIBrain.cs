using UnityEngine;
using System.Collections.Generic;

public enum AIStateType
{
    None,
    Patrol,
    Chase,
    Confusion,
    Investigate
}

public class AIBrain : MonoBehaviour
{
    [Header("Brain Settings")]
    [SerializeField] private AIStateType initialStateType = AIStateType.Patrol;
    private Dictionary<AIStateType, AIState> stateDictionary = new Dictionary<AIStateType, AIState>();
    private AIState currentState;

    [Header("AI Modules")]
    [SerializeField] private List<AIModule> aiModules = new List<AIModule>();

    [Header("UI Panels")]
    [SerializeField] private GameObject alertPanel;
    [SerializeField] private GameObject confusionPanel;

    public AIStateType CurrentStateType => currentState != null ? currentState.StateType : AIStateType.None;

    public Transform TargetPlayer { get; private set; }

    private void Awake()
    {
        // Initialize AI States
        AIState[] stateComponents = GetComponents<AIState>();
        foreach (AIState state in stateComponents)
        {
            stateDictionary[state.StateType] = state;
            state.enabled = false;
        }

        // Initialize AI Modules
        AIModule[] moduleComponents = GetComponents<AIModule>();
        foreach (AIModule module in moduleComponents)
        {
            aiModules.Add(module);
            module.InitializeModule(this);
        }
    }

    private void Start()
    {
        TransitionToState(initialStateType);
    }

    private void Update()
    {
        currentState?.OnStateUpdate();
    }

    public void TransitionToState(AIStateType newStateType)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
            currentState.enabled = false;
        }

        if (stateDictionary.TryGetValue(newStateType, out AIState newState))
        {
            currentState = newState;
            currentState.enabled = true;
            currentState.OnStateEnter();
        }
        else
        {
            Debug.LogError($"State {newStateType} not found on {gameObject.name}");
        }
    }

    public T GetModule<T>() where T : AIModule
    {
        foreach (AIModule module in aiModules)
        {
            if (module is T)
                return module as T;
        }
        return null;
    }

    public void SetTargetPlayer(Transform playerTransform) => TargetPlayer = playerTransform;

    public void EnableAlertPanel() => alertPanel.SetActive(true);

    public void DisableAlertPanel() => alertPanel.SetActive(false);

    public void EnableConfusionPanel() => confusionPanel.SetActive(true);

    public void DisableConfusionPanel() => confusionPanel.SetActive(false);
}

