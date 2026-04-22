using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [Header("UI Canvas References")]
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private GameObject inGameCanvas;

    [Header("Input Actions")]
    [Tooltip("Bind to Left Controller Primary Button (e.g., X)")]
    [SerializeField] private InputActionReference tutorialAction;
    [Tooltip("Bind to Left Controller Secondary Button (e.g., Y)")]
    [SerializeField] private InputActionReference inGameMenuAction;

    [Header("Tutorial Progression")]
    [Tooltip("Assign the individual panels/steps of the tutorial here")]
    [SerializeField] private GameObject[] tutorialSteps;

    private int currentStepIndex = 0;

    private void Start()
    {
        tutorialCanvas.SetActive(true);
        inGameCanvas.SetActive(false);

        if (tutorialSteps.Length > 0)
        {
            ShowTutorialStep(0);
        }
    }

    private void OnEnable()
    {
        if (tutorialAction != null) tutorialAction.action.performed += OnTutorialPressed;
        if (inGameMenuAction != null) inGameMenuAction.action.performed += OnInGameMenuPressed;
    }

    private void OnDisable()
    {
        if (tutorialAction != null) tutorialAction.action.performed -= OnTutorialPressed;
        if (inGameMenuAction != null) inGameMenuAction.action.performed -= OnInGameMenuPressed;
    }

    private void OnTutorialPressed(InputAction.CallbackContext context)
    {
        if (!tutorialCanvas.activeSelf)
        {
            tutorialCanvas.SetActive(true);
            inGameCanvas.SetActive(false);
            ShowTutorialStep(0);
            return;
        }
        currentStepIndex++;

        if (currentStepIndex >= tutorialSteps.Length)
        {
            tutorialCanvas.SetActive(false);
        }
        else
        {
            ShowTutorialStep(currentStepIndex);
        }
    }

    private void OnInGameMenuPressed(InputAction.CallbackContext context)
    {
        bool isActive = !inGameCanvas.activeSelf;
        inGameCanvas.SetActive(isActive);

        if (isActive)
        {
            tutorialCanvas.SetActive(false);
        }
    }

    private void ShowTutorialStep(int index)
    {
        currentStepIndex = index;

        for (int i = 0; i < tutorialSteps.Length; i++)
        {
            if (tutorialSteps[i] != null)
            {
                tutorialSteps[i].SetActive(i == currentStepIndex);
            }
        }
    }
}