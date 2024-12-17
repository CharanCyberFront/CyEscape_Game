using UnityEngine;
using System.Collections;

public class GuardPatrolLvl5 : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform doorPosition;
    
    [Header("Patrol Settings")]
    public float leftBound = -16f;
    public float rightBound = -12f;
    
    private bool movingRight = false;
    private bool isPatrolling = true;
    private bool canSwitchDirection = true;
    private float directionLockTime = 0.5f;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isSleeping = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null)
        {
            Debug.LogError("No Animator component found on guard!");
        }
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer component found on guard!");
        }
    }
    
    void Update()
    {
        if (!isPatrolling || isSleeping) return;
        
        float moveAmount = moveSpeed * Time.deltaTime;
        float newX = transform.position.x + (movingRight ? moveAmount : -moveAmount);
        
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = movingRight;
        }
        
        newX = Mathf.Clamp(newX, leftBound, rightBound);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }
        
        if (canSwitchDirection)
        {
            if (movingRight && newX >= rightBound)
            {
                StartCoroutine(SwitchDirection(false));
            }
            else if (!movingRight && newX <= leftBound)
            {
                StartCoroutine(SwitchDirection(true));
            }
        }
    }

    private IEnumerator SwitchDirection(bool goingRight)
    {
        canSwitchDirection = false;
        movingRight = goingRight;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !goingRight;
        }
        
        yield return new WaitForSeconds(directionLockTime);
        canSwitchDirection = true;
    }

    public void Sleep(int seconds)
    {
        if (!isSleeping)
        {
            StartCoroutine(SleepRoutine(seconds));
        }
    }

    private IEnumerator SleepRoutine(int seconds)
    {
        isSleeping = true;
        
        // Stop walking animation
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }

        // Wait for specified duration
        yield return new WaitForSeconds(seconds);

        // Resume normal behavior
        isSleeping = false;
        
        // Resume walking animation if we're still patrolling
        if (isPatrolling && animator != null)
        {
            animator.SetBool("isWalking", true);
        }
    }

    public void UnlockDoor()
    {
        Debug.LogError("Starting unlock");
        isPatrolling = false;
        
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }
        
        StartCoroutine(MoveToUnlock());
    }
    
    private IEnumerator MoveToUnlock()
    {
        if (doorPosition == null)
        {
            Debug.LogError("Door position not set!");
            yield break;
        }

        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(doorPosition.position.x, transform.position.y, transform.position.z);
        float duration = Vector3.Distance(startPos, targetPos) / moveSpeed;
        float elapsed = 0;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = targetPos.x > startPos.x;
        }
        
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;
            
            transform.position = Vector3.Lerp(startPos, targetPos, percent);
            yield return null;
        }
        
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }
        
        InteractableLvl5 interactable = FindObjectOfType<InteractableLvl5>();
        if (interactable != null)
        {
            interactable.UnlockDoor();
        }
        else
        {
            Debug.LogError("Failed to find InteractableLvl5 component!");
        }
    }

    public void HandleTerminalCommand(string command)
    {
        Debug.Log("Guard received command: " + command);
        if (command.ToLower() == "unlock")
        {
            Debug.Log("Guard starting unlock sequence");
            UnlockDoor();
        }
    }
}