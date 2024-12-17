using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TerminalManagerLvl3 : MonoBehaviour
{
    public GameObject directoryLine;
    public GameObject responseLine;

    public TMP_InputField terminalInput;
    public TMP_Text directoryText;
    public GameObject userInputLine;
    public ScrollRect sr;
    public GameObject msgList;
    //public RectTransorm content;

    public int lastChild = 4;

    public int totalLines = 0;
    private List<string> commandHistory = new List<string>(); // Stores command history
    private int historyIndex = -1; // Tracks position in the history

    public bool isLogin = false;

    InterpreterLvl3 interpreter;

    private void Start()
    {
        interpreter = GetComponent<InterpreterLvl3>();
        terminalInput.enabled = true;
        terminalInput.ActivateInputField();
    }
    private void Update()
    {
        HandleCommandHistoryNavigation(); // Check for up/down arrow presses
    }

    private void OnGUI()
    {
        string userInput;

        if (terminalInput.isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            userInput = terminalInput.text;
            terminalInput.text = interpreter.TabComplete(userInput);
            terminalInput.MoveTextEnd(false);
        }

        if (terminalInput.isFocused && Input.GetKeyDown(KeyCode.Escape))
        {
            terminalInput.DeactivateInputField(false);

        }
        if (terminalInput.isFocused == false && !Input.GetKeyDown(KeyCode.Escape) && Input.anyKeyDown && !Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.UpArrow))
        {
            terminalInput.ActivateInputField();
            //RectTransform targetChild = content.GetChild(lastChild).GetComponent<RectTransform>();
            //SmoothScrollToChild(targetChild);
        }
        if (terminalInput.isFocused == false && Input.GetKey(KeyCode.UpArrow))
        {
            sr.velocity = new Vector2(0, -400);
        }
        if (terminalInput.isFocused == false && Input.GetKey(KeyCode.DownArrow))
        {
            sr.velocity = new Vector2(0, 400);
        }

        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            userInput = terminalInput.text;
            ClearInputField();



            if (!isLogin)
            {
                // Add directory line for normal command
                AddDirectoryLine(userInput);
                

                if (userInput == "sudo -s")
                {
                    isLogin = true; // Enter password prompt mode
                    AddInterpreterLines(interpreter.Interpret(userInput));
                    directoryText.text = "Password:"; // Prompt for password
                }
                else
                {
                    // Handle normal commands
                    AddInterpreterLines(interpreter.Interpret(userInput));
                    totalLines++;
                    AddToCommandHistory(userInput);
                }
            }
            else
            {
                // Handle password input
                AddInterpreterLines(interpreter.Interpret(userInput)); // Send input to InterpreterLvl3
                isLogin = false; // Exit login mode
                directoryText.text = "//////>"; // Reset terminal prompt
                totalLines++;

            }

            userInputLine.transform.SetAsLastSibling();
            terminalInput.ActivateInputField();
            terminalInput.Select();
        }
    }

    void ClearInputField()
    {
        terminalInput.text = "";
    }

    void AddDirectoryLine(string userInput)
    {
        Vector2 msgListSize = msgList.GetComponent<RectTransform>().sizeDelta;
        msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(msgListSize.x, msgListSize.y + 35.0f);

        GameObject msg = Instantiate(directoryLine, msgList.transform);
        msg.transform.SetSiblingIndex(msgList.transform.childCount - 1);
        msg.GetComponentsInChildren<TMP_Text>()[1].text = userInput;
    }

    int AddInterpreterLines(List<string> interpretation)
    {
        for (int i = 0; i < interpretation.Count; i++)
        {
            GameObject res = Instantiate(responseLine, msgList.transform);
            res.transform.SetAsLastSibling();

            Vector2 listSize = msgList.GetComponent<RectTransform>().sizeDelta;
            msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(listSize.x, listSize.y + 35.0f);

            res.GetComponentInChildren<TMP_Text>().text = interpretation[i];
        }

        lastChild += 2;
        ScrolltoBottom(interpretation.Count + 1);

        return interpretation.Count;
    }

    void ScrolltoBottom(int lines)
    {
        if (lines < 2 && totalLines < 1)
        {

        }
        else
        {
            int j = 95 * lines;
            sr.velocity = new Vector2(0, j);
        }
    }

    // Handle up/down arrow for command history navigation
    private void HandleCommandHistoryNavigation()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && terminalInput.isFocused == true)
        {
            if (historyIndex > 0)
            {
                historyIndex--;
                terminalInput.text = commandHistory[historyIndex];
                terminalInput.caretPosition = terminalInput.text.Length; // Move caret to the end
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && terminalInput.isFocused == true)
        {
            if (historyIndex < commandHistory.Count - 1)
            {
                historyIndex++;
                terminalInput.text = commandHistory[historyIndex];
                terminalInput.caretPosition = terminalInput.text.Length; // Move caret to the end
            }
            else if (historyIndex == commandHistory.Count - 1)
            {
                historyIndex++;
                terminalInput.text = ""; // Clear input field
            }
        }
    }

    // Add command to history
    private void AddToCommandHistory(string command)
    {
        if (!string.IsNullOrWhiteSpace(command))
        {
            commandHistory.Add(command);
        }
        historyIndex = commandHistory.Count; // Reset history index to the end
    }

    /*
    private IEnumerator SmoothScrollTo(Vector2 targetPosition, float duration)
    {
        Vector2 start = sr.normalizedPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            sr.normalizedPosition = Vector2.Lerp(start, targetPosition, elapsedTime / duration);
            yield return null;
        }

        sr.normalizedPosition = targetPosition;
    }

    public void SmoothScrollToChild(RectTransform child, float duration = 0.5f)
    {
        Vector2 targetPosition = GetChildNormalizedPosition(child);
        StartCoroutine(SmoothScrollTo(targetPosition, duration));
    }

    private Vector2 GetChildNormalizedPosition(RectTransform child)
    {
        // Ensure the child is part of the content
        if (!child.IsChildOf(content))
        {
            Debug.LogError("The specified child is not part of the ScrollRect content.");
            return Vector2.zero;
        }

        Vector2 viewportSize = sr.viewport.rect.size;
        Vector2 contentSize = content.rect.size;
        Vector2 childPosition = (Vector2)content.InverseTransformPoint(child.position);

        return new Vector2(
            Mathf.Clamp01((childPosition.x + (contentSize.x * 0.5f)) / contentSize.x),
            Mathf.Clamp01((childPosition.y + (contentSize.y * 0.5f)) / contentSize.y)
        );
    }
    */
}