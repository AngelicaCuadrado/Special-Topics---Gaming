using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Time References")]
    [SerializeField] private RectTransform sunIcon;
    [SerializeField] private Image timeBarFill;

    [Header("UI Stats References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI wormsKilledText;
    [Tooltip("UI Text for Trucks Filled (e.g., Trucks: 0/1)")]
    [SerializeField] private TextMeshProUGUI trucksGoalText;

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverUI;
    [Tooltip("Drag the Win Game Panel/Canvas here")]
    [SerializeField] private GameObject winUI;

    [Header("Settings & Goals")]
    [SerializeField] private float dayDurationInSeconds = 200f;
    [SerializeField] private float sunRotationSpeed = -45f;
    [Tooltip("How many trucks need to be filled to win the level?")]
    [SerializeField] private int trucksGoal = 1;


    private float currentTime;
    private int totalScore = 0;
    private int wormsKilled = 0;
    private int trucksFilled = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentTime = dayDurationInSeconds;
        isGameOver = false;
        Time.timeScale = 1f;

        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (winUI != null) winUI.SetActive(false);

        UpdateUI();
    }

    void Update()
    {
        if (isGameOver) return;

        if (sunIcon != null) sunIcon.Rotate(0, 0, sunRotationSpeed * Time.deltaTime);

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (timeBarFill != null) timeBarFill.fillAmount = currentTime / dayDurationInSeconds;

            if (currentTime <= 0) EndDay(false);
        }
    }


    public void AddTomatoScore() { totalScore += 20; UpdateUI(); }
    public void BasketFilledScore() { totalScore += 15; UpdateUI(); }
    public void CaterpillarKilledScore() { totalScore += 10; wormsKilled++; UpdateUI(); }

    public void TruckDispatched()
    {
        totalScore += 50;
        trucksFilled++;
        UpdateUI();

        //if (trucksFilled >= trucksGoal)
        //{
        //    EndDay(true);
        //}
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "" + totalScore;
        if (wormsKilledText != null) wormsKilledText.text = "" + wormsKilled;
        if (trucksGoalText != null) trucksGoalText.text = "" + trucksFilled + " / " + trucksGoal;
    }

    private void EndDay(bool didWin)
    {
        isGameOver = true;
        Time.timeScale = 0f;

        if (didWin)
        {
            if (winUI != null) winUI.SetActive(true);
        }
        else
        {
            currentTime = 0;
            if (timeBarFill != null) timeBarFill.fillAmount = 0;
            if (gameOverUI != null) gameOverUI.SetActive(true);
        }
    }
}