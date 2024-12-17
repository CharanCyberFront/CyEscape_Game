using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;

class User
{
    public string username { get; set; }
    public string password { get; set; }
    public string security { get; set; }
}

class File
{
    public string filename { get; set; }
    public string Priveleges { get; set; }
    public string Data { get; set; }
    public string creator { get; set; }
    public string DateCreated { get; set; }
}

public class InterpreterLvl3 : MonoBehaviour
{
    private InteractableLevel3 interactable; // Reference to the associated InteractableLevel3
    Dictionary<string, string> colors = new Dictionary<string, string>()
    {
        {"blue", "#08d9d6"},
        {"green", "#444444"},
        {"yellow", "#FFFF00"}
    };

    Dictionary<string, List<string>> directories = new Dictionary<string, List<string>>()
    {
        { "/", new List<string> { "trials21" } },
        { "/trials21", new List<string> { "passwords.txt", "web.net", "experiments.png", "mirailabs.txt", "strings" } }
    };

    List<User> users = new List<User>();
    List<File> files = new List<File>();

    //private int hintnumber = 0;

    private string currentUser = "guest";
    private string currentDirectory = "/";
    List<string> response = new List<string>();

    string[] hints = { "0", "1", "2", "3", "4", "5" };

    public List<string> Interpret(string userInput)
    {
        if (files.Count == 0)
        {
            generateFiles();
        }
        response.Clear();
        ExecuteCommand(userInput);
        return response;
    }

    public void generateFiles()
    {
        createNewFile("passwords.txt", "root: d3a87256ed7203166184d983249a9669", "-rw-r--r--", "root", "Oct 29 10:32");
        createNewFile("experiments.png", "", "-rw-r-----", "root", "Oct 29 10:32");
        createNewFile("web.net", "", "-rw-r-----", "root", "Oct 29 10:32");
        createNewFile("mirailabs.txt", "", "-rw-r-----", "root", "Oct 29 10:32");
        createNewFile("strings", "safe, secure, corporate, obey, your, boss, no, questions, asked", "-rw-r--r-x", "root", "Oct 29 10:32");
        createNewUser("root", "security6794", "admin");
        createNewUser("guest", "guestuser", "other");

hints[0] = "This command shows file permissions. For example, -rw-r--r-- means:\n'r' = read, 'w' = write, 'x' = execute, and '-' = no access.\nFirst group: admin, second: group, third: everyone else.\nHmm, I don’t seem to have much access. Time to find a way to level up my privileges!";
hints[1] = "Looks like a password hash! Hashes are one-way (no reverse magic here).\nI’ll need a cracking tool. Let’s try running view-tools to see what I can use.";
hints[2] = "Password cracked! Let’s get superuser access. Run sudo -s and use your new password.\nThis should give me full control. Let’s do this!";
hints[3] = "Security nerd alert: steganography time! Images can hide secrets.\nCheck out experiments.png. Try running strings <file> on it—I’ve got a hunch something’s hidden there.";
hints[4] = "You’re doing great! Stay sharp and keep exploring. The secrets won’t hide themselves.";
hints[5] = "";

        for (int i = 1; i < 6; i++)
        {
            hints[i] = ColorString(hints[i], "yellow");

        }

        // Welcome message when the terminal boots
        response.Add("Welcome to the Trials21 Terminal.");
        response.Add("Type 'cd trials21' to access the Trials21 directory.");
    }

    // Helper to color strings for terminal output
    public string ColorString(string s, string color)
    {
        string leftTag = "<color=" + color + ">";
        string rightTag = "</color>";
        return leftTag + s + rightTag;
    }


    void ExecuteCommand(string command)
    {

        if (command.EndsWith("\t")) // Detect tab key
        {
            string partialInput = command.TrimEnd('\t'); // Remove the tab character
            string completed = TabComplete(partialInput);
            response.Add(completed);
            return;
        }

        if (command.ToLower() == "help")
        {
            response.Add("All Commands:");
            response.Add("whoami");
            response.Add("Print working directory.");
            response.Add("cd <directory>");
            response.Add("Change the current working directory.");
            response.Add("");
            response.Add("sudo -s");
            response.Add("Escalates privilege to login as the root user.");
            response.Add("");
            response.Add("ls");
            response.Add("List files in the current directory.");
            response.Add("");
            response.Add("ls -l");
            response.Add("List files with detailed information.");
            response.Add("");
            response.Add("pwd");
            response.Add("Print the current working directory.");
            response.Add("");
            response.Add("exit");
            response.Add("Exit the terminal.");
            response.Add("");
            response.Add("view-tools");
            response.Add("Displays available tools for hacking.");
            response.Add("");
            response.Add("hashcat -m 0 -a 0 <file> <wordlist>");
            response.Add("Cracks password hashes using a wordlist.");
        }
        else if (command.ToLower() == "whoami")
        {
            response.Add(currentUser);
        }
        else if (command.ToLower().StartsWith("ls"))
        {
            string[] parts = command.Split(' ');
            string targetDirectory = currentDirectory; // Default to current directory
            bool longFormat = false; // Track if -l option is used

            if (currentDirectory == "/trials21" && command == "ls -l")
            {
                print(hints[0]);
            }

            // Parse command arguments
            if (parts.Length > 1)
            {
                if (parts[1] == "-l")
                {
                    longFormat = true;
                }
                else
                {
                    targetDirectory = parts[1];
                    if (parts.Length > 2 && parts[2] == "-l")
                    {
                        longFormat = true;
                    }
                }
            }

            // Check if the directory exists
            string path = targetDirectory.StartsWith("/") ? targetDirectory : (currentDirectory == "/" ? "/" + targetDirectory : currentDirectory + "/" + targetDirectory);
            if (directories.ContainsKey(path))
            {
                List<string> items = directories[path];
                foreach (string item in items)
                {
                    if (longFormat)
                    {
                        // Add detailed information for files
                        bool found = false;
                        foreach (File f in files)
                        {
                            if (f.filename == item)
                            {
                                string details = $"{f.Priveleges} 1 {f.creator} {f.creator} 4096 {f.DateCreated} {f.filename}";
                                response.Add(details);
                                found = true;
                                break;
                            }
                        }

                        // Treat as directory if not found in files
                        if (!found)
                        {
                            string dirDetails = $"drwxr-xr-x 1 root root 4096 {System.DateTime.Now.ToString("MMM dd HH:mm")} {item}";
                            response.Add(dirDetails);
                        }
                    }
                    else
                    {
                        // Simple listing
                        response.Add(item);
                    }
                }
            }
            else
            {
                response.Add("Directory does not exist.");
            }
        }
        else if (command.ToLower() == "pwd")
        {
            response.Add(currentDirectory);
        }
        else if (command.ToLower().StartsWith("cat "))
        {
            string[] parts = command.Split(' ');
            if (parts.Length > 1)
            {
                ReadFile(parts[1]);
            }
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
        else if (command.ToLower() == "view-tools")
        {
            response.Add("Available Tools:");
            response.Add("1. nmap         - Network scanning tool - nmap <ip address>");
            response.Add("2. hashcat      - Password cracking tool - hashcat <filename>");
            response.Add("3. metasploit   - Exploit development framework - metasploit");
            response.Add("4. sqlmap       - SQL injection tool - sqlmap");
            response.Add("5. cipherblast  - Encrypt/decrypt files - cipherblast <filename>");
        }
        else if (command.ToLower().StartsWith("cd "))
        {
            string[] parts = command.Split(' ');
            if (parts.Length > 1)
            {
                string targetDirectory = parts[1];

                if (targetDirectory == "..")
                {
                    // Move to parent directory
                    if (currentDirectory != "/")
                    {
                        int lastSlashIndex = currentDirectory.LastIndexOf('/');
                        currentDirectory = currentDirectory.Substring(0, lastSlashIndex);
                        if (currentDirectory == "")
                        {
                            currentDirectory = "/";
                        }
                        response.Add("Moved to " + currentDirectory);
                    }
                    else
                    {
                        response.Add("Already at the root directory.");
                    }
                }
                else if (directories.ContainsKey(currentDirectory + "/" + targetDirectory))
                {
                    // Navigate to subdirectory
                    currentDirectory = currentDirectory == "/" ? "/" + targetDirectory : currentDirectory + "/" + targetDirectory;
                    response.Add("Moved to " + currentDirectory);
                }
                else if (directories.ContainsKey("/" + targetDirectory) && currentDirectory == "/")
                {
                    // Special case for moving directly from root
                    currentDirectory = "/" + targetDirectory;
                    response.Add("Moved to " + currentDirectory);
                }
                else
                {
                    response.Add("Directory does not exist.");
                }
            }
            else
            {
                response.Add("Usage: cd <directory>");
            }
        }
        // else if (command.ToLower().StartsWith("opendoor"))
        // {
        //     response.Add("Door functionality coming soon...");
        // }
        else if (command.ToLower().StartsWith("hashcat passwords.txt"))
        {
            response.Add("Cracking... Password found: security6794");
            print(hints[2]);
        }
        else if (command.ToLower().StartsWith("hashcat -m 0 -a 0"))
        {
            response.Add("Cracking... Password found: security6794");
            print(hints[2]);
        }
        else if (command.ToLower() == "sudo -s")
        {
            if (currentUser == "root")
            {
                response.Add("You are already root.");
            }
            else
            {
                response.Add("Please provide the root password:");
                currentDirectory = "PasswordPrompt"; // Special state for terminal
            }
        }
        else if (currentDirectory == "PasswordPrompt")
        {
            if (command == GetPass("root")) // Check if input matches root password
            {
                currentUser = "root";
                response.Add("Root access granted. You are now root.");
                currentDirectory = "/";
                print(hints[3]);
            }
            else
            {
                response.Add("Incorrect password.");
                currentDirectory = "/";
            }
        }
        else if (command.ToLower().StartsWith("strings "))
        {
            string[] parts = command.Split(' ');
            if (parts.Length > 1 && parts[1] == "experiments.png")
            {
                response.Add("experiment1 | room#101 | 2021-03-11");
                response.Add("experiment2 | room#102 | 2021-04-05");
                response.Add("...");
                response.Add("experiment23 | room#423 | 2021-09-23");
                response.Add("...");
                response.Add("experiment100 | room#567 | 2024-10-29");
            }
            response.Add(ColorString("Experiment 23...is that me?", "yellow"));
            openDoor();
        }


        else
        {
            response.Add("Command not recognized.");
        }
    }
    public void SetInteractable(InteractableLevel3 interactableScript)
    {
        interactable = interactableScript; // Link the Interactable script
    }

    private void ReadFile(string fileName)
    {
        foreach (File f in files)
        {
            if (f.filename == fileName)
            {
                if ((currentUser == "root") || (f.Priveleges[7] == 'r' && currentUser == "guest"))
                {
                    response.Add(f.Data);
                    if (f.Data == "root: d3a87256ed7203166184d983249a9669")
                    {
                        print(hints[1]);
                    }
                }
                else
                {
                    response.Add("You do not have permission to read this file.");
                }
                return;
            }
        }
        response.Add("File does not exist.");
    }

    // public void openDoor()
    // {
    //     response.Add("Door Successfully Opened!");
    //     GameObject door = GameObject.Find("door");
    //     DoorIsOpen doorOpenScript = door.GetComponent<DoorIsOpen>();
    //     doorOpenScript.enabled = true;
    // }

    public void openDoor()
    {
        // Find the Door GameObject
        GameObject door = GameObject.Find("door");
        if (door != null)
        {
            // Get the DoorIsOpen script
            DoorIsOpen doorOpenScript = door.GetComponent<DoorIsOpen>();
            if (doorOpenScript != null)
            {
                // Mark the terminal challenge as complete
                doorOpenScript.CompleteTerminalChallenge();
                doorOpenScript.enabled = true; // Enable the script if needed
                response.Add("Door Successfully Opened!");
                Debug.Log("Door opened successfully.");
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

    public void createNewUser(string username, string password, string priveleges)
    {
        User u = new User();
        u.security = priveleges;
        u.username = username;
        u.password = password;
        users.Add(u);
    }

    public void createNewFile(string filename, string data, string priveleges, string creator, string DateCreated)
    {
        File f = new File();
        f.Priveleges = priveleges;
        f.filename = filename;
        f.Data = data;
        f.creator = creator;
        f.DateCreated = DateCreated;
        files.Add(f);
    }

    public string GetPass(string username)
    {
        foreach (User u in users)
        {
            if (u.username == username)
            {
                return u.password;
            }
        }
        return "";
    }
    public void print(string message)
    {
        string[] balls = message.Split('\n');
        foreach (string b in balls)
        {
            string s = ColorString(b, "yellow");
            response.Add(s);
        }
    }
    public string TabComplete(string partialInput)
    {
        List<string> possibleMatches = new List<string>();

        int option = 0;

        // Add possible commands
        string[] commands = { "help", "ls", "ls -l", "cat", "sudo -s", "view-tools", "hashcat", "strings" };

        string[] arr = partialInput.Split(" ");
        if (arr.Length == 1)
        {
            possibleMatches.AddRange(commands);
            option = 1;
        }
        else if (arr.Length >= 2 && arr[1].Contains("-"))
        {
            possibleMatches.AddRange(commands);
            option = 2;
        }
        else if (arr.Length >= 2 && !arr[1].Contains("-") && directories.ContainsKey(currentDirectory))
        {
            possibleMatches.AddRange(directories[currentDirectory]);
            string[] arr2 = possibleMatches.ToArray();
            option = 3;
        }

        var matches = possibleMatches.FindAll(cmd => cmd.StartsWith(partialInput));

        // Find matches
        if (option == 3)
        {
            matches = possibleMatches.FindAll(cmd => cmd.StartsWith(arr[arr.Length - 1]));
        }

        // If a single match, return it; if multiple matches, return a hint
        if (matches.Count == 1)
        {
            if (option == 1)
            {
                return matches[0];
            }
            else if (option == 2)
            {
                return matches[0];
            }
            else if (option == 3)
            {
                string s = "";
                for (int i = 0; i < arr.Length - 1; i++)
                {
                    s += arr[i];
                    s += " ";
                }
                s += matches[0];
                return s;
            }
        }
        else if (matches.Count > 1)
        {
            //response.Add("Possible matches: " + string.Join(", ", matches));
            return partialInput; // Leave input unchanged if ambiguous
        }
        else
        {
            return partialInput;
        }
        return partialInput;
    }
}