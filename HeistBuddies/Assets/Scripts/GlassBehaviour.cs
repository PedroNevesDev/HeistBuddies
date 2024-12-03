using System.Collections.Generic;
using UnityEngine;

public class GlassBehaviour : MonoBehaviour
{
    [SerializeField] GameObject glassPiecesObj;
    [SerializeField] float breakForce = 1;
    [SerializeField] float glassPiecesPropelForce = 1;
    [SerializeField] List<Rigidbody> glassPiecesRbs = new List<Rigidbody>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision other) 
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if(rb && rb.angularVelocity.magnitude>= breakForce)
        {
            gameObject.SetActive(false);
            glassPiecesObj.SetActive(true);
            foreach(Rigidbody rbs in glassPiecesRbs)
            {
                rbs.AddForce(new Vector3(Random.Range(-1,1),Random.Range(-1,1),Random.Range(-1,1))* glassPiecesPropelForce,ForceMode.Impulse);
            }
        }
    }
}
