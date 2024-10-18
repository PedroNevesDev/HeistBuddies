using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    [SerializeField] private Vector3Event soundEvent;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EmitSound();
        }
    }

    private void EmitSound()
    {
        if (soundEvent != null)
        {
            soundEvent.Raise(transform.position);
            Debug.Log("Sound emitted at position: " + transform.position);
        }
    }
}
