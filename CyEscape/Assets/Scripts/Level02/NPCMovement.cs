using UnityEngine;
using System.Collections;

public class NPCMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rightwardDistance = 10f;
    public float upwardDistance = 10f;
    private bool isMoving = false;
    private Vector3 currentTarget;
    private const float POSITION_THRESHOLD = 0.01f;

    public void StartMoving()
    {
        if (!isMoving)
        {
            isMoving = true;
            StartCoroutine(MoveAlongPath());
        }
    }

    private IEnumerator MoveAlongPath()
    {
        Vector3 startPosition = transform.position;
        
        // First movement: Right
        currentTarget = startPosition + Vector3.right * rightwardDistance;
        yield return StartCoroutine(MoveToPosition(currentTarget));
        
        // Small delay between movements
        yield return new WaitForSeconds(0.1f);
        
        // Second movement: Up
        currentTarget = transform.position + Vector3.up * upwardDistance;
        yield return StartCoroutine(MoveToPosition(currentTarget));
        
        // Small delay before destroying
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        float startTime = Time.time;
        Vector3 startPos = transform.position;
        float journeyLength = Vector3.Distance(startPos, targetPosition);
        
        while (Vector3.Distance(transform.position, targetPosition) > POSITION_THRESHOLD)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            
            transform.position = Vector3.Lerp(startPos, targetPosition, fractionOfJourney);
            
            // Debug logging
            if (fractionOfJourney > 1f)
            {
                Debug.Log($"NPC position: {transform.position}, Target: {targetPosition}, Distance: {Vector3.Distance(transform.position, targetPosition)}");
            }
            
            yield return null;
        }
        
        // Ensure exact final position
        transform.position = targetPosition;
    }
}