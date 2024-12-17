using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelExit : MonoBehaviour
{
    public string nextLevelName = "Level3";
    private bool hasAccessCard = false;
    private Animator doorAnimator;
    public float doorOpenDelay = 1f; // Time to wait after door opens before scene change

    public bool HasAccessCard()  // Add this getter
    {
        return hasAccessCard;
    }

    private void Start()
    {
        doorAnimator = GetComponent<Animator>();
        if (doorAnimator == null)
        {
            Debug.LogError("Animator component missing on door!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            SceneManager.LoadScene(3);
        }
    }

    // private IEnumerator OpenDoorAndExit()
    // {
    //     // Play door opening animation
    //     if (doorAnimator != null)
    //     {
    //         doorAnimator.SetTrigger("OpenDoor");
    //         yield return new WaitForSeconds(doorOpenDelay);
    //     }
        
    //     // Load next level
    //     SceneManager.LoadScene(2);
    // }

    // public void SetHasAccessCard(bool value)
    // {
    //     hasAccessCard = value;
    //     Debug.Log("Access card status updated: " + hasAccessCard);
    // }
}