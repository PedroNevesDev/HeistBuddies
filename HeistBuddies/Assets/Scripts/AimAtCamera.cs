using UnityEngine;

public class AimAtCamera : MonoBehaviour
{
    Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(-cam.transform.position);
    }
}
