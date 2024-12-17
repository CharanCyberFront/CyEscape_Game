using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AccessCardTrigger : MonoBehaviour
{
    public List<DialogueLine> successDialogue;
    private static bool hasEverTriggered = false;  // Static flag that persists even if object is disabled/re-enabled
    public SocialEngineeringMinigame minigame;
    public LevelExit levelExit;

    private void Start()
    {
        if (minigame == null)
        {
            Debug.LogError("SocialEngineeringMinigame not found in the scene!");
        }
        if (levelExit == null)
        {
            Debug.LogError("LevelExit not assigned to AccessCardTrigger!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If it has EVER triggered, do nothing
        if (hasEverTriggered) return;
        
        if (collision.CompareTag("Player"))
        {
            hasEverTriggered = true;  // Set this immediately and it will never change
            
            if (minigame != null)
            {
                Debug.Log("Starting minigame from trigger");                 
                minigame.StartMinigame();
            }
            else
            {
                Debug.LogError("SocialEngineeringMinigame is null!");
            }
        }
    }

    public void OnMinigameSuccess()
    {
        Debug.Log("Minigame success handled in AccessCardTrigger");
        
        if (levelExit != null)
        {
            //levelExit.SetHasAccessCard(true);
            Debug.Log("Access card status set to true in LevelExit");
        }
        else
        {
            Debug.LogError("LevelExit is null in AccessCardTrigger!");
        }
        
        if (successDialogue != null && successDialogue.Count > 0)
        {
            StartCoroutine(ShowSuccessDialogueAndHide());
        }
        else
        {
            StartCoroutine(ShowDefaultSuccessAndHide());
        }
    }

    private IEnumerator ShowSuccessDialogueAndHide()
    {
        DialogueManager.Instance.StartConversation(successDialogue);
        yield return new WaitForSeconds(successDialogue.Count * 2f);
        DialogueManager.Instance.HideDialoguePanel();
    }

    private IEnumerator ShowDefaultSuccessAndHide()
    {
        DialogueManager.Instance.ShowDialogue("Success! You've used social engineering to gain access.");
        yield return new WaitForSeconds(2f);
        DialogueManager.Instance.HideDialoguePanel();
    }
}