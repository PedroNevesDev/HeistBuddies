using UnityEngine;

public class PositionEventData : EventData
{
    public Vector3 Position { get; }

    public PositionEventData(AIBrain targetBrain, Vector3 position) : base(targetBrain)
    {
        Position = position;
    }
}
