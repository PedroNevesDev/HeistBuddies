using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LockPicking : MonoBehaviour
{
    public RectTransform movingMarker;       // The moving marker
    public GameObject targetPrefab;          // Prefab for the targets
    public RectTransform bar;                // The skill bar (parent RectTransform)

    [Header("Lockpicking Settings")]
    public int numberOfTargets = 5;          // Number of targets to generate
    public float speed = 200f;               // Speed of the moving marker
    private bool isGoingRight = true;        // Direction of the marker's movement
    public bool stupify = false;             // Makes lockpicking easier, making so everything doesnt reset
    public bool useOrder = true;             // Takes in consideration pin order

    private List<RectTransform> targets;     // List of target RectTransforms
    private HashSet<RectTransform> hitTargets; // Tracks which targets have been hit
    private Action onSuccess;
    int currentPin = 0;

    public Action OnSuccess { get => onSuccess; set => onSuccess = value; }

    

    void OnEnable()
    {
        // Initialize targets and hits tracking
        targets = new List<RectTransform>();
        hitTargets = new HashSet<RectTransform>();

        // Clear old targets if any
        foreach (Transform child in bar)
        {
            Destroy(child.gameObject);
        }

        // Generate new targets within the bar's range
        float barWidth = bar.rect.width;
        for (int i = 0; i < numberOfTargets; i++)
        {
            RectTransform newTarget = Instantiate(targetPrefab, bar).GetComponent<RectTransform>();
            Vector2 position;

            // Ensure no overlap
            do
            {
                float x = UnityEngine.Random.Range(-barWidth / 2, barWidth / 2); // Range for X based on the bar's width
                position = new Vector2(x, 0f);                      // Assuming fixed Y position
            } while (IsOverlappingWithExisting(position, newTarget.sizeDelta.x));

            newTarget.anchoredPosition = position;
            targets.Add(newTarget);
            newTarget.GetComponent<LockpickPin>().PinOrderIndex = i;
        }
    }

    void Update()
    {
        // Move the marker
        float step = speed * Time.deltaTime * (isGoingRight ? 1 : -1);
        movingMarker.anchoredPosition += new Vector2(step, 0);

        // Reverse direction and clamp at the edges of the bar
        float barWidth = bar.rect.width;
        float halfMarkerWidth = movingMarker.sizeDelta.x / 2;

        if (movingMarker.anchoredPosition.x > (barWidth / 2) - halfMarkerWidth)
        {
            movingMarker.anchoredPosition = new Vector2((barWidth / 2) - halfMarkerWidth, movingMarker.anchoredPosition.y);
            isGoingRight = false; // Reverse direction
        }
        else if (movingMarker.anchoredPosition.x < -(barWidth / 2) + halfMarkerWidth)
        {
            movingMarker.anchoredPosition = new Vector2(-(barWidth / 2) + halfMarkerWidth, movingMarker.anchoredPosition.y);
            isGoingRight = true; // Reverse direction
        }

        // Check for interaction
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckForOverlap();
        }
    }

    public void CheckForOverlap()
    {
        RectTransform closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (var target in targets)
        {
            if (!hitTargets.Contains(target) && RectOverlaps(movingMarker, target))
            {
                float distance = Mathf.Abs(movingMarker.anchoredPosition.x - target.anchoredPosition.x);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = target;
                }
            }
        }

        if (closestTarget != null)
        {
                LockpickPin lockpickPin = closestTarget.GetComponent<LockpickPin>();
                bool condition = useOrder?lockpickPin.PinOrderIndex == currentPin:true;
                if(condition)
                {
                    hitTargets.Add(closestTarget);
                    closestTarget.GetComponent<LockpickPin>().UnlockPin(true);
                    currentPin++;
                }
                else
                {
                    ResetPins();
                }
        }
        else
        {
            ResetPins();
        }

        // Check if all targets are hit
        if (hitTargets.Count == targets.Count)
        {
            onSuccess.Invoke();


            gameObject.SetActive(false);
        }
    }

    void ResetPins()
    {
        if(stupify)return;

        foreach(RectTransform rt in hitTargets)
        {
            rt.GetComponent<LockpickPin>().UnlockPin(false);
        }
        currentPin=0;
        hitTargets.Clear();
    }


    bool RectOverlaps(RectTransform rect1, RectTransform rect2)
    {
        // Use anchored positions for overlap check
        float halfWidth1 = rect1.sizeDelta.x / 2;
        float halfWidth2 = rect2.sizeDelta.x / 2;

        float rect1MinX = rect1.anchoredPosition.x - halfWidth1;
        float rect1MaxX = rect1.anchoredPosition.x + halfWidth1;

        float rect2MinX = rect2.anchoredPosition.x - halfWidth2;
        float rect2MaxX = rect2.anchoredPosition.x + halfWidth2;

        return rect1MinX < rect2MaxX && rect1MaxX > rect2MinX;
    }

    bool IsOverlappingWithExisting(Vector2 position, float width)
    {
        foreach (var target in targets)
        {
            float distance = Mathf.Abs(target.anchoredPosition.x - position.x);
            if (distance < width) // Adjust for size
                return true;
        }
        return false;
    }
}
