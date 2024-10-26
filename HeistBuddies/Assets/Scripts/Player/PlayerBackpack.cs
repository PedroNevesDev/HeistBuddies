using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBackpack : MonoBehaviour
{
    [Header("Backpack Settings")]
    [SerializeField] private List<Item> items = new List<Item>();

    public void AddItemToBackPack(Item item)
    {
        bool itemExists = items.Any(existingItem => existingItem.ItemData.id != item.ItemData.id);

        if (!itemExists)
        {
            items.Add(item);
            Debug.Log($"{item.ItemData.name} with the ID {item.ItemData.id} was added to the backpack");
        }
    }

    public int ClearItemsFromBackpack()
    {
        int totalPoints = items.Sum(item => item.ItemData.points);

        items.Clear();

        return totalPoints;
    }
}
