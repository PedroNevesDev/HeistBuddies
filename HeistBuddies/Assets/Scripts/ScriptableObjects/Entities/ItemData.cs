using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Item/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("General Settings")]
    public string Name;
    public int Points;

    [Header("Item Conditions")]
    public bool isBreakable = false;
    public float breakingSpeed = 1;
    public AudioClip breakingSound;
    public bool isThrowable = false;
    public bool isStorable = false;
    public float weight;
    public bool isCollectable = false;

    [Header("Item Logo")]
    public Sprite Image;
}
