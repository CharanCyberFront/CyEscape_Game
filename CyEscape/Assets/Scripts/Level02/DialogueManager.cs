using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public Text dialogueText;
    public Text speakerNameText;
    public GameObject dialoguePanel;
    public float typingSpeed = 0.05f;
    public int charactersPerLine = 40;

    [SerializeField] private NPCMovementController npcMovementController;


    public delegate void DialogueEndedDelegate();
    public event DialogueEndedDelegate OnDialogueEnded;

    private Coroutine typingCoroutine;
    private Queue<DialogueLine> currentConversation;

    [Header("UI Styling")]
    public Image dialogueBackgroundPanel;  // Main panel background
    public Image dialogueBorder;           // Separate border image
    public float cornerRadius = 10f;       // For rounded corners
    
    private void SetupDialoguePanel()
    {
        if (dialoguePanel != null)
        {
            // Add or get Image component for the background
            dialogueBackgroundPanel = dialoguePanel.GetComponent<Image>();
            if (dialogueBackgroundPanel == null)
                dialogueBackgroundPanel = dialoguePanel.gameObject.AddComponent<Image>();
            
            // Set background styling
            dialogueBackgroundPanel.color = new Color(0, 0, 0, 0.85f);  // Semi-transparent black
            
            // Add outline for border effect
            var outline = dialoguePanel.GetComponent<Outline>();
            if (outline == null)
                outline = dialoguePanel.gameObject.AddComponent<Outline>();
            
            outline.effectColor = new Color(0, 174, 239, 1f);  // Cyber blue
            outline.effectDistance = new Vector2(2, 2);        // Border thickness
            
            // Style the text elements
            if (dialogueText != null)
            {
                dialogueText.color = Color.white;
                // Add shadow for better readability
                var shadow = dialogueText.gameObject.GetComponent<Shadow>();
                if (shadow == null)
                    shadow = dialogueText.gameObject.AddComponent<Shadow>();
                shadow.effectColor = new Color(0, 0, 0, 0.5f);
                shadow.effectDistance = new Vector2(1, 1);
            }
            
            if (speakerNameText != null)
            {
                speakerNameText.color = new Color(0, 174, 239, 1f);  // Match border color
                var shadow = speakerNameText.gameObject.GetComponent<Shadow>();
                if (shadow == null)
                    shadow = speakerNameText.gameObject.AddComponent<Shadow>();
                shadow.effectColor = new Color(0, 0, 0, 0.5f);
                shadow.effectDistance = new Vector2(1, 1);
            }
        }
    }
    private void Awake()
    {
        SetupDialoguePanel();

        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        currentConversation = new Queue<DialogueLine>();

        // Ensure all necessary components are assigned
        if (dialoguePanel == null)
            Debug.LogError("DialoguePanel is not assigned in the DialogueManager!");
        if (dialogueText == null)
            Debug.LogError("DialogueText is not assigned in the DialogueManager!");
        if (speakerNameText == null)
            Debug.LogError("SpeakerNameText is not assigned in the DialogueManager!");

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void StartConversation(List<DialogueLine> conversation)
    {
        if (conversation == null || conversation.Count == 0)
        {
            Debug.LogWarning("Attempted to start a conversation with no dialogue lines.");
            return;
        }

        currentConversation = new Queue<DialogueLine>(conversation);
        ShowDialoguePanel();
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (currentConversation.Count == 0)
        {
            EndConversation();
            return;
        }

        DialogueLine line = currentConversation.Dequeue();
        if (speakerNameText != null)
            speakerNameText.text = line.speaker;
        
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        
        if (dialogueText != null)
            typingCoroutine = StartCoroutine(TypeLine(line.dialogue));
        else
            Debug.LogError("DialogueText is not assigned in the DialogueManager!");
    }

    private IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        string[] words = line.Split(' ');
        string currentLine = "";

        foreach (string word in words)
        {
            if (currentLine.Length + word.Length > charactersPerLine)
            {
                currentLine += "\n" + word + " ";
            }
            else
            {
                currentLine += word + " ";
            }

            dialogueText.text = currentLine;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(1f);
        DisplayNextLine();
    }

    public void EndConversation()
    {
        currentConversation.Clear();
        HideDialoguePanel();
        OnDialogueEnded?.Invoke();

        if (npcMovementController != null)
        {
            npcMovementController.StartNPCsWalking();
        }
        else
        {
            Debug.LogError("NPCMovementController not assigned in DialogueManager!");
        }
    }

    public void ShowDialogue(string text)
    {
        List<DialogueLine> singleLineDialogue = new List<DialogueLine>
        {
            new DialogueLine { speaker = "System", dialogue = text }
        };
        StartConversation(singleLineDialogue);
    }

    private void ShowDialoguePanel()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);
        else
            Debug.LogError("DialoguePanel is not assigned in the DialogueManager!");
    }

    public void HideDialoguePanel()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }
}