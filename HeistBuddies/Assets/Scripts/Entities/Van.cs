using UnityEngine;
using TMPro;
using System.Collections;
public class Van : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] AudioClip doorSlamSoundThatDoesntSoundLikeADoorSlam;
    float score;
    UIManager uiManager;
    AudioManager audioManager;
    
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward*300,ForceMode.Impulse);
        gameManager = GameManager.Instance;
        uiManager = UIManager.Instance;
        audioManager = AudioManager.Instance;
        audioManager.PlaySoundEffect(doorSlamSoundThatDoesntSoundLikeADoorSlam);
    }
    void OnTriggerEnter(Collider other) 
    {
        BodyPartOwner playerBodyPart = other.GetComponent<BodyPartOwner>();
        if(playerBodyPart)
        {
            rb.isKinematic = true;
            PlayerBackpackModule backpackModule = playerBodyPart.MyOwner.PlayerModules.BackpackModule;
            if(backpackModule!=null)
            {
                gameManager.AddScore(backpackModule.ClearItemsFromBackpack());
                backpackModule.ClearWeight();
            }
        }
        Item item = other.GetComponent<Item>();
        if(item)
            rb.isKinematic = true;
        if(item&&item.Data.isCollectable)
        {
            uiManager.uiItemDictionary.TryGetValue(item.Data,out ItemToPickupUI itemUI);
            itemUI.CheckRightMark();

            gameManager.AddScore(item.Data.Heuries);
            Destroy(item.gameObject);
        }
    }
}
