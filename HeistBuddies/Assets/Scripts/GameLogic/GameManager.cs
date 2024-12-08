using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    UIManager uiManager;

    int currentScore = 0;

    int levelTotalScore = 0;

    [Header("Game Items")]
    public List<ItemData> Items = new List<ItemData>();

    void Start()
    {
        uiManager = UIManager.Instance;
        foreach(ItemData item in GetItems())
        {
            levelTotalScore +=item.Heuries;
        }
    }

    public void AddScore(int valueToAdd)
    {
        currentScore+=valueToAdd;
        uiManager.UpdateCurrency(valueToAdd);
    }

    public List<ItemData> GetItems()
    {
        return Items;
    }
}
