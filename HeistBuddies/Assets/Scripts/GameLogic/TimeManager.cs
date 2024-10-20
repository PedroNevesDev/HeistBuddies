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
    [Tooltip("This is in minutes!")]
    [SerializeField] private float cycleDuration = 10f;
    [SerializeField] private float startHour = 0f;
    [SerializeField] private float endHour = 7f;

    public float CurrentTime { get; private set; }

    private void Start()
    {
        CurrentTime = startHour;
    }

    private void Update()
    {
        //24H time convertion
        float cycleSpeed = 24f / (cycleDuration * 60f);
        CurrentTime += cycleSpeed * Time.deltaTime;

        if (CurrentTime > endHour)
        {
            CurrentTime = startHour;
        }

        float dayProgress = Mathf.InverseLerp(startHour, endHour, CurrentTime);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, lightIntensity.Evaluate(dayProgress));
        float sunAngle = Mathf.Lerp(-90f, 90f, dayProgress);

        directionalLight.intensity = intensity;
        directionalLight.color = lightColor.Evaluate(dayProgress);
        directionalLight.transform.rotation = Quaternion.Euler(new Vector3(sunAngle, -30f, 170f));
    }
}
