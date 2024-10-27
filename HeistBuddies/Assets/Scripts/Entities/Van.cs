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
            int newScore = playerBodyPart.MyOwner.Backpack.ClearItemsFromBackpack();
            print(newScore);
            if(newScore>0)
            {
                score += newScore;
                scoreText.text = score.ToString();
                StartCoroutine(TextPreFadeDelay());
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
