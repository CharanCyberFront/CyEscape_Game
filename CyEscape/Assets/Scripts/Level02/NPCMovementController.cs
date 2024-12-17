using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCMovementController : MonoBehaviour
{
    [SerializeField] private GameObject[] npcs;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float rightwardDistance = 10f;  // Distance to walk right
    [SerializeField] private float upwardDistance = 10f;     // Distance to walk up
    
    private bool isWalking = false;
    
    public void StartNPCsWalking()
    {
        if (!isWalking)
        {
            isWalking = true;
            foreach (GameObject npc in npcs)
            {
                StartCoroutine(MoveNPCAlongPath(npc));
            }
        }
    }

    private IEnumerator MoveNPCAlongPath(GameObject npc)
    {
        if (npc == null) yield break;

        Transform npcTransform = npc.transform;
        Animator animator = npc.GetComponent<Animator>();
        Vector3 startPosition = npcTransform.position;

        // Start animation
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
        }

        // First movement: Right
        Vector3 rightwardTarget = startPosition + Vector3.right * rightwardDistance;
        yield return StartCoroutine(MoveToPosition(npcTransform, rightwardTarget));

        // Second movement: Up
        Vector3 upwardTarget = rightwardTarget + Vector3.up * upwardDistance;
        yield return StartCoroutine(MoveToPosition(npcTransform, upwardTarget));

        Destroy(npc);
    }

    private IEnumerator MoveToPosition(Transform transform, Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                walkSpeed * Time.deltaTime
            );
            yield return null;
        }
        
        transform.position = targetPosition; // Ensure exact position
    }

    public void SetMovementDistances(float rightDistance, float upDistance)
    {
        rightwardDistance = rightDistance;
        upwardDistance = upDistance;
    }
}