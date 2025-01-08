using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemToPickupUI : MonoBehaviour
{
    public Image rightCheckMark;
    public Image wrongCheckMark;
    public TextMeshProUGUI Name;

    public bool isCollected = false; 

    public void Populate( string name, int points)
    {

        Name.text = name+ " <color=yellow>+" +points+"H</color>";
    }

    public void CheckWrongMark()
    {
        UIManager.Instance.ShowList();
        wrongCheckMark.gameObject.SetActive(true);
    }
    public void CheckRightMark()
    {
        UIManager.Instance.ShowList();
        rightCheckMark.gameObject.SetActive(true);
        isCollected = true;
    }

}
