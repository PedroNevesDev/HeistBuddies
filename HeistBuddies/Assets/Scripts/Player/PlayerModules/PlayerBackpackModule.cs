using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerBackpackModule : MonoBehaviour
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
            item.gameObject.SetActive(false);
        }
    }

    public int ClearItemsFromBackpack()
    {
        int totalPoints = items.Sum(item => item.Data.Heuries);
        UIManager uIManager = UIManager.Instance;
        foreach(Item i in items)
        {
            uIManager.uiItemDictionary.TryGetValue(i.Data,out ItemToPickupUI itemUI);
            itemUI.CheckRightMark();
        }

        items.Clear();

        return totalPoints;
    }
}
