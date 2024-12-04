using UnityEngine;
using System.Collections.Generic;
using TMPro;

public enum AIStateType
{
    None,
    Patrol,
    Chase,
    Confusion,
    Investigate,
    Grab,
    Alert
}

public class AIBrain : MonoBehaviour
{
    [Header("Brain Settings")]
    [SerializeField] protected AIStateType initialStateType = AIStateType.Patrol;
    protected Dictionary<AIStateType, AIState> stateDictionary = new Dictionary<AIStateType, AIState>();
    protected AIState currentState;

    [Header("AI Modules")]
    [SerializeField] protected List<AIModule> aiModules = new List<AIModule>();

    public AIStateType CurrentStateType => currentState != null ? currentState.StateType : AIStateType.None;

    public Transform TargetPlayer { get; private set; }

    protected virtual void Awake()
    {
        Initialization();
    }

    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {
        
    }

    protected virtual void Start()
    {
        TransitionToState(initialStateType);
    }

    protected virtual void Update()
    {
        currentState?.OnStateUpdate();
    }

    protected virtual void Initialization()
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
        }
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

    public void SetInvestigateTarget(Vector3 targetPosition)
    {
        if (stateDictionary.TryGetValue(AIStateType.Investigate, out AIState state))
        {
            InvestigateState investigateState = state as InvestigateState;
            if (investigateState != null)
            {
                investigateState.SetTargetToInvestigate(targetPosition);
            }
            else
            {
                Debug.LogWarning("Investigate state is not correctly configured in the state dictionary.");
            }
        }
        else
        {
            Debug.LogError("Investigate state not found in state dictionary.");
        }
    }

    public void SetSinglePatrolPosition(Transform patrolPosition)
    {
        if (stateDictionary.TryGetValue(AIStateType.Patrol, out AIState state))
        {
            PatrolState patrolState = state as PatrolState;
            if (patrolState != null)
            {
                patrolState.SetSinglePatrolPosition(patrolPosition);
            }
            else
            {
                Debug.LogWarning("Investigate state is not correctly configured in the state dictionary.");
            }
        }
        else
        {
            Debug.LogError("Investigate state not found in state dictionary.");
        }
    }

    public void SetCanDetect(bool condition)
    {
        DetectionModule module = GetModule<DetectionModule>();

        if (module != null)
        {
            if (condition)
            {
                module.CanDetect = true;
            }
            else
            {
                module.CanDetect = false;
            }
        }
    }
}

