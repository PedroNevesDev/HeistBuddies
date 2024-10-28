using UnityEngine;

public class BodyPartOwner : MonoBehaviour
{
    /* Summary: Created to be able to access body part player controller through colision detection for example*/
    PlayerController myOwner;

    public PlayerController MyOwner { get => myOwner; set => myOwner = value; }
}