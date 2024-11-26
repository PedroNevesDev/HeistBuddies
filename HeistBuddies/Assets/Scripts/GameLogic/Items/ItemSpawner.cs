using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Position")]
    [SerializeField] private Transform position;

    [Header("Possible Items")]
    [SerializeField] private GameObject[] items = new GameObject[0];
    private GameObject itemToSpawn = null;

    public Transform Position { get => position; set => position = value; }
    public GameObject ItemToSpawn { get => itemToSpawn; set => itemToSpawn = value; }

    public void Initialize()
    {
        if (position == null)
        {
            position = this.transform;
        }

        var rnd = Random.Range(0, items.Length);
        var item = items[rnd];

        if (item != null)
        {
            itemToSpawn = item;
        }
    }
}
