using UnityEngine;

public class LockpickPin : MonoBehaviour
{
    float speed = 8f;
    bool unlock;

    int pinOrderIndex;

    public void UnlockPin(bool status) => unlock = status;

    float offset = 0.4f; // Adjust to suit UI units
    Vector2 startPos;

    RectTransform rectTransform;

    public int PinOrderIndex { get => pinOrderIndex; set => pinOrderIndex = value; }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition; // Use anchoredPosition
    }

    void Update()
    {
        Vector2 targetPos = unlock ?startPos + Vector2.up * offset:startPos;

        if (rectTransform.anchoredPosition != targetPos)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPos, speed * Time.deltaTime);
        }
    }
}
