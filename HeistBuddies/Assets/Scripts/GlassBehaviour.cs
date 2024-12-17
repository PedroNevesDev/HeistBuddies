using System.Collections.Generic;
using UnityEngine;

public class GlassBehaviour : MonoBehaviour
{
    [SerializeField] GameObject glassPiecesObj;
    [SerializeField] float breakForce = 1;
    [SerializeField] float glassPiecesPropelForce = 1;
    [SerializeField] List<Rigidbody> glassPiecesRbs = new List<Rigidbody>();

    [SerializeField] AudioClip breakingSound;
    [SerializeField] private PositionEvent positionEvent;
    [SerializeField] private Transform glassPosisiton;

    private void OnCollisionEnter(Collision other) 
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if(rb && rb.angularVelocity.magnitude>= breakForce)
        {
            gameObject.SetActive(false);
            glassPiecesObj.SetActive(true);
            AudioManager.Instance.PlaySoundEffect(breakingSound);

            PositionEventData eventData = new PositionEventData(null, glassPosisiton.position, glassPosisiton);
            positionEvent.Invoke(eventData);

            foreach(Rigidbody rbs in glassPiecesRbs)
            {
                rbs.AddForce(new Vector3(Random.Range(-1,1),Random.Range(-1,1),Random.Range(-1,1))* glassPiecesPropelForce,ForceMode.Impulse);
                Destroy(rbs.gameObject, 2f);
            }
        }
    }
}
