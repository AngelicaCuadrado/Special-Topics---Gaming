using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Time References")]
    [Tooltip("Drag the SunIcon RectTransform here")]
    [SerializeField] private RectTransform sunIcon;
    [Tooltip("Drag the TimeBarFill Image here")]
    [SerializeField] private Image timeBarFill;

    [Header("UI Stats References")]
    [Tooltip("Drag the TextMeshPro UI for Score here")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [Tooltip("Drag the TextMeshPro UI for Worms Killed here")]
    [SerializeField] private TextMeshProUGUI wormsKilledText;
    [Tooltip("Drag the Game Over Panel/Canvas here")]
    [SerializeField] private GameObject gameOverUI;

    [Header("Settings")]
    [SerializeField] private float dayDurationInSeconds = 120f;
    [SerializeField] private float sunRotationSpeed = -45f;

    private float currentTime;
    private int totalScore = 0;
    private int wormsKilled = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentTime = dayDurationInSeconds;
        isGameOver = false;
        Time.timeScale = 1f;

        if (gameOverUI != null) gameOverUI.SetActive(false);

        UpdateUI();
    }

    void Update()
    {
        if (isGameOver) return;

        if (sunIcon != null)
        {
            sunIcon.Rotate(0, 0, sunRotationSpeed * Time.deltaTime);
        }
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (timeBarFill != null)
            {
                timeBarFill.fillAmount = currentTime / dayDurationInSeconds;
            }
            if (currentTime <= 0)
            {
                EndDay();
            }
        }
    }

    public void AddTomatoScore()
    {
        totalScore += 20;
        UpdateUI();
    }

    public void BasketFilledScore()
    {
        totalScore += 15;
        UpdateUI();
    }

    public void TruckDeliveryScore()
    {
        totalScore += 50;
        UpdateUI();
    }

    public void CaterpillarKilledScore()
    {
        totalScore += 10;
        wormsKilled++;
        UpdateUI();
    }


    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + totalScore;
        if (wormsKilledText != null) wormsKilledText.text = "Worms: " + wormsKilled;
    }

    private void EndDay()
    {
        isGameOver = true;
        currentTime = 0;

        if (timeBarFill != null) timeBarFill.fillAmount = 0;
        if (gameOverUI != null) gameOverUI.SetActive(true);

        Time.timeScale = 0f;
    }
}