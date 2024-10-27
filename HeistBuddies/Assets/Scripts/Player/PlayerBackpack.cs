using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBackpack : MonoBehaviour
{
    [Header("Backpack Settings")]
    [SerializeField] private List<Item> items = new List<Item>();

    public void AddItemToBackPack(Item item)
    {
        bool itemExists = items.Any(existingItem => existingItem.Id == item.Id);

        if (!itemExists)
        {
            items.Add(item);
            Debug.Log($"{item.Data.Name} with the ID {item.Id} was added to the backpack");
        }
    }

    public int ClearItemsFromBackpack()
    {
        int totalPoints = items.Sum(item => item.Data.Points);

        items.Clear();

        return totalPoints;
    }
}
