using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    public DialogueTrigger playerThoughtsTrigger;
    private bool canMove = true;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (canMove)
        {
            // Get input
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // Normalize diagonal movement
            if (movement.magnitude > 1)
            {
                movement.Normalize();
            }
        }
        else
        {
            movement = Vector2.zero;
        }

        // Trigger player thoughts
        if (Input.GetKeyDown(KeyCode.T))
        {
            playerThoughtsTrigger.conversation = new List<DialogueLine>
            {
                new DialogueLine { speaker = "Player", dialogue = "I need to get inside that lab. But how? I can't just force my way in. Maybe I can… convince someone to help." }
            };
            DialogueManager.Instance.StartConversation(playerThoughtsTrigger.conversation);
        }
    }

    void FixedUpdate()
    {
        // Apply movement
        rb.velocity = movement * moveSpeed;
    }

    public void FreezeMovement()
    {
        canMove = false;
        rb.velocity = Vector2.zero;
    }

    public void UnfreezeMovement()
    {
        canMove = true;
    }
}