using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Items")]
    public List<ItemData> Items = new List<ItemData>();

    private void Awake()
    {
        Instance = this;
    }

    public List<ItemData> GetItems()
    {
        return Items;
    }
}
