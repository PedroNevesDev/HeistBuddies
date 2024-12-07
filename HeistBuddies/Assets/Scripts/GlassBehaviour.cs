using System.Collections.Generic;
using UnityEngine;

public class GlassBehaviour : MonoBehaviour
{
    [SerializeField] GameObject glassPiecesObj;
    [SerializeField] float breakForce = 1;
    [SerializeField] float glassPiecesPropelForce = 1;
    [SerializeField] List<Rigidbody> glassPiecesRbs = new List<Rigidbody>();

    [SerializeField] AudioClip breakingSound;

    private void OnCollisionEnter(Collision other) 
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if(rb && rb.angularVelocity.magnitude>= breakForce)
        {
            gameObject.SetActive(false);
            glassPiecesObj.SetActive(true);
            AudioManager.Instance.PlaySoundEffect(breakingSound);
            foreach(Rigidbody rbs in glassPiecesRbs)
            {
                rbs.AddForce(new Vector3(Random.Range(-1,1),Random.Range(-1,1),Random.Range(-1,1))* glassPiecesPropelForce,ForceMode.Impulse);
            }
        }
    }
}
