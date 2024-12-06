using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class SyncingSeeThrough : MonoBehaviour
{
    private Camera cam;
    public LayerMask mask;
    public float fadeSpeed = 1f; 

    List<RaycastHit> wallsToFadeIn = new List<RaycastHit>();
    List<RaycastHit> oldWalls = new List<RaycastHit>();
    List<RaycastHit> wallsToFadeOut = new List<RaycastHit>();

    private Vector3 dir;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        dir = cam.transform.position - transform.position;
        ProcessNewHits();
        ProcessOldHits();
    }

    void ProcessNewHits()
    {
        // Raycast to get all the walls within range that are now visible
        RaycastHit[] newHits = Physics.RaycastAll(transform.position, dir.normalized, 40, mask);

        // Add any new hits that weren't previously in oldWalls to wallsToFadeIn
        foreach (RaycastHit hit in newHits)
        {
            AlterHitMaterialOpacity(hit, 0.35f); // Fade in detected walls immediately
        }

        foreach (RaycastHit wall in oldWalls)
        {
            if (!newHits.Contains(wall)&&!wallsToFadeIn.Contains(wall))  // If the wall is no longer hit
            {
                wallsToFadeIn.Add(wall);  // Move it to the fade-out list
            }
        }
        oldWalls.Clear();
    }

    void ProcessOldHits()
    {
        // Fade in walls that are still being hit but were not marked to fade yet
        foreach (RaycastHit wall in wallsToFadeIn)
        {
            AlterHitMaterialOpacity(wall, 1f); // Fade in
        }

        oldWalls.AddRange(wallsToFadeOut); // Update oldWalls list with walls that are fading out
    }

    void AlterHitMaterialOpacity(RaycastHit hit, float targetOpacity)
    {
        MeshRenderer mr = hit.collider.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            Material mat = mr.material;
            // Smoothly transition opacity toward targetOpacity
            float currentOpacity = mat.GetFloat("_Opacity");
            float newOpacity = Mathf.Lerp(currentOpacity, targetOpacity, fadeSpeed * Time.deltaTime);
            mat.SetFloat("_Opacity", newOpacity);
            mr.material = mat;
        }
    }
}
