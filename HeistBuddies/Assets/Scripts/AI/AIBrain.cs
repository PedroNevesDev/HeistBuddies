using UnityEngine;
using System.Collections.Generic;

public enum AIStateType
{
    Patrol
}


public class AIBrain : MonoBehaviour
{
    [Header("Brain Settings")]
    [SerializeField] private AIStateType initialStateType = AIStateType.Patrol;
    private Dictionary<AIStateType, AIState> stateDictionary = new Dictionary<AIStateType, AIState>();
    private AIState currentState;

    public Transform targetPlayer { get; private set; }

    private void Awake()
    {
        // Find and cache all AIState components attached to this GameObject
        AIState[] stateComponents = GetComponents<AIState>();
        foreach (AIState state in stateComponents)
        {
            stateDictionary[state.StateType] = state;
            state.enabled = false;
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

    public void SetTargetPlayer(Transform playerTransform)
    {
        targetPlayer = playerTransform;
    }
}

