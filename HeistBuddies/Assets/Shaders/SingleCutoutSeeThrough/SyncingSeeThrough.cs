using System;
using UnityEngine;

public class SyncingSeeThrough : MonoBehaviour
{
    static string pos = "_CutoutPosition";
    static string size = "_CutoutSize";
    static string direction = "_CutoutDirection";

    private Camera cam;
    public LayerMask mask;
    public float maxSize = 1f;
    public float fadeInSpeed = 1f; // Adjust this for speed

    private MaterialPropertyBlock propBlock;
    private Renderer wallRenderer;
    private float currentSize = 0f; // Start with size 0
    public int playerID = 1; // 1 for first player, 2 for second player

    private Vector3 worldPos;
    private Vector3 dir;

    void Awake()
    {
        if (FindObjectsByType<SyncingSeeThrough>(FindObjectsSortMode.None).Length == 2)
        {
            playerID = 2;
        }
    }

    void Start()
    {
        cam = Camera.main;
        propBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        dir = cam.transform.position - transform.position;
        var ray = new Ray(transform.position, dir.normalized);

        if (Physics.Raycast(ray, out RaycastHit hit, Vector3.Distance(cam.transform.position,transform.position), mask))
        {
            worldPos = hit.point; // Use the hit point as the cutout position
            currentSize = Mathf.Lerp(currentSize, maxSize, fadeInSpeed * Time.deltaTime); // Fade in
            wallRenderer = hit.collider.GetComponent<MeshRenderer>();
        }
        else
        {
            // Fade out, ensuring we can access wallRenderer
            if (wallRenderer != null)
            {
                currentSize = Mathf.Lerp(currentSize, 0, fadeInSpeed * Time.deltaTime);
            }
        }
    }

    void FixedUpdate()
    {
        if (wallRenderer != null)
        {
            wallRenderer.GetPropertyBlock(propBlock);

            // Set the cutout size
            propBlock.SetFloat(Shader.PropertyToID(size + playerID), currentSize);
            propBlock.SetVector(Shader.PropertyToID(pos + playerID), new Vector4(worldPos.x, worldPos.y, worldPos.z, 1));
            propBlock.SetVector(Shader.PropertyToID(direction + playerID), new Vector4(dir.x, dir.y, dir.z, 1));

            wallRenderer.SetPropertyBlock(propBlock);
        }
    }
}
