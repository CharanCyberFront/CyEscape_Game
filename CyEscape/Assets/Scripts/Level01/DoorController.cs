using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    public Animator doorAnimator; // Drag and drop the Animator component for the door here.
    public float detectionRadius = 5f; // Distance at which the player can trigger the door.
    private Transform player; // To reference the player’s position.
    private BoxCollider2D doorCollider;

    void Start()
    {
        // Find the player by tag (assuming the player has the "Player" tag)
        player = GameObject.FindGameObjectWithTag("MainCharacter").transform;

        doorCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        // Calculate distance between player and the door
        float distance = Vector3.Distance(player.position, transform.position);

        // If the player is within detection radius, open the door
        if (distance < detectionRadius)
        {
            doorAnimator.SetBool("isOpen", true); // Trigger the open animation
        }
        else
        {
            doorAnimator.SetBool("isOpen", false); // Close the door when the player leaves
        }

    }

    void OnTriggerStay2D(Collider2D other) {

        if (doorAnimator.GetBool("isOpen")) 
        {
            if (other.CompareTag("Player")) 
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
                    SceneManager.LoadScene(1);
                }
            }
        }
    }
}
