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

    private void OnCollisionEnter(Collision collision)
    {
        if (itemData.isBreakable)
        {
            float randomValue = UnityEngine.Random.value;
            if (randomValue <= itemData.breakChance)
            {
                var eventData = new PositionEventData(null, this.transform.position);
                EventManager.InvokeGlobalEvent(GlobalEvent.SoundAlert, eventData);
                Destroy(this.gameObject);
            }
        }
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
