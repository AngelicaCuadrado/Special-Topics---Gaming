using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [Header("UI Canvas References")]
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private GameObject inGameCanvas;
    [SerializeField] private GameObject pauseUI;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference tutorialAction;
    [SerializeField] private InputActionReference inGameMenuAction;
    [SerializeField] private InputActionReference pauseMenuAction;

    [Header("Tutorial Progression")]
    [SerializeField] private GameObject[] tutorialSteps;

    private int currentStepIndex = 0;

    private void Start()
    {
        tutorialCanvas.SetActive(true);
        inGameCanvas.SetActive(false);
        pauseUI.SetActive(false);

        if (tutorialSteps.Length > 0)
            ShowTutorialStep(0);
    }

    private void OnEnable()
    {
        if (tutorialAction != null)
        {
            tutorialAction.action.Enable();
            tutorialAction.action.performed += OnTutorialPressed;
        }

        if (inGameMenuAction != null)
        {
            inGameMenuAction.action.Enable();
            inGameMenuAction.action.performed += OnInGameMenuPressed;
        }

        if (pauseMenuAction != null)
        {
            pauseMenuAction.action.Enable();
            pauseMenuAction.action.performed += OnPauseMenuPressed;
        }
    }

    private void OnDisable()
    {
        if (tutorialAction != null)
        {
            tutorialAction.action.performed -= OnTutorialPressed;
            tutorialAction.action.Disable();
        }

        if (inGameMenuAction != null)
        {
            inGameMenuAction.action.performed -= OnInGameMenuPressed;
            inGameMenuAction.action.Disable();
        }

        if (pauseMenuAction != null)
        {
            pauseMenuAction.action.performed -= OnPauseMenuPressed;
            pauseMenuAction.action.Disable();
        }
    }

    private void OnTutorialPressed(InputAction.CallbackContext context)
    {
        if (!tutorialCanvas.activeSelf)
        {
            tutorialCanvas.SetActive(true);
            inGameCanvas.SetActive(false);
            pauseUI.SetActive(false);
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
            pauseUI.SetActive(false);
        }
    }

    private void OnPauseMenuPressed(InputAction.CallbackContext context)
    {
        bool isActive = !pauseUI.activeSelf;
        pauseUI.SetActive(isActive);

        if (isActive)
        {
            tutorialCanvas.SetActive(false);
            inGameCanvas.SetActive(false);
        }
    }

    private void ShowTutorialStep(int index)
    {
        currentStepIndex = index;

        for (int i = 0; i < tutorialSteps.Length; i++)
        {
            if (tutorialSteps[i] != null)
                tutorialSteps[i].SetActive(i == currentStepIndex);
        }
    }
}