using UnityEngine;
using TMPro;
using System;

public class Item : MonoBehaviour, IGrabbable
{
    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    [Header("Item UI")]
    [SerializeField] private TextMeshProUGUI itemText;

    public Guid Id { get; private set; }
    public ItemData Data { get => itemData; private set => itemData = value; }

    private void Awake()
    {
        Id = Guid.NewGuid();
    }

    public void Grab()
    {
        this.gameObject.SetActive(false);
    }

    public void EnableUI()
    {
        itemText.text = "GRAB";
    }

    public void DisableUI()
    {
        itemText.text = "";
    }

}
