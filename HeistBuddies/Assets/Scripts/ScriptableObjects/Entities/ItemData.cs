using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Item/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("General Settings")]
    public string Name;
    public int Points;

    [Header("Item Conditions")]
    public bool isBreakable = false;
    public bool isThrowable = false;
    public bool isStorable = false;
}
