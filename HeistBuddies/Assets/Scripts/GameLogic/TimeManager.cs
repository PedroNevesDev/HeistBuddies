using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Gradient lightColor;
    [SerializeField] private AnimationCurve lightIntensity;
    [SerializeField] private float maxIntensity = 1.5f;
    [SerializeField] private float minIntensity = 0.1f;

    [Header("Time Settings")]
    [SerializeField] private float currentTime = 0f;
    [Tooltip("This is in minutes!")]
    [SerializeField] private float cycleDuration = 10f;
    [SerializeField] private float startHour = 7f; // Start time is now larger
    [SerializeField] private float endHour = 0f;  // End time is smaller

    public float CurrentTime { get => currentTime; private set => currentTime = value; }

    private void Start()
    {
        currentTime = startHour; // Start at the larger hour
    }

    private void Update()
    {
        // Calculate total hours (negative for decreasing)
        float totalHours = endHour - startHour;
        float cycleSpeed = totalHours / (cycleDuration * 60f); // Speed adjusted for negative progress
        currentTime += cycleSpeed * Time.deltaTime; // Decrease currentTime
        UIManager.Instance.UpdateTimer(currentTime);

        if (currentTime >= endHour) // Loop back to startHour
        {
            currentTime = startHour;
        }

        // Calculate day progress (reversed)
        float dayProgress = Mathf.InverseLerp(startHour, endHour, currentTime);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, lightIntensity.Evaluate(dayProgress));
        float sunAngle = Mathf.Lerp(90f, -90f, dayProgress); // Sun angle adjusted for reverse direction

        directionalLight.intensity = intensity;
        directionalLight.color = lightColor.Evaluate(dayProgress);
        directionalLight.transform.rotation = Quaternion.Euler(new Vector3(sunAngle, -30f, 170f));
    }

}
