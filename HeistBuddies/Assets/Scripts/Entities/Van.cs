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
                int totalItemScore = backpackModule.ClearItemsFromBackpack();

                if(totalItemScore>0)
                {
                    score += totalItemScore;
                    scoreText.text = score.ToString();
                    StartCoroutine(TextPreFadeDelay());
                }
            }
        }
    }

    IEnumerator TextPreFadeDelay()
    {
        scoreText.enabled = true;
        yield return new WaitForSeconds(1);
        scoreText.enabled = false;
    }
}
