using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem.Haptics;

public class TransparentWallBehaviour : MonoBehaviour
{
    MeshRenderer meshRenderer;
    float timerUntilDesapear;
    bool hide = false;

    float fadeSpeed = 5;

    float pretendedOpacity = 1;
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
        if(opacity==pretendedOpacity)return;
        meshRenderer.material.SetFloat("_Opacity",Mathf.Lerp(opacity,pretendedOpacity, fadeSpeed*Time.deltaTime));
    }
}
