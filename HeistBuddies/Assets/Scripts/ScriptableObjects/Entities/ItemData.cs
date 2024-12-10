using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Item/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("General Settings")]
    public string Name;
    public int Heuries;
    public GameObject Particle;

    [Header("Item Conditions")]
    public bool isBreakable = false;
    public float breakingSpeed = 1;
    public AudioClip breakingSound;
    public bool isThrowable = false;
    public bool isStorable = false;
    public float weight;
    public bool isCollectable = true;

    [Header("Item Logo")]
    public Sprite Image;

    public PositionEvent positionEvent;
}
