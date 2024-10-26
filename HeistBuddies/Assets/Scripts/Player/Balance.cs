using UnityEngine;

public class Balance : MonoBehaviour
{
    public GameObject body;
    [SerializeField]bool  balance;

    // Update is called once per frame
    void Update()
    {
        if(balance)
        {
            transform.position=body.transform.position;
            transform.rotation=body.transform.rotation;
        }

    }
}
