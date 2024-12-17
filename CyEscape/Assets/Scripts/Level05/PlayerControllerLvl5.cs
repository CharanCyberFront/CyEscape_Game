using UnityEngine;
using System.Collections.Generic;

public class PlayerControllerLvl5 : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
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