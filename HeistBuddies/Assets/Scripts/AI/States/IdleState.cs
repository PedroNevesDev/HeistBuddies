using UnityEngine;

public class IdleState : AIState
{
    public override AIStateType StateType => AIStateType.Idle;
    public float idleDuration = 2f;
    private float timer = 0f;

    public override void OnStateEnter()
    {
        timer = 0f;
        // Set idle animation if necessary
        // animator.SetBool("isIdle", true);
    }

    public override void StateUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= idleDuration)
        {
            brain.TransitionToState(AIStateType.Patrol);
        }
    }

    public override void OnStateExit()
    {
        // Reset or clean up
        // animator.SetBool("isIdle", false);
    }
}
