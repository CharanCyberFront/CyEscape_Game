using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorIsOpen : MonoBehaviour
{
    public Animator doorAnimator; // Reference to the Animator component for the door
    public float doorOpenDelay = 1f; // Time to wait before scene transition
    public bool terminalChallengeCompleted = false; // Tracks whether the terminal challenge is complete

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && terminalChallengeCompleted) // Check if the challenge is completed
        {
            if (doorAnimator != null)
            {
                doorAnimator.SetBool("isOpen", true); // Play door opening animation
            }
            
            PlayDoorOpenSound(); // Play the door-opening sound effect

            Debug.Log("Player entered. Door is opening.");
            StartCoroutine(LoadNextSceneAfterDelay()); // Load next scene after a delay
        }
        else if (other.CompareTag("Player") && !terminalChallengeCompleted)
        {
            Debug.Log("Player entered, but terminal challenge is not complete.");
        }
    }

    private void PlayDoorOpenSound()
    {
        // Find the GameObject tagged as DoorOpen
        GameObject doorSoundObject = GameObject.FindWithTag("DoorOpen");

        if (doorSoundObject != null)
        {
            // Get the AudioSource component and play the sound
            AudioSource audioSource = doorSoundObject.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
                Debug.Log("Playing door-opening sound effect.");
            }
            else
            {
                Debug.LogError("AudioSource component not found on the GameObject tagged as 'DoorOpen'.");
            }
        }
        else
        {
            Debug.LogError("GameObject tagged as 'DoorOpen' not found in the scene.");
        }
    }

    public void CompleteTerminalChallenge()
    {
        terminalChallengeCompleted = true; // Mark the challenge as complete
        Debug.Log("Terminal challenge completed. Door is now accessible.");
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(doorOpenDelay); // Wait for the door animation to finish
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Load next scene
    }
}