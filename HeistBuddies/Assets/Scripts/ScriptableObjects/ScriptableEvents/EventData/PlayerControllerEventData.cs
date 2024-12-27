using UnityEngine;

public class PlayerControllerEventData : EventData
{
    public PlayerController PlayerController { get; }

    public PlayerControllerEventData(AIBrain targetBrain, PlayerController player) : base(targetBrain)
    {
        PlayerController = player;
    }
}
