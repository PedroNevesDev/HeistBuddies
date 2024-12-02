using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Timer Text")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Item List")]
    [SerializeField] private GameObject ItemHolder;
    [SerializeField] private GameObject ItemListPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateItemList();
    }

    public void UpdateTimer(float currentTime)
    {
        int hours = Mathf.FloorToInt(currentTime);
        int minutes = Mathf.FloorToInt((currentTime - hours) * 60f);

        string formattedTime = string.Format("{0:00}:{1:00}", hours, minutes);
        timerText.text = formattedTime;
    }

    public void UpdateItemList()
    {
        foreach (Transform child in ItemHolder.transform)
        {
            Destroy(child.gameObject);
        }

        var items = GameManager.Instance.GetItems();
        foreach (var itemData in items)
        {
            GameObject newItem = Instantiate(ItemListPrefab, ItemHolder.transform);
            var itemUI = newItem.GetComponent<ItemToPickupUI>();
            itemUI.Populate(itemData.Image, itemData.Name, itemData.Points);
        }
    }
}
