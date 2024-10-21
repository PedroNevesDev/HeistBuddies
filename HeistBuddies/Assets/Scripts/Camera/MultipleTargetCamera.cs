using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultipleTargetCamera : MonoBehaviour
{
    // [Tooltip("This is a testing dependency. Should be removed and a normal array or list should be used instead")]
    // public ControllerListener controllerListener;

    // [Header("Moving Parameters")]
    // public Vector3 offset;
    // private Vector3 velocity;
    // public float smoothTime = 0.5f;

    // [Header("Zooming Parameters")]
    // public float minZoom = 40f;    // Maximum zoom-out level
    // public float maxZoom = 10f;    // Maximum zoom-in level
    // public float zoomLimiter = 50f;

    // private Camera cam;
    // private float cameraAngleY;  // Angle of the camera relative to the horizontal plane (for isometric view)

    // void Start()
    // {
    //     cam = GetComponent<Camera>();
    //     cameraAngleY = cam.transform.eulerAngles.x; // Store the camera's Y-axis tilt
    // }

    // // Update is called once per frame
    // void LateUpdate()
    // {
    //     if (controllerListener.playerInstances.Count == 0)
    //         return;

    //     Move();
    //     Zoom();
    // }

    // void Zoom()
    // {
    //     Bounds bounds = GetTargetBounds();

    //     // Calculate required zoom based on bounds
    //     float width = bounds.size.x;
    //     float height = bounds.size.y;

    //     // Adjust Y axis based on the camera's tilt (using trigonometry to get the Y coverage)
    //     float verticalZoom = height / Mathf.Tan(cameraAngleY * Mathf.Deg2Rad);
        
    //     // Ensure camera zooms enough for both X and Y axes
    //     float horizontalZoom = width / cam.aspect;

    //     // Take the largest required zoom to fit both dimensions
    //     float greatestZoom = Mathf.Max(horizontalZoom, verticalZoom) / zoomLimiter;

    //     // Smoothly interpolate between the current field of view and the new zoom level
    //     float newZoom = Mathf.Lerp(maxZoom, minZoom, greatestZoom);
    //     cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    // }

    // Bounds GetTargetBounds()
    // {
    //     int playerNum = controllerListener.playerInstances.Count;
    //     var bounds = new Bounds(controllerListener.playerInstances[0].transform.position, Vector3.zero);
        
    //     for (int i = 0; i < playerNum; i++)
    //     {
    //         bounds.Encapsulate(controllerListener.playerInstances[i].transform.position);
    //     }

    //     return bounds;
    // }

    // void Move()
    // {
    //     Vector3 centerPoint = GetCenterPoint();

    //     Vector3 newPosition = centerPoint + offset;

    //     transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    // }

    // Vector3 GetCenterPoint()
    // {
    //     int playerNum = controllerListener.playerInstances.Count;
    //     if (playerNum == 1)
    //     {
    //         return controllerListener.playerInstances[0].transform.position;
    //     }

    //     Bounds bounds = new Bounds(controllerListener.playerInstances[0].transform.position, Vector3.zero);
    //     for (int i = 0; i < playerNum; i++)
    //     {
    //         bounds.Encapsulate(controllerListener.playerInstances[i].transform.position);
    //     }

    //     return bounds.center;
    // }
}
