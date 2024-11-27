using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class ScriptableEvent<T> : ScriptableObject
{
    private Action<EventData> listeners;

    public void Subscribe(Action<EventData> listener)
    {
        listeners += listener;
    }

    public void Unsubscribe(Action<EventData> listener)
    {
        listeners -= listener;
    }

    public void Invoke(EventData data)
    {
        if (listeners != null)
        {
            Debug.Log($"Event {name} triggered by {data.TargetBrain}");
            listeners.Invoke(data);
        }
        else
        {
            Debug.LogWarning($"Event {name} triggered but no listeners are attached.");
        }
    }
}
