using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class UIManager : Singleton<UIManager>
{
    [Header("Timer Text")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Item List")]
    [SerializeField] private GameObject ItemHolder;
    [SerializeField] private GameObject ItemListPrefab;
    public Dictionary<ItemData,ItemToPickupUI> uiItemDictionary = new Dictionary<ItemData, ItemToPickupUI>();
    [SerializeField] private CanvasGroup itemListForFade;
    [SerializeField] private float itemListFadeInSpeed;
    [SerializeField] private float itemListFadeOutSpeed;
    float desiredAlpha = 0;
    float timer;
    [SerializeField] private float timeUntilFadeOut;

    [Header("Weight Settings")]

    [SerializeField] private Image lockWeightFillImage;
    [SerializeField] private Image pickWeightFillImage;
    [SerializeField] private float weightUpdateSpeed;
    [SerializeField] private Gradient weightColors;

    [Header("Currency")]
    [SerializeField] private TextMeshProUGUI currencyText;

    float lockWeightTarget = 0f;
    float pickWeightTarget = 0f;
    
    private void Start()
    {
        UpdateItemList();
        ShowList();
    }
    void Update()
    {

        if(lockWeightFillImage.fillAmount!=lockWeightTarget)
        {
            lockWeightFillImage.fillAmount = Mathf.Lerp(lockWeightFillImage.fillAmount,lockWeightTarget,weightUpdateSpeed*Time.deltaTime);
            lockWeightFillImage.color = weightColors.Evaluate(lockWeightFillImage.fillAmount);
        }

        if(pickWeightFillImage.fillAmount!=pickWeightTarget)
        {
            pickWeightFillImage.fillAmount = Mathf.Lerp(pickWeightFillImage.fillAmount,pickWeightTarget,weightUpdateSpeed*Time.deltaTime);
            pickWeightFillImage.color = weightColors.Evaluate(pickWeightFillImage.fillAmount);
        }

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
            itemUI.Populate(itemData.Name, itemData.Heuries);
            uiItemDictionary.Add(itemData,itemUI);
        }
    }
    public void UpdateCurrency(int value)
    {
        currencyText.text = value.ToString();
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

    public void UpdateWeight(float fillAmount, string characterName)
    {
        if(characterName.Contains("Lock"))
        {
            lockWeightTarget = fillAmount;
        }
        else
        {
            pickWeightTarget = fillAmount;
        }
    }
}
