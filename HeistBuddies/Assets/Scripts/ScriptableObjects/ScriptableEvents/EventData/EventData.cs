using UnityEngine;

public class EventData
{
    public AIBrain TargetBrain { get; }

    public EventData(AIBrain targetBrain)
    {
        TargetBrain = targetBrain;
    }
}