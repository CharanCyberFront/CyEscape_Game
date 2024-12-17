// LevelEndTrigger.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelEndTrigger : MonoBehaviour
{
    public float fadeOutDuration = 1.0f;
    public float playerExitSpeed = 3f;
    public float guardDelayBeforeExit = 0.5f;
    public float guardExitDuration = 1.0f;
    public GuardPatrolLvl5 guardPatrol;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControllerLvl5 player = other.GetComponent<PlayerControllerLvl5>();
            SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
            
            if (player != null)
            {
                player.FreezeMovement();
                StartCoroutine(HandleLevelExit(player.gameObject, playerSprite));
            }
        }
    }

    private IEnumerator HandleLevelExit(GameObject player, SpriteRenderer playerSprite)
    {
        // Create screen fade overlay
        GameObject overlay = new GameObject("FadeOverlay");
        SpriteRenderer fadeRenderer = overlay.AddComponent<SpriteRenderer>();
        fadeRenderer.color = new Color(0, 0, 0, 0);
        fadeRenderer.sortingOrder = 999;
        
        // Size the overlay to cover the screen
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            float height = 2f * mainCamera.orthographicSize;
            float width = height * mainCamera.aspect;
            fadeRenderer.transform.localScale = new Vector3(width, height, 1);
            fadeRenderer.transform.position = new Vector3(
                mainCamera.transform.position.x, 
                mainCamera.transform.position.y, 
                fadeRenderer.transform.position.z);
        }

        // Player exit sequence
        float elapsedTime = 0f;
        Vector3 startPos = player.transform.position;
        
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / fadeOutDuration;
            
            // Move player left
            player.transform.position += Vector3.left * playerExitSpeed * Time.deltaTime;
            
            // Fade out player sprite
            if (playerSprite != null)
            {
                playerSprite.color = new Color(1, 1, 1, 1 - percent);
            }
            
            yield return null;
        }

        // Hide player completely
        playerSprite.enabled = false;

        // Wait before guard follows
        yield return new WaitForSeconds(guardDelayBeforeExit);

        // Guard follows player
        if (guardPatrol != null)
        {
            // Make sure guard is facing left
            SpriteRenderer guardSprite = guardPatrol.GetComponent<SpriteRenderer>();
            if (guardSprite != null)
            {
                guardSprite.flipX = false; // Adjust this based on your sprite's default direction
            }

            // Start guard's walking animation
            Animator guardAnimator = guardPatrol.GetComponent<Animator>();
            if (guardAnimator != null)
            {
                guardAnimator.SetBool("isWalking", true);
            }

            // Move guard left
            elapsedTime = 0f;
            while (elapsedTime < guardExitDuration)
            {
                elapsedTime += Time.deltaTime;
                guardPatrol.transform.position += Vector3.left * playerExitSpeed * Time.deltaTime;
                yield return null;
            }
        }

        // Fade screen to black
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration * 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / (fadeOutDuration * 0.5f));
            fadeRenderer.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Load next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}