using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] // Add this attribute
    public List<DialogueLine> dialogueLines;
    
    [SerializeField] // Add this attribute
    public bool isMainNPC = false;
    
    [SerializeField] // Add this attribute
    public bool isSocialEngineeringTarget = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Modify this to prevent regular dialogue for social engineering target
        if (collision.CompareTag("Player") && !isSocialEngineeringTarget)
        {
            if (dialogueLines != null && dialogueLines.Count > 0 && DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartConversation(dialogueLines);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.EndConversation();
        }
    }
}