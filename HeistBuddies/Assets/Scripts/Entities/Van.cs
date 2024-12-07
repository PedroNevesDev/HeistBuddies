using UnityEngine;
using TMPro;
using System.Collections;
public class Van : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;

    [SerializeField] AudioClip doorSlamSoundThatDoesntSoundLikeADoorSlam;
    float score;
    UIManager uiManager;
    AudioManager audioManager;
    void Start()
    {
        uiManager = UIManager.Instance;
        audioManager = AudioManager.Instance;
        audioManager.PlaySoundEffect(doorSlamSoundThatDoesntSoundLikeADoorSlam);
    }
    void OnTriggerEnter(Collider other) 
    {
        BodyPartOwner playerBodyPart = other.GetComponent<BodyPartOwner>();
        if(playerBodyPart)
        {
            PlayerBackpackModule backpackModule = playerBodyPart.MyOwner.PlayerModules.BackpackModule;
            if(backpackModule!=null)
            {
                AddScore(backpackModule.ClearItemsFromBackpack());
            }
        }
        Item item = other.GetComponent<Item>();
        if(item&&item.Data.isCollectable)
        {
            
            uiManager.uiItemDictionary.TryGetValue(item.Data,out ItemToPickupUI itemUI);
            itemUI.CheckRightMark();

            AddScore(item.Data.Points);
            Destroy(item.gameObject);
        }
    }

    void AddScore(int total)
    {

        if(total>0)
        {
            score += total;
            scoreText.text = score.ToString();
            StartCoroutine(TextPreFadeDelay());
            audioManager.PlaySoundEffect(doorSlamSoundThatDoesntSoundLikeADoorSlam);
        }
    }

    IEnumerator TextPreFadeDelay()
    {
        scoreText.enabled = true;
        yield return new WaitForSeconds(1);
        scoreText.enabled = false;
    }
}
