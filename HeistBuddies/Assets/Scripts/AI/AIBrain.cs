using UnityEngine;
using System.Collections.Generic;

public enum AIStateType
{
    Idle,
    Patrol,
}


public class AIBrain : MonoBehaviour
{
    public AIStateType initialStateType = AIStateType.Idle;
    private Dictionary<AIStateType, AIState> stateDictionary = new Dictionary<AIStateType, AIState>();
    private AIState currentState;

    void Awake()
    {
        // Find and cache all AIState components attached to this GameObject
        AIState[] stateComponents = GetComponents<AIState>();
        foreach (AIState state in stateComponents)
        {
            stateDictionary[state.StateType] = state;
            state.enabled = false; // Disable all states initially
        }
    }

    void Start()
    {
        TransitionToState(initialStateType);
    }

    void Update()
    {
        currentState?.StateUpdate();
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
}

