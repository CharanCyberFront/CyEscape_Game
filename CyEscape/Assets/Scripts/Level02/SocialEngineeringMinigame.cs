using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SocialEngineeringMinigame : MonoBehaviour
{
    [System.Serializable]
    public class DialogueOption
    {
        public string optionText;
        public bool isCorrect;
        public string response;
        [TextArea(3, 5)]
        public string educationalFeedback;
    }

    [System.Serializable]
    public class DialogueStep
    {
        public string question;
        public List<DialogueOption> options;
    }

    public List<DialogueStep> dialogueSteps;
    public Text questionText;
    public Button[] optionButtons;
    public Text feedbackText;
    public GameObject minigamePanel;
    public float delayBeforeRestart = 5f;

    private PlayerController playerController;
    private int currentStep = 0;
    private bool gameSucceeded = false;

    [Header("Educational Feedback UI")]
    public GameObject educationalPanel;
    public Text educationalText;
    public Button continueButton;

    public GameObject door;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        Debug.Log("Starting SocialEngineeringMinigame initialization");

        if (playerController == null)
        {
            Debug.LogError("PlayerController not found in the scene!");
        }

        if (minigamePanel != null)
        {
            minigamePanel.SetActive(false);
        }

        if (educationalPanel != null)
        {
            educationalPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Educational Panel is not assigned!");
        }

        if (educationalText != null)
        {
            educationalText.alignment = TextAnchor.MiddleCenter;
            RectTransform textRect = educationalText.GetComponent<RectTransform>();
            if (textRect != null)
            {
                textRect.anchorMin = new Vector2(0, 0);
                textRect.anchorMax = new Vector2(1, 1);
                textRect.pivot = new Vector2(0.5f, 0.5f);
                textRect.offsetMin = new Vector2(20, 20); // Padding from edges
                textRect.offsetMax = new Vector2(-20, -20); // Padding from edges
                textRect.anchoredPosition = Vector2.zero;
                textRect.sizeDelta = Vector2.zero;
            }
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(RestartLevel);
        }

        foreach (Button btn in optionButtons)
        {
            if (btn != null)
            {
                Text buttonText = btn.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.raycastTarget = false;
                }
            }
        }
    }

    public void StartMinigame()
    {
        Debug.Log("StartMinigame called");
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.HideDialoguePanel();
        }

        currentStep = 0;
        gameSucceeded = false;

        if (minigamePanel == null)
        {
            Debug.LogError("MinigamePanel reference is null!");
            return;
        }

        if (playerController != null)
        {
            playerController.FreezeMovement();
        }
            
        minigamePanel.SetActive(true);
        EnableAllButtons();
        DisplayCurrentStep();
    }

    private void DisableAllButtons()
    {
        foreach (Button btn in optionButtons)
        {
            if (btn != null)
            {
                btn.interactable = false;
            }
        }
    }

    private void EnableAllButtons()
    {
        foreach (Button btn in optionButtons)
        {
            if (btn != null)
            {
                btn.interactable = true;
            }
        }
    }

    private void DisplayCurrentStep()
    {
        if (currentStep < dialogueSteps.Count)
        {
            DialogueStep step = dialogueSteps[currentStep];
            questionText.text = step.question;

            for (int i = 0; i < optionButtons.Length; i++)
            {
                if (i < step.options.Count)
                {
                    Button btn = optionButtons[i];
                    btn.gameObject.SetActive(true);

                    Text buttonText = btn.GetComponentInChildren<Text>();
                    buttonText.raycastTarget = false;
                    buttonText.text = step.options[i].optionText;
                    
                    btn.onClick.RemoveAllListeners();
                    
                    int index = i; 
                    btn.onClick.AddListener(() => OnOptionSelected(index));
                }
                else
                {
                    optionButtons[i].gameObject.SetActive(false);
                }
            }
            EnableAllButtons();
        }
        else
        {
            EndMinigame();
        }
    }

    private void OnOptionSelected(int optionIndex)
    {
        Debug.Log($"Option {optionIndex} selected");
        DisableAllButtons();
        
        if (currentStep >= dialogueSteps.Count || optionIndex >= dialogueSteps[currentStep].options.Count)
        {
            Debug.LogError("Invalid step or option index");
            return;
        }

        DialogueOption selectedOption = dialogueSteps[currentStep].options[optionIndex];
        feedbackText.text = selectedOption.response;

        if (selectedOption.isCorrect)
        {
            currentStep++;
            if (currentStep < dialogueSteps.Count)
            {
                Invoke("DisplayCurrentStep", 2f);
            }
            else
            {
                gameSucceeded = true;
                Invoke("EndMinigame", 2f);
            }
        }
        else
        {
            Debug.Log("Showing educational feedback and restarting");
            if (minigamePanel != null)
            {
                minigamePanel.SetActive(false);
            }

            if (educationalPanel != null)
            {
                educationalPanel.SetActive(true);
                if (educationalText != null)
                {
                    educationalText.text = selectedOption.educationalFeedback;
                }
            }
            
            // Use Invoke instead of Coroutine for more reliable execution
            Invoke("RestartLevel", delayBeforeRestart);
        }
    }

    private void RestartLevel()
    {
        Debug.Log("Restarting level");
        
        // Make sure to disable all panels before restarting
        if (educationalPanel != null)
        {
            educationalPanel.SetActive(false);
        }
        if (minigamePanel != null)
        {
            minigamePanel.SetActive(false);
        }
        
        // Ensure player is unfrozen before restart
        if (playerController != null)
        {
            playerController.UnfreezeMovement();
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void EndMinigame()
    {
        if (playerController != null)
        {
            playerController.UnfreezeMovement();
        }

        if (minigamePanel != null)
        {
            minigamePanel.SetActive(false);
        }

        if (gameSucceeded)
        {
            door.GetComponent<Animator>().SetTrigger("OpenDoor");
            // AccessCardTrigger accessCardTrigger = FindObjectOfType<AccessCardTrigger>();
            // if (accessCardTrigger != null)
            // {
            //     accessCardTrigger.OnMinigameSuccess();
            // }
        }
        else
        {
            StartCoroutine(ShowFinalDialogueAndHide());
        }
    }

    private IEnumerator ShowFinalDialogueAndHide()
    {
        DialogueManager.Instance.ShowDialogue("Jamie: Sorry, I can't help you. Please leave.");
        yield return new WaitForSeconds(2f);
        DialogueManager.Instance.HideDialoguePanel();
    }

    private void OnDestroy()
    {
        if (playerController != null)
        {
            playerController.UnfreezeMovement();
        }
    }
}