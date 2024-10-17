using UnityEngine;
using UnityEngine.Events;

public class GameEventListener<T> : MonoBehaviour
{
    public GameEvent<T> gameEvent;
    public UnityEvent<T> response;

    private void OnEnable() => gameEvent?.RegisterListener(this);
    private void OnDisable() => gameEvent?.UnregisterListener(this);

    public void OnEventRaised(T item) => response?.Invoke(item);
}
