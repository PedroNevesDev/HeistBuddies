using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem.Haptics;

public class TransparentWallBehaviour : MonoBehaviour
{
    MeshRenderer meshRenderer;
    float fadeSpeed = 5;

    float min = 0.15f;
    float max = 1f;

    float pretendedOpacity = 1;

    List<TransparentWallDetection> listOfDetectorsBehindWalls = new List<TransparentWallDetection>();

    public List<TransparentWallDetection> ListOfDetectorsBehindWalls { get => listOfDetectorsBehindWalls; set => listOfDetectorsBehindWalls = value; }

    public void ToggleHide(bool status,float min,float max)=> pretendedOpacity = status==true?min:max;
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer=GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float opacity = meshRenderer.material.GetFloat("_Opacity");
        pretendedOpacity = listOfDetectorsBehindWalls.Count==0?max:min;
        if(opacity==pretendedOpacity)return;
        meshRenderer.material.SetFloat("_Opacity",Mathf.Lerp(opacity,pretendedOpacity, fadeSpeed*Time.deltaTime));
    }
}
