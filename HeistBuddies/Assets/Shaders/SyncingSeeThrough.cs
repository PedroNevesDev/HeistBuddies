using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SyncingSeeThrough : MonoBehaviour
{
    public static int posID= Shader.PropertyToID("_Position");
    public static int sizeID= Shader.PropertyToID("_Size");

    private Camera cam;
    public LayerMask mask;
    public float maxSize=1;
    public float fadeInSpeed;

    Material refMat;

    float lerpValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        var dir = cam.transform.position - transform.position;
        var ray = new Ray(transform.position,dir.normalized);
        if(Physics.Raycast(ray,out RaycastHit hit,3000,mask))
        {
            lerpValue=maxSize;
            var view = cam.WorldToViewportPoint(transform.position);
            refMat = hit.collider.GetComponent<MeshRenderer>().material;
            refMat.SetVector(posID, view);
            
        }
        else
        {
            lerpValue=0;
        }

    }
    void FixedUpdate()
    {
        if(refMat)
        {
        refMat.SetFloat(sizeID,Mathf.Lerp(refMat.GetFloat(sizeID),lerpValue,fadeInSpeed*Time.fixedDeltaTime));
        }

    }


}
