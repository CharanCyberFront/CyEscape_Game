using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    public List<DialogueLine> conversation;
    public NPCMovement[] npcsToMove; // Array of NPCs to move after conversation

    public float rightwardDistance = 10f;
    public float upwardDistance = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartConversation(conversation);
            DialogueManager.Instance.OnDialogueEnded += MoveNPCs;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnded -= MoveNPCs;
        }
    }

    private void MoveNPCs()
    {
        if (npcsToMove != null)
        {
            foreach (var npc in npcsToMove)
            {
                if (npc != null)
                {
                    // Configure the movement distances if needed
                    npc.rightwardDistance = rightwardDistance;
                    npc.upwardDistance = upwardDistance;
                    npc.StartMoving();
                }
            }
        }
        
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnded -= MoveNPCs; // Unsubscribe after moving
        }
    }
}