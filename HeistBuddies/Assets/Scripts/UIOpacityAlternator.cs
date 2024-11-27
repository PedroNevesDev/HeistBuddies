using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.UI;

public class UIOpacityAlternator : MonoBehaviour
{
    
    [SerializeField] float maxOpacity;
    [SerializeField] float minOpacity;
    [SerializeField] float fadeSpeed;
    [SerializeField] List<Image> images = new List<Image>();
    
    float currentOpacity;
    Camera cam;
    Vector3 dir = Vector3.zero;

    Color myColor;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentOpacity = maxOpacity;
        cam = Camera.main;
    }

    void Update()
    {
        dir = cam.transform.position - transform.position;
        var ray = new Ray(transform.position, dir.normalized);

        if (Physics.Raycast(ray, Vector3.Distance(cam.transform.position,transform.position)))
        {
            currentOpacity = minOpacity;
        }
        else
        {
            currentOpacity = maxOpacity;
        }

        foreach(Image img in images)
        {
            myColor=img.color;
            myColor.a = Mathf.Lerp(myColor.a,currentOpacity,fadeSpeed*Time.deltaTime);
            img.color = myColor;
        }
    }
    void OnDrawGizmos() 
    {
         var ray = new Ray(transform.position, dir.normalized);
        Color color = Color.red;
        Gizmos.DrawRay(ray);
    }
}
