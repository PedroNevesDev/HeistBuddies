using UnityEngine;

public class Item : MonoBehaviour, IGrabbable
{
    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    public ItemData ItemData { get => itemData; set => itemData = value; }

    public void Grab()
    {
        
    }
}
