using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    [Header("Items To Spawn")]
    [SerializeField] private List<ItemSpawner> items = new List<ItemSpawner>();

    private void Start()
    {
        for (int i = 0; i < items.Count; i++) 
        {
            items[i].Initialize();
            SpawnItem(items[i].ItemToSpawn, items[i].Position);
        }
    }

    private void SpawnItem(GameObject item, Transform position)
    {
        Instantiate(item, position.position, Quaternion.identity);
    }
}
