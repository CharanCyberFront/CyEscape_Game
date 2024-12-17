using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableLevel3 : MonoBehaviour
{
    public GameObject terminal; // The terminal prefab to instantiate
    private bool isPlayerInRange = false; // Tracks if the player is within range
    private bool terminalSpawned = false; // Tracks if a terminal has already been spawned

    void Update()
    {
        // Check if the player is in range and the E key is pressed
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !terminalSpawned)
        {
            SpawnTerminal();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player in range. Press E to interact.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player left range.");
        }
    }

    private void SpawnTerminal()
    {
        Time.timeScale = 0; // Pause the game
        GameObject terminalInstance = Instantiate(terminal, transform.position, Quaternion.identity); // Spawn the terminal
        terminalInstance.GetComponent<InterpreterLvl3>().SetInteractable(this); // Link the terminal to this interactable
        terminalSpawned = true; // Set the flag to prevent multiple spawns
        Debug.Log("Terminal spawned.");
    }

    public void ResetTerminalSpawned()
    {
        terminalSpawned = false; // Reset the terminal spawn flag
        Debug.Log("Terminal spawn flag reset.");
    }
}
