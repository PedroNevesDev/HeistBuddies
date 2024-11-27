using UnityEngine;

public class PositionEventData : EventData
{
    public Vector3 Position { get; }
    public Transform TransformPosition { get; }

    public PositionEventData(AIBrain targetBrain, Vector3 position, Transform transformPosition) : base(targetBrain)
    {
        Position = position;
        TransformPosition = transformPosition;
    }
}
