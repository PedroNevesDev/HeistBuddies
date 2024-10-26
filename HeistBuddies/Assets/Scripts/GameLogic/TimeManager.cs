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
    [SerializeField] private float startHour = 0f;
    [SerializeField] private float endHour = 7f;

    public float CurrentTime { get => currentTime; private set => currentTime = value; }

    private void Start()
    {
        currentTime = startHour;
    }

    private void Update()
    {
        //24H time convertion
        float totalHours = endHour - startHour;
        float cycleSpeed = totalHours / (cycleDuration * 60f);
        currentTime += cycleSpeed * Time.deltaTime;
        UIManager.Instance.UpdateTimer(currentTime);

        if (currentTime >= endHour)
        {
            currentTime = startHour;
        }

        float dayProgress = Mathf.InverseLerp(startHour, endHour, currentTime);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, lightIntensity.Evaluate(dayProgress));
        float sunAngle = Mathf.Lerp(-90f, 90f, dayProgress);

        directionalLight.intensity = intensity;
        directionalLight.color = lightColor.Evaluate(dayProgress);
        directionalLight.transform.rotation = Quaternion.Euler(new Vector3(sunAngle, -30f, 170f));
    }
}
