using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Item/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("General Settings")]
    public string Name;
    public int Points;

    [Header("Breaking Settings")]
    public bool isBreakable = false;
    [Range(0, 1)] public float breakChance;
}
