using UnityEngine;
using TMPro;
using System.Collections;
public class Van : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    float score;
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
        }
    }

    IEnumerator TextPreFadeDelay()
    {
        scoreText.enabled = true;
        yield return new WaitForSeconds(1);
        scoreText.enabled = false;
    }
}
