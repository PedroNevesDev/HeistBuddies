using System;
using System.Collections.Generic;
using UnityEngine;

public enum GlobalEvent
{
    DogAlert,
    SoundAlert,
}

public enum LocalEvent
{
    PlayerFound,
    PlayerLost,
    PlayerGrabbed,
}

public static class EventManager
{
    private static readonly Dictionary<GlobalEvent, Action<EventData>> globalEvents = new Dictionary<GlobalEvent, Action<EventData>>();
    private static readonly Dictionary<LocalEvent, Action<EventData>> localEvents = new Dictionary<LocalEvent, Action<EventData>>();

    public static void SubscribeToGlobalEvent(GlobalEvent eventType, Action<EventData> callback)
    {
        if (!globalEvents.ContainsKey(eventType))
            globalEvents[eventType] = null;

        globalEvents[eventType] += callback;
    }

    public static void UnsubscribeFromGlobalEvent(GlobalEvent eventType, Action<EventData> callback)
    {
        if (globalEvents.ContainsKey(eventType))
            globalEvents[eventType] -= callback;
    }

    public static void SubscribeToLocalEvent(LocalEvent eventType, Action<EventData> callback)
    {
        if (!localEvents.ContainsKey(eventType))
            localEvents[eventType] = null;

        localEvents[eventType] += callback;
    }

    public static void UnsubscribeFromLocalEvent(LocalEvent eventType, Action<EventData> callback)
    {
        if (localEvents.ContainsKey(eventType))
            localEvents[eventType] -= callback;
    }

    public static void InvokeGlobalEvent(GlobalEvent eventType, EventData eventData)
    {
        if (globalEvents.ContainsKey(eventType))
        {
            Debug.Log($"Global Event Triggered with data: {eventType}");
            globalEvents[eventType]?.Invoke(eventData);
        }
        else
        {
            Debug.LogWarning($"Global Event {eventType} has no subscribers.");
        }
    }

    public static void InvokeLocalEvent(LocalEvent eventType, EventData eventData)
    {
        if (localEvents.ContainsKey(eventType))
        {
            Debug.Log($"Local Event Triggered: {eventType} with data: {eventData}");
            localEvents[eventType]?.Invoke(eventData);
        }
        else
        {
            Debug.LogWarning($"Local Event {eventType} has no subscribers");
        }
    }
}

