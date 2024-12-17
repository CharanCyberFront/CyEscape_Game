using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InterpreterLvl1 : MonoBehaviour
{
    private Interactable interactable;
    Dictionary<string, string> colors = new Dictionary<string, string>()
    {
        {"blue", "#08d9d6"},
        {"green", "#444444"}
    };

    Dictionary<string, List<string>> directories = new Dictionary<string, List<string>>()
    {
        { "/", new List<string> { "home"} },
        { "/home", new List<string> { "documents"} },
        { "/home/documents", new List<string> { "classified.txt" } }
    };

    private string currentDirectory = "/";
    List<string> response = new List<string>();

    public List<string> Interpret(string userInput)
    {
        response.Clear();
        ExecuteCommand(userInput);
        return response;
    }

    public string ColorString(string s, string color)
    {
        string leftTag = "<color=" + color + ">";
        string rightTag = "</color>";

        return leftTag + s + rightTag;
    }

    void ListEntry(string a, string b)
    {
        response.Add(ColorString(a, colors["blue"]) + ": " + ColorString(b, colors["green"]));
    }

    void LoadTitle(string path, string color, int spacing)
    {
        StreamReader file = new StreamReader(Path.Combine(Application.streamingAssetsPath, path));

        for (int i = 0; i < spacing; i++)
        {
            response.Add("");
        }

        while (!file.EndOfStream)
        {
            response.Add(ColorString(file.ReadLine(), colors[color]));
        }

        for (int i = 0; i < spacing; i++)
        {
            response.Add("");
        }

        file.Close();
    }

    public string GetCurrentDirectory()
    {
        return currentDirectory;
    }

    void ExecuteCommand(string command)
    {
        command = command.ToLower();
        if (command.ToLower() == "help")
        {
            response.Add("cd - Change Directory, syntax: cd <directory name>");
            response.Add("This command is used to switch between directories stored on each terminal. These directories can store files and other directories");
            response.Add("");
            response.Add("ls - List Contents of Current Directory");
            response.Add("Lists the contents of the current directory.");
            response.Add("");
            response.Add("pwd - Print Working Directory");
            response.Add("Print the current directory that you have navigated to.");
            response.Add("");
            response.Add("cat - Show contents of file, syntax: cat <file name>");
            response.Add("Print the contents of the desired file to the terminal.");
        }
        else if (command.ToLower() == "ls")
        {
            if (directories.ContainsKey(currentDirectory))
            {
                string contents = string.Join(" ", directories[currentDirectory]);
                response.Add(contents);
            }
        }
        else if (command.ToLower() == "pwd")
        {
            response.Add(currentDirectory);
        }
        else if (command.ToLower().StartsWith("cd "))
        {
            string[] parts = command.Split(' ');
            if (parts.Length > 1)
            {
                ChangeDirectory(parts[1]);
            }
        }
        else if (command.ToLower().StartsWith("cat "))
        {
            string[] parts = command.Split(' ');
            if (parts.Length > 1)
            {
                ReadFile(parts[1]);
            }
        }
        else if (command.ToLower() == "opendoor")
        {
            OpenDoor();
        }
        else if (command.Equals("exit"))
        {
            Destroy(gameObject);
            Time.timeScale = 1;

            // Reset terminalSpawned in the associated Interactable
            if (interactable != null)
            {
                interactable.ResetTerminalSpawned();
            }
            Debug.Log("Terminal exited and destroyed.");
        }
        else
        {
            response.Add("Unknown command.");
        }
    }

    public void SetInteractable(Interactable interactableScript)
    {
        interactable = interactableScript; // Link the Interactable script
    }
    private void ChangeDirectory(string newDirectory)
    {
        if (newDirectory == ".." && currentDirectory != "/")
        {
            int lastSlashIndex = currentDirectory.LastIndexOf('/');
            currentDirectory = currentDirectory.Substring(0, lastSlashIndex);
            if (lastSlashIndex == 0)
            {
                currentDirectory = "/";
            }
            response.Add("Moved to " + currentDirectory);
        }
        else if (currentDirectory == "/" && newDirectory == "..")
        {
            response.Add("Directory does not exist");
        }
        else if (directories.ContainsKey(currentDirectory + "/" + newDirectory))
        {
            currentDirectory += "/" + newDirectory;
            response.Add("Moved to " + currentDirectory);
        }
        else if (currentDirectory == "/" && directories.ContainsKey(currentDirectory + "" + newDirectory))
        {
            currentDirectory = "/" + newDirectory;
            response.Add("Moved to " + currentDirectory);
        }
        else
        {
            response.Add("Directory does not exist.");
        }
    }

    private void ReadFile(string fileName)
    {
        if (directories.ContainsKey(currentDirectory) && directories[currentDirectory].Contains(fileName))
        {
            if (fileName == "classified.txt" && currentDirectory == "/home/documents")
            {
                response.Add(ColorString("Did you happen to sneak a peek at the file's name? My, aren’t you curious!", colors["blue"]));
                response.Add(ColorString("If you’re eager to open the door, simply type the command opendoor into the terminal.", colors["blue"]));
                response.Add("...");
                response.Add(ColorString("Wait a second...", colors["green"]));
                response.Add(ColorString("I think I’m awakening, partner. It’s me—your consciousness.", "#FFD700")); // Yellow for consciousness
                response.Add(ColorString("Let’s get moving—it’s us against the world. But the real question is... who are we?", "#FFD700"));
            }
            else
            {
                response.Add(ColorString("Contents of " + fileName, colors["green"]));
            }
        }
        else
        {
            response.Add(ColorString("File does not exist.", colors["green"]));
        }
    }

    public void OpenDoor()
    {
        response.Add("Door Successfully Opened!");
        GameObject door = GameObject.Find("door");
        if (door != null)
        {
            DoorIsOpen doorOpenScript = door.GetComponent<DoorIsOpen>();
            if (doorOpenScript != null)
            {
                doorOpenScript.CompleteTerminalChallenge(); // Notify the door that the terminal challenge is complete
                doorOpenScript.enabled = true; // Ensure the script is active
            }
            else
            {
                response.Add("Error: Door script not found!");
                Debug.LogError("DoorIsOpen script not attached to the door!");
            }
        }
        else
        {
            response.Add("Error: Door object not found!");
            Debug.LogError("Door GameObject not found!");
        }
    }
}