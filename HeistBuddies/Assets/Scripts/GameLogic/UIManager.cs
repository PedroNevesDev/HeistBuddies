using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Timer Text")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Item List")]
    [SerializeField] private GameObject ItemHolder;
    [SerializeField] private GameObject ItemListPrefab;
    public Dictionary<ItemData,ItemToPickupUI> uiItemDictionary = new Dictionary<ItemData, ItemToPickupUI>();
    [SerializeField] private CanvasGroup itemListForFade;
    [SerializeField] float itemListFadeInSpeed;
    [SerializeField] float itemListFadeOutSpeed;
    float desiredAlpha = 0;
    float timer;
    [SerializeField] float timeUntilFadeOut;
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateItemList();
        ShowList();
    }
    void Update()
    {
        UpdateListAlpha();
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
            itemUI.Populate(itemData.Name, itemData.Points);
            uiItemDictionary.Add(itemData,itemUI);
        }
    }

    void UpdateListAlpha()
    {
        itemListForFade.alpha = Mathf.Lerp(itemListForFade.alpha,desiredAlpha,(desiredAlpha==1?itemListFadeInSpeed:itemListFadeOutSpeed )*Time.deltaTime);
        timer-=Time.deltaTime;
        if(timer<=0)
            desiredAlpha = 0;
    }

    public void ShowList()
    {
        desiredAlpha = 1;
        timer =timeUntilFadeOut;
    }
}
