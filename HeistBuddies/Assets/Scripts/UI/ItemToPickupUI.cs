using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemToPickupUI : MonoBehaviour
{
    public Image Icon;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Points;

    public void Populate(Sprite icon, string name, int points)
    {
        Icon.sprite = icon;
        Name.text = name;
        Points.text = $"+ {points}";
    }
}
