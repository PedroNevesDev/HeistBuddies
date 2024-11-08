using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrabState : AIState
{
    public override AIStateType StateType => AIStateType.Grab;

    [Header("State Settings")]
    [SerializeField] private float impulseForceUp = 500f;
    [SerializeField] private float impulseForceForward = 500f;

    private NavMeshAgent agent = null;
    private DetectionModule detectionModule = null;

    private PlayerController player = null;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();

        detectionModule = brain.GetModule<DetectionModule>();
        if (detectionModule == null)
        {
            Debug.LogError("AIDetection module not found!");
        }
    }

    public override void OnStateEnter()
    {
        agent.isStopped = true;

        player = detectionModule.DetectedPlayer.GetComponentInParent<PlayerController>();
        player.CanMove = false;

        // Turn this into a coroutine
        player.ChangePlayerBodyParts();
        player.balance.ShouldBalance = false;
        player.LaunchPlayer(impulseForceUp, impulseForceForward);

        brain.TransitionToState(AIStateType.Patrol);
    }

    public override void OnStateUpdate()
    {
    
    }

    public override void OnStateExit()
    {
        player.ResetPlayerBodyParts();
        player = null;
    }
}
