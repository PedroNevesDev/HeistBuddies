using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    UIManager uiManager;

    int currentScore = 0;

    int levelTotalScore = 0;

    [Header("Game Items")]
    public List<ItemData> Items = new List<ItemData>();
    public int itemsGrabbed = 0;

    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1;

        uiManager = UIManager.Instance;
        foreach(ItemData item in GetItems())
        {
            levelTotalScore +=item.Heuries;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void AddScore(int valueToAdd)
    {
        currentScore+=valueToAdd;
        uiManager.UpdateCurrency(currentScore);
    }

    public void CheckForItems(ItemData data)
    {
        UIManager.Instance.uiItemDictionary.TryGetValue(data, out var item);
        if (item.isCollected) itemsGrabbed++;

        if (itemsGrabbed == Items.Count) 
        {
            Time.timeScale = 0;
            UIManager.Instance.ShowGameWinPanel();
        }
    }

    public List<ItemData> GetItems()
    {
        return Items;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
            UIManager.Instance.ShowPausePanel(true);
        }
        else
        {
            Time.timeScale = 1;
            UIManager.Instance.ShowPausePanel(false);
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        UIManager.Instance.ShowGameOverPanel();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
