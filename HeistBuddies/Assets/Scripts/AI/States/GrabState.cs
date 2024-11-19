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
        if (player.WasTeleported)
        {
            brain.TransitionToState(AIStateType.Patrol);
            return;
        }
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
        player.PlayerModules.MovementModule.enabled = false;
        player.WasGrabbed = true;

        player.ChangePlayerBodyParts();
        player.Balance.ShouldBalance = false;

        yield return new WaitForSeconds(0.1f);

        player.LaunchPlayer(impulseForceUp, impulseForceForward);
    }
}
