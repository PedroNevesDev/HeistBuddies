using UnityEngine;
using TMPro;
using System;

public enum ItemState
{
    Idle,
    Grabbed,
    Throwing
}

public class Item : MonoBehaviour, IGrabbable, IThrowable
{
    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    [Header("Item UI")]
    [SerializeField] private TextMeshProUGUI itemText;

    [Header("Item Conditions")]
    [SerializeField] private bool isBreakable;
    [SerializeField] private bool isThrowable;
    [SerializeField] private bool isStorable;

    [Header("Item State")]
    [SerializeField] private ItemState state = ItemState.Idle;

    private Rigidbody rb;

    public Guid Id { get; private set; }
    public ItemData Data { get => itemData; private set => itemData = value; }
    public ItemState State { get => state; set => state = value; }

    private void Awake()
    {
        Id = Guid.NewGuid();

        rb = GetComponent<Rigidbody>();

        if (itemData == null)
        {
            Debug.Log("No ItemData assigned!");
            return;
        }

        isBreakable = itemData.isBreakable;
        isThrowable = itemData.isThrowable;
        isStorable = itemData.isStorable;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (itemData.isBreakable)
        {
            
        }
    }

    #region IGrabbable

    public void Grab(Transform target)
    {
        state = ItemState.Grabbed;

        rb.isKinematic = true;

        this.transform.SetParent(target);
        this.transform.position = target.position;
    }

    public void Release()
    {
        state = ItemState.Idle;
        transform.SetParent(null);
    }

    public void EnableUI()
    {
        itemText.text = "GRAB";
    }

    public void DisableUI()
    {
        itemText.text = "";
    }

    public void Throw(Vector3 direction)
    {
        state = ItemState.Throwing;
        rb.isKinematic = false;
        Release();
        rb.AddForce(direction, ForceMode.Impulse);
    }

    #endregion

    #region IThrowable



    #endregion
}
