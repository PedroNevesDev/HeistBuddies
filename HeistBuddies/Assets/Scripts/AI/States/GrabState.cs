using System.Collections;
using UnityEngine;

public class GrabState : AIState
{
    public override AIStateType StateType => AIStateType.Grab;

    [Header("State Settings")]
    [SerializeField] private float impulseForceUp = 500f;
    [SerializeField] private float impulseForceForward = 500f;

    private Coroutine grabCoroutine = null;
    private PlayerController player = null;

    public override void OnStateEnter()
    {
        agent.isStopped = true;

        player = detectionModule.DetectedPlayer.GetComponentInParent<PlayerController>();

        if (!player.WasGrabbed)
        {
            grabCoroutine = StartCoroutine(GrabPlayer());   
        }
        else
        {
            brain.TransitionToState(AIStateType.Patrol);
        }
    }

    public override void OnStateUpdate()
    {
    
    }

    public override void OnStateExit()
    {
        player.ResetPlayerBodyParts();
        player = null;

        if (grabCoroutine != null)
        {
            StopCoroutine(grabCoroutine);
        }
    }

    private IEnumerator GrabPlayer()
    {
        player.CanMove = false;
        player.WasGrabbed = true;

        player.ChangePlayerBodyParts();
        player.balance.ShouldBalance = false;

        yield return new WaitForSeconds(0.1f);

        player.LaunchPlayer(impulseForceUp, impulseForceForward);

        yield return new WaitForSeconds(0.1f);

        brain.TransitionToState(AIStateType.Patrol);
    }
}
