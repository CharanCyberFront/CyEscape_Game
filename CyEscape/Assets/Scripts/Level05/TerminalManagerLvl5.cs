using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class TerminalManagerLvl5 : MonoBehaviour
{
    public GameObject directoryLine;
    public GameObject responseLine;
    public TMP_InputField terminalInput;
    public GameObject userInputLine;
    public ScrollRect sr;
    public GameObject msgList;
    public int lines = 0;

    // Command history functionality
    private List<string> commandHistory = new List<string>();
    private int historyIndex;
    private string currentInputBuffer = "";

    // Tab completion functionality
    private readonly string[] availableCommands = new string[] 
    {
        "help",
        "nmap -p-",
        "ssh root@192.168.1.42",
        "unlock guard",
        "whoami",
        "ls",
        "toor"
    };
    private int tabIndex = -1;
    private string tabSearchBuffer = "";

    InterpreterLvl5 interpreter;

    private void Start()
    {
        interpreter = GetComponent<InterpreterLvl5>();
        historyIndex = 0;
        // Add input field navigation handler
        terminalInput.onValueChanged.AddListener(OnInputValueChanged);
    }

    private void OnInputValueChanged(string newValue)
    {
        // Reset tab completion when input changes
        tabIndex = -1;
        tabSearchBuffer = newValue;
    }

    private void Update()
    {
        if (!terminalInput.isFocused) return;

        // Handle command history (Up/Down arrows)
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            NavigateHistory(-1); // Go back in history
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            NavigateHistory(1);  // Go forward in history
        }
        // Handle tab completion
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            HandleTabCompletion();
        }
    }

    private void OnGUI()
    {
        terminalInput.enabled = true;
        terminalInput.ActivateInputField();
        
        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            string userInput = terminalInput.text;
            
            // Add command to history
            if (!string.IsNullOrWhiteSpace(userInput))
            {
                commandHistory.Add(userInput);
            }
            
            // Reset history index to allow immediate access to the last command
            historyIndex = commandHistory.Count;

            ClearInputField();
            AddDirectoryLine(userInput);
            AddInterpreterLines(interpreter.Interpret(userInput));
            lines++;
            ScrolltoBottom(lines);
            userInputLine.transform.SetAsLastSibling();
            terminalInput.ActivateInputField();

            // Store the empty input as current buffer
            currentInputBuffer = "";
        }
    }

    private void NavigateHistory(int direction)
    {
        if (commandHistory.Count == 0) return;

        // If we're at the newest position, store the current input
        if (historyIndex == commandHistory.Count)
        {
            currentInputBuffer = terminalInput.text;
        }

        // Update the index
        historyIndex = Mathf.Clamp(historyIndex + direction, 0, commandHistory.Count);

        // Set the text
        if (historyIndex == commandHistory.Count)
        {
            // At the newest position, show the current buffer
            terminalInput.text = currentInputBuffer;
        }
        else
        {
            // Show the historical command
            terminalInput.text = commandHistory[historyIndex];
        }

        // Move caret to end
        terminalInput.caretPosition = terminalInput.text.Length;
        terminalInput.selectionAnchorPosition = terminalInput.caretPosition;
    }

    private void HandleTabCompletion()
    {
        string currentInput = terminalInput.text;
        
        // Get matching commands
        var matches = availableCommands
            .Where(cmd => cmd.StartsWith(currentInput, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matches.Count == 0) return;

        // Reset tab index if input changed
        if (currentInput != tabSearchBuffer)
        {
            tabIndex = -1;
            tabSearchBuffer = currentInput;
        }

        // Increment tab index
        tabIndex = (tabIndex + 1) % matches.Count;

        // Update input field
        terminalInput.text = matches[tabIndex];
        terminalInput.caretPosition = terminalInput.text.Length;
        terminalInput.selectionAnchorPosition = terminalInput.caretPosition;
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
        foreach (string line in interpretation)
        {
            GameObject res = Instantiate(responseLine, msgList.transform);
            res.transform.SetAsLastSibling();

            Vector2 listSize = msgList.GetComponent<RectTransform>().sizeDelta;
            msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(listSize.x, listSize.y + 35.0f);

            res.GetComponentInChildren<TMP_Text>().text = line;
        }
        return interpretation.Count;
    }

    void ScrolltoBottom(int lines)
    {
        sr.velocity = new Vector2(0, 250);
    }
}