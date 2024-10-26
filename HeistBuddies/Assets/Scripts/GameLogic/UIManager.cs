using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Timer Text")]
    [SerializeField] private TextMeshProUGUI timerText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateTimer(float currentTime)
    {
        int hours = Mathf.FloorToInt(currentTime);
        int minutes = Mathf.FloorToInt((currentTime - hours) * 60f);

        string formattedTime = string.Format("{0:00}:{1:00}", hours, minutes);
        timerText.text = formattedTime;
    }
}
