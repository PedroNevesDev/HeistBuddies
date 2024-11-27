using UnityEngine;

[CreateAssetMenu(fileName = "PlayerLostEvent", menuName = "Events/Player/PlayerLostEvent")]
public class PlayerLostEvent : ScriptableEvent<PositionEventData> { }
