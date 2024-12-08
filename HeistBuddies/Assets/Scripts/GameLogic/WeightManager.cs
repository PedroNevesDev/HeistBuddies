using UnityEngine;

public class WeightManager : Singleton<WeightManager>
{
    float currentWeightLock;
    float currentWeightPick;
    [SerializeField] float maxWeightValue = 50;

    UIManager uiManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiManager = UIManager.Instance;
    }

    public bool CheckIfItemCanBeAdded(Item item,string characterName)
    {
        float weight = characterName.Contains("Lock")?currentWeightLock:currentWeightPick;
        return item.Data.weight+weight<=maxWeightValue;
    }

    public void AddItemWeight(Item item,string characterName)
    {
        float itemWeight = item.Data.weight;
        bool isPlayerLock = characterName.Contains("Lock");

        if(isPlayerLock)
        {
            currentWeightLock+=itemWeight;
            uiManager.UpdateWeight(currentWeightLock/maxWeightValue,isPlayerLock);
        }
        else
        {
            currentWeightPick+=itemWeight;
            uiManager.UpdateWeight(currentWeightPick/maxWeightValue,isPlayerLock);
        }
    }

    public void ClearWeight(string characterName)
    {
        bool isPlayerLock = characterName.Contains("Lock");

        if(isPlayerLock)
        {
            currentWeightLock=0;
            uiManager.UpdateWeight(currentWeightLock/maxWeightValue,isPlayerLock);
        }
        else
        {
            currentWeightPick=0;
            uiManager.UpdateWeight(currentWeightPick/maxWeightValue,isPlayerLock);
        }
    }
}
