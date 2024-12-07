using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TransparentWallDetection : MonoBehaviour
{
    private Camera cam;
    public LayerMask mask;
    public float min = 0.35f;
    public float max = 1f;

    private HashSet<Collider> oldColliders = new HashSet<Collider>();

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        ProcessHits();
    }

    void ProcessHits()
    {
        Vector3 dir = cam.transform.position - transform.position;

        // Raycast to get all the walls within range that are now visible
        RaycastHit[] newHits = Physics.RaycastAll(transform.position, dir.normalized, 40, mask);

        // Extract colliders from the new hits
        HashSet<Collider> newColliders = new HashSet<Collider>(newHits.Select(hit => hit.collider));

        // Hide new colliders not in the old set
        foreach (Collider newCollider in newColliders)
        {
            if (!oldColliders.Contains(newCollider))
            {
                newCollider.GetComponent<TransparentWallBehaviour>()?.ListOfDetectorsBehindWalls.Add(this);
            }
        }

        // Unhide old colliders no longer in the new set
        foreach (Collider oldCollider in oldColliders)
        {
            if (!newColliders.Contains(oldCollider))
            {
                oldCollider.GetComponent<TransparentWallBehaviour>()?.ListOfDetectorsBehindWalls.Remove(this);
            }
        }

        // Update the old colliders set
        oldColliders = newColliders;
    }
}
