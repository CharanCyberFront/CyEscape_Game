using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class EavesdroppingTrig : MonoBehaviour 
{
    public List<DialogueLine> eavesdroppingDialogue;
    public PlayerController playerController;
    private bool hasTriggered = false;
    public NPCMovement[] npcsToMove; // Add reference to NPCs that should move
    public float rightwardDistance = 10f;
    public float upwardDistance = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            StartEavesdropping();
        }
    }

    private void StartEavesdropping()
    {
        hasTriggered = true;
        if (playerController != null)
        {
            playerController.FreezeMovement();
        }
        
        DialogueManager.Instance.typingSpeed = 0.1f;
        DialogueManager.Instance.StartConversation(eavesdroppingDialogue);
        DialogueManager.Instance.OnDialogueEnded += OnEavesdroppingEnded;
        
        // Disable the collider immediately to prevent future triggers
        GetComponent<Collider2D>().enabled = false;
    }

    private void OnEavesdroppingEnded()
    {
        if (playerController != null)
        {
            playerController.UnfreezeMovement();
        }
        
        DialogueManager.Instance.typingSpeed = 0.05f;
        DialogueManager.Instance.OnDialogueEnded -= OnEavesdroppingEnded;
        
        // Start NPC movement after dialogue ends
        if (npcsToMove != null)
        {
            foreach (var npc in npcsToMove)
            {
                if (npc != null)
                {
                    npc.rightwardDistance = rightwardDistance;
                    npc.upwardDistance = upwardDistance;
                    npc.StartMoving();
                }
            }
        }
        
        // Completely disable the GameObject
        gameObject.SetActive(false);
    }
}