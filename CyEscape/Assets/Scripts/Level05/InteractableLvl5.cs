using UnityEngine;
using UnityEngine.UI;

public class InteractableLvl5 : MonoBehaviour
{
    public Canvas mainCanvas;
    public GameObject terminalPrefab;
    public GameObject Level5BGwithdoorClosed;
    public GameObject Level5BGwithdoorOpened;

    public GameObject exitBlocker;
    public GuardPatrolLvl5 guardPatrol;
    private bool isTerminalOpen = false;
    private GameObject currentTerminal;
    private bool isDoorUnlocked = false;

    void Start()
    {
        // Find backgrounds if not assigned
        if (Level5BGwithdoorClosed == null)
        {
            Level5BGwithdoorClosed = GameObject.Find("Level5BGwithdoorClosed");
        }
        if (Level5BGwithdoorOpened == null)
        {
            Level5BGwithdoorOpened = GameObject.Find("Level5BGwithdoorOpened");
        }
        
        // Ensure correct initial states
        if (Level5BGwithdoorClosed != null) Level5BGwithdoorClosed.SetActive(true);
        if (Level5BGwithdoorOpened != null) Level5BGwithdoorOpened.SetActive(false);
    }

    // private void OnTriggerStay2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player") && !isTerminalOpen && !isDoorUnlocked)
    //     {
    //         if (Input.GetKeyDown(KeyCode.E))
    //         {
    //             Debug.Log("E pressed - attempting to spawn terminal");
    //             isTerminalOpen = true;
    //             currentTerminal = Instantiate(terminalPrefab);
                
    //             var interpreter = currentTerminal.GetComponent<InterpreterLvl5>();
    //             if (interpreter == null)
    //             {
    //                 interpreter = currentTerminal.GetComponentInChildren<InterpreterLvl5>();
    //             }
                
    //             if (interpreter != null)
    //             {
    //                 interpreter.SetReferences(guardPatrol, this);
    //             }
    //         }
    //     }
    // }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTerminalOpen && !isDoorUnlocked)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E pressed - spawning terminal");
                SpawnTerminal();
            }
        }
    }
    private void SpawnTerminal()
    {
        // Mark the terminal as open to prevent multiple instances
        isTerminalOpen = true;

        // Instantiate the terminal prefab in the scene
        currentTerminal = Instantiate(terminalPrefab);

        // Attempt to get the InterpreterLvl5 component from the terminal prefab
        // If not found on the root object, check its children
        var interpreter = currentTerminal.GetComponent<InterpreterLvl5>() ?? currentTerminal.GetComponentInChildren<InterpreterLvl5>();

        // If the interpreter component is found, set necessary references
        if (interpreter != null)
        {
            // Pass references to the guard patrol and this object for further interaction
            interpreter.SetReferences(guardPatrol, this);
        }
        else
        {
            // Log an error if the interpreter component is missing, as it is critical for functionality
            Debug.LogError("InterpreterLvl5 component not found on terminal prefab!");
        }
    }
    private void Update()
    {
        if (isTerminalOpen && isDoorUnlocked)
        {
            CloseTerminal();
        }
    }

    public void CloseTerminal()
    {
        if (currentTerminal != null)
        {
            Destroy(currentTerminal);
        }
        isTerminalOpen = false;
        Time.timeScale = 1f;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isTerminalOpen)
        {
            CloseTerminal();
        }
    }

    public void UnlockDoor()
    {
        Debug.Log("UnlockDoor called - checking background objects");
        Debug.Log($"Level5BGwithdoorClosed null? {Level5BGwithdoorClosed == null}");
        Debug.Log($"Level5BGwithdoorOpened null? {Level5BGwithdoorOpened == null}");

        isDoorUnlocked = true;
        if (exitBlocker != null)
        {
            exitBlocker.SetActive(false);
        }

        if (Level5BGwithdoorClosed != null && Level5BGwithdoorOpened != null)
        {
            Level5BGwithdoorClosed.SetActive(false);
            Level5BGwithdoorOpened.SetActive(true);
            Debug.Log("Direct background switch completed");
        }
        else
        {
            Debug.LogError("Background GameObjects not found!");
        }

        StartCoroutine(SwitchBackgrounds());
    }

    private System.Collections.IEnumerator SwitchBackgrounds()
    {
        Debug.Log("Starting background switch");

        if (Level5BGwithdoorClosed == null || Level5BGwithdoorOpened == null)
        {
            Debug.LogError("Background references missing in coroutine!");
            yield break;
        }

        GameObject overlay = new GameObject("FadeOverlay");
        overlay.transform.parent = transform;
        SpriteRenderer fadeRenderer = overlay.AddComponent<SpriteRenderer>();
        fadeRenderer.color = new Color(0, 0, 0, 0);
        fadeRenderer.sortingOrder = 999;
        
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            yield break;
        }
        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;

        // Position overlay relative to camera
        fadeRenderer.transform.localScale = new Vector3(width, height, 1);
        fadeRenderer.transform.position = new Vector3(mainCamera.transform.position.x, 
                                                    mainCamera.transform.position.y, 
                                                    fadeRenderer.transform.position.z);

        // Fade to black
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.unscaledDeltaTime * 2;
            fadeRenderer.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Switch backgrounds
        Level5BGwithdoorClosed.SetActive(false);
        Level5BGwithdoorOpened.SetActive(true);
        Debug.Log($"Backgrounds switched in coroutine - closed: {Level5BGwithdoorClosed.activeSelf}, opened: {Level5BGwithdoorOpened.activeSelf}");

        while (alpha > 0)
        {
            alpha -= Time.unscaledDeltaTime * 2;
            fadeRenderer.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        Destroy(overlay);
        Debug.Log("Background switch complete");
    }
}