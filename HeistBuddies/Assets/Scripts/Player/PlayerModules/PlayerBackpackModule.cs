using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerBackpackModule : MonoBehaviour
{
    [Header("Backpack Settings")]
    float currentWeight;
    [SerializeField] float maxWeightValue = 50;
    [SerializeField] private List<Item> items = new List<Item>();
    UIManager uiManager;

    PlayerController playerController;

    public string GetPlayerName()=>playerController.SkinnedMeshRenderer.sharedMaterial.name;
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        uiManager = UIManager.Instance;
    }
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


    public bool CheckIfItemCanBeAdded(Item item)
    {
        if(!item)return false;
        return item.Data.weight+currentWeight<=maxWeightValue;
    }

    public void AddItemWeight(Item item)
    {
        float itemWeight = item.Data.weight;

        currentWeight+=itemWeight;
        UpdateUI();
    }
    public void RemoveItemWeight(Item item)
    {
        float itemWeight = item.Data.weight;
        currentWeight-=itemWeight;
        UpdateUI();
    }

    void UpdateUI()
    {
        uiManager.UpdateWeight(currentWeight/maxWeightValue,GetPlayerName());
    }

    public void ClearWeight()
    {
        currentWeight=0;
        UpdateUI();
    }

    public float GetWeightPercentage()
    {
        return currentWeight/maxWeightValue;
    }
}

