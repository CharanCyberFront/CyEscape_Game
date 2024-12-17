using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private Animator animator;
    private Vector2 movement;
    private Vector2 lastNonZeroMovement;

    // Animation parameter names
    private readonly string MovementX = "MovementX";
    private readonly string MovementY = "MovementY";

    // Threshold for idle/walking transition
    private const float MOVEMENT_THRESHOLD = 0.1f;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing on " + gameObject.name);
        }
        // Initialize last movement as facing down
        lastNonZeroMovement = Vector2.down;
    }

    void Update()
    {
        // Get input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize diagonal movement
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // Store last non-zero movement
        if (movement.magnitude > 0)
        {
            lastNonZeroMovement = movement;
        }

        // If not moving, use last direction but scaled to idle threshold
        if (movement.magnitude == 0)
        {
            animator.SetFloat(MovementX, lastNonZeroMovement.x * MOVEMENT_THRESHOLD);
            animator.SetFloat(MovementY, lastNonZeroMovement.y * MOVEMENT_THRESHOLD);
        }
        else
        {
            // Use actual movement values for walking animations
            animator.SetFloat(MovementX, movement.x);
            animator.SetFloat(MovementY, movement.y);
        }
    }
}