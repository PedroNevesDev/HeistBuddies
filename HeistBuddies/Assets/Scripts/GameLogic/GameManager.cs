using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)||Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void AddScore(int valueToAdd)
    {
        currentScore+=valueToAdd;
        uiManager.UpdateCurrency(currentScore);
    }

    public List<ItemData> GetItems()
    {
        return Items;
    }
}
