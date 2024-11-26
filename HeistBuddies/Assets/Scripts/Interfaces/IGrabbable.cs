using UnityEngine;

public interface IGrabbable
{
    void Grab(Transform target);
    void Release();
}
