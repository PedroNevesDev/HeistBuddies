using UnityEngine;

public class SelectionPersistantData : Singleton<SelectionPersistantData>
{
    [SerializeField] int lockModel;
    [SerializeField] int pickModel;

    public int PickModel { get => pickModel; set => pickModel = value; }
    public int LockModel { get => lockModel; set => lockModel = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
