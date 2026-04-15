using UnityEngine;
using UnityEngine.UI;

public class DayTimerUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag the SunIcon RectTransform here")]
    [SerializeField] private RectTransform sunIcon;

    [Tooltip("Drag the TimeBarFill Image here (Make sure its Image Type is 'Filled')")]
    [SerializeField] private Image timeBarFill;

    [Header("Settings")]
    [Tooltip("How long does one day last in real-world seconds?")]
    [SerializeField] private float dayDurationInSeconds = 120f;

    [Tooltip("Speed of the sun spinning. Negative values spin clockwise.")]
    [SerializeField] private float sunRotationSpeed = -45f;

    private float currentTime;

    void Start()
    {
        currentTime = dayDurationInSeconds;
    }

    void Update()
    {
        if (sunIcon != null)
        {
            sunIcon.Rotate(0, 0, sunRotationSpeed * Time.deltaTime);
        }
        if (timeBarFill != null && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timeBarFill.fillAmount = currentTime / dayDurationInSeconds;
        }
    }
}