using UnityEngine;
using System.Collections.Generic;

public abstract class GameEvent<T> : ScriptableObject
{
    private readonly List<GameEventListener<T>> eventListeners = new();

    public void Raise(T item)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
        {
            eventListeners[i].OnEventRaised(item);
        }
    }

    public void RegisterListener(GameEventListener<T> listener) => eventListeners.Add(listener);
    public void UnregisterListener(GameEventListener<T> listener) => eventListeners.Remove(listener);
}
