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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (itemData.isBreakable)
        {
            float colisionSpeed = rb.angularVelocity.magnitude;
            if(colisionSpeed>= itemData.breakingSpeed)
            {
                UIManager uiManager = UIManager.Instance;
                uiManager.uiItemDictionary.TryGetValue(itemData,out ItemToPickupUI itemUI);
                itemUI?.CheckWrongMark();
                AudioManager.Instance.PlaySoundEffect(itemData.breakingSound);
                print(this.name + " broke at " + colisionSpeed + "km/h");
                Destroy(gameObject);
            }
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


    public void Throw(Vector3 direction)
    {
        rb.isKinematic = false;
        Release();
        rb.AddForce(direction, ForceMode.Impulse);
    }

    #endregion

    #region IThrowable



    #endregion
}
