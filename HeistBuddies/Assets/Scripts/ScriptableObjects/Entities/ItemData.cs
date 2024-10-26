using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Item/Item Data")]
public class ItemData : ScriptableObject
{
    public string id = Guid.NewGuid().ToString();
    public string name;
    public int points;
}
