using UnityEngine;

public interface IGrabbable
{
    void Grab(Transform target,PlayerController player);
    void Release();
}
