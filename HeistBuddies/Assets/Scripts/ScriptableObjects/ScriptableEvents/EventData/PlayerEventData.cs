using UnityEngine;

public class PlayerEventData : EventData
{
    public Vector3 Position { get; }

    public PlayerEventData(AIBrain targetBrain, Vector3 position) : base(targetBrain)
    {
        Position = position;
    }
}