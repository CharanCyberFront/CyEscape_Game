using System.Collections;
using System.Collections.Generic;
using System.IO;
using System; 
using UnityEngine.SceneManagement;
using UnityEngine;

public class InterpreterLvl4 : MonoBehaviour
{
    private InteractableLvl4 interactable; // Reference to the associated InteractableLevel3
    private string currentDirectory = "/";
    private bool isAdmin = false; // Tracks admin privileges
    private bool isDowngraded = false; // Tracks if the terminal has been downgraded
    private bool passwordPromptActive = false; // Tracks if a password prompt is active

    // Simulated directory structure
    private Dictionary<string, List<string>> directories = new Dictionary<string, List<string>>()
    {
        { "/", new List<string> { "success", "failures", "clue.txt", "tests_conducted.txt" } },
        { "/success", new List<string> { "audio_08.wav", "audio_15.wav", "audio_23.wav", "audio_50.wav" } },
        { "/failures", new List<string> { "test02", "test18", "test10", "test23" } }
    };

    private Dictionary<string, string> permissions = new Dictionary<string, string>()
    {
        { "success", "drwx------" },
        { "failures", "drwx------" },
        { "clue.txt", "-r--r--r--" },
        { "tests_conducted.txt", "----------" },
        { "audio_08.wav", "----------" },
        { "audio_15.wav", "----------" },
        { "audio_23.wav", "----------" },
        { "audio_50.wav", "----------" },
        { "test02", "-r--r--r--" },
        { "test18", "-r--r--r--" },
        { "test10", "-r--r--r--" },
        { "test23", "-r--r--r--" }
    };

    // File contents
    private Dictionary<string, string> fileContents = new Dictionary<string, string>()
    {
        { "clue.txt", "To resolve terminal issues, downgrade the terminal:\nRun 'apt-get remove terminal_v2.0' and 'apt-get install terminal_v1.5'." },
        { "tests_conducted.txt", "Experiments have a 10% success rate. So far, only experiment 23 has survived the cyborg procedure and is held captive." },
        { "audio_23.wav", "Playing..." },
        { "test02", "Experiment test02: Failed after 12 hours due to hardware malfunction." },
        { "test18", "Experiment test18: Terminated after subjects displayed violent behavior." },
        { "test10", "Experiment test10: Success rate improved to 20% with modified procedure." },
        { "test23", "Experiment test23: Only survivor of the cyborg procedure, currently in containment." }
    };

    private List<string> response = new List<string>(); // Stores terminal output

    // Color variables
    private const string HintColor = "#FFD700";  // Yellow for hints
    private const string DefaultTextColor = "#FFFFFF";  // White for normal text
    private const string ErrorColor = "#FF0000";  // Red for errors
    private const string SuccessColor = "#00FF00";  // Green for success

    // Main method to process user input
    public List<string> Interpret(string userInput)
    {
        response.Clear();
        ExecuteCommand(userInput.ToLower());
        return response;
    }

    // Formats text with a specified color
    private string FormatWithColor(string text, string color)
    {
        return $"<color={color}>{text}</color>";
    }

    // Executes the given command and generates the appropriate response
    private void ExecuteCommand(string command)
    {
        if (passwordPromptActive)
        {
            HandlePasswordPrompt(command);
            return;
        }

        if (command.EndsWith("\t")) // Detect tab key
        {
            string partialInput = command.TrimEnd('\t'); // Remove the tab character
            string completedCommand = TabComplete(partialInput);
            response.Add(FormatWithColor(completedCommand, DefaultTextColor)); // Display completed command
            return;
        }
        
        string[] parts = command.Split(' ');

        if (command.ToLower() == "help")
        {
            response.Add(FormatWithColor("All Commands:", HintColor));
            response.Add(FormatWithColor("play <audio_file.wav>", DefaultTextColor));
            response.Add(FormatWithColor("    plays audio file out loud.", DefaultTextColor));
            response.Add("");
            response.Add(FormatWithColor("whoami", DefaultTextColor));
            response.Add(FormatWithColor("    Prints the current user identity.", DefaultTextColor));
            response.Add("");
            response.Add(FormatWithColor("pwd", DefaultTextColor));
            response.Add(FormatWithColor("    Print the current working directory.", DefaultTextColor));
            response.Add("");
            response.Add(FormatWithColor("cd <directory>", DefaultTextColor));
            response.Add(FormatWithColor("    Change the current working directory.", DefaultTextColor));
            response.Add("");
            response.Add(FormatWithColor("ls", DefaultTextColor));
            response.Add(FormatWithColor("    List files in the current directory.", DefaultTextColor));
            response.Add("");
            response.Add(FormatWithColor("ls -l", DefaultTextColor));
            response.Add(FormatWithColor("    List files with detailed information.", DefaultTextColor));
            response.Add("");
            response.Add(FormatWithColor("cat <file>", DefaultTextColor));
            response.Add(FormatWithColor("    Display the contents of a file.", DefaultTextColor));
            response.Add("");
            response.Add(FormatWithColor("chmod <permissions> <file/directory>", DefaultTextColor));
            response.Add(FormatWithColor("    Change the file or directory permissions.", DefaultTextColor));
            response.Add(FormatWithColor("    Use symbolic (e.g., +x, -r) or octal (e.g., 755) formats.", DefaultTextColor));
            response.Add("");
            response.Add(FormatWithColor("man chmod", DefaultTextColor));
            response.Add(FormatWithColor("    Displays the manual page for the chmod command.", DefaultTextColor));
            response.Add("");
            response.Add(FormatWithColor("man sql", DefaultTextColor));
            response.Add(FormatWithColor("    Displays the manual page for basic SQL queries.", DefaultTextColor));
            response.Add("");
            response.Add(FormatWithColor("sudo -s", DefaultTextColor));
            response.Add(FormatWithColor("    Escalates privileges to root user (requires password).", DefaultTextColor));
            response.Add("");
            response.Add(FormatWithColor("exit", DefaultTextColor));
            response.Add(FormatWithColor("    Exit the terminal.", DefaultTextColor));
            response.Add("");
        }
        else if (command == "ls" || command == "ls -l")
        {
            ListContents(command == "ls -l");
        }
        else if (command == "pwd")
        {
            response.Add(FormatWithColor(currentDirectory, DefaultTextColor));
        }
        else if (command.ToLower() == "whoami")
        {
            if (isAdmin)
            {
                response.Add(FormatWithColor("root", DefaultTextColor));
            }
            else
            {
                response.Add(FormatWithColor("user", DefaultTextColor));
            }
        }
        else if (command.StartsWith("cd "))
        {
            if (parts.Length > 1) ChangeDirectory(parts[1]);
            else response.Add(FormatWithColor("Usage: cd <directory>", ErrorColor));
        }
        else if (command.StartsWith("cat "))
        {
            if (parts.Length > 1) ReadFile(parts[1]);
            else response.Add(FormatWithColor("Usage: cat <file>", ErrorColor));
        }
        else if (command == "man sql")
        {
            DisplaySQLManPage();
        }
        else if (command == "sudo -s")
        {
            if (!isDowngraded)
            {
                response.Add(FormatWithColor("Permission Denied", ErrorColor));
            }
            else
            {
                response.Add(FormatWithColor("Please provide the root password:", HintColor));
                passwordPromptActive = true;
            }
        }
        else if (command == "apt-get remove terminal_v2.0")
        {
            response.Add(FormatWithColor("Terminal version 2.0 removed.", SuccessColor));
        }
        else if (command == "apt-get install terminal_v1.5")
        {
            response.Add(FormatWithColor("Terminal downgraded to version 1.5.", SuccessColor));
            response.Add(FormatWithColor("Hey, something feels... off here. That terminal’s not as secure as it looks—passwords aren’t well-queried (sql).", HintColor));
            response.Add(FormatWithColor("Maybe there’s a way to climb the ladder. Try aiming for the top... you know, where the root of things lies.", HintColor));
            isDowngraded = true;
        }
        else if (command.StartsWith("chmod"))
        {
            if (isAdmin)
            {
                HandleChmod(command);
            }
            else
            {
                response.Add(FormatWithColor("Permission denied. Admin access required.", ErrorColor));
            }
        }
        else if (command == "man chmod")
        {
            DisplayChmodManPage();
        }
        else if (command.StartsWith("play "))
        {
            string[] playparts = command.Split(' ');

            if (playparts.Length != 2)
            {
                response.Add(FormatWithColor("Usage: play <audio_file>", ErrorColor));
            }
            else
            {
                string audioFile = playparts[1];

                // Check if the file exists and permissions allow access
                if (directories[currentDirectory].Contains(audioFile) && permissions.ContainsKey(audioFile))
                {
                    string filePermission = permissions[audioFile];

                    // Check for read permission
                    if (filePermission[1] == 'r' || (isAdmin && filePermission[4] == 'r'))
                    {
                        // Output the text
                        response.Add(FormatWithColor(fileContents[audioFile], DefaultTextColor));

                        // Play the audio file
                        if (audioFile == "audio_23.wav")
                        {
                            PlayAudioClip(); // Call the function to play the audio clip
                        }

                        TriggerAlarm(); // Trigger the alarm after successful play
                    }
                    else
                    {
                        response.Add(FormatWithColor("Permission denied. You do not have read access to this file.", ErrorColor));
                    }
                }
                else
                {
                    response.Add(FormatWithColor($"Audio file '{audioFile}' not found in the current directory.", ErrorColor));
                }
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
        else if (command.Equals("leave"))
        {
            TriggerAlarm(); 
        }
        else
        {
            response.Add(FormatWithColor("Command not recognized.", ErrorColor));
        }
    }
    public void SetInteractable(InteractableLvl4 interactableScript)
    {
        interactable = interactableScript; // Link the Interactable script
    }

    // private void PlayAudioClip()
    // {
    //     // Find the audio source tagged as "Player_Audio"
    //     GameObject audioObject = GameObject.FindWithTag("Player_Audio");
    //     if (audioObject != null)
    //     {
    //         AudioSource audioSource = audioObject.GetComponent<AudioSource>();
    //         if (audioSource != null)
    //         {
    //             audioSource.Play(); // Play the audio clip
    //             Debug.Log("Playing audio clip: Player_Audio");
    //         }
    //         else
    //         {
    //             Debug.LogError("AudioSource component not found on GameObject tagged as 'Player_Audio'.");
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogError("GameObject tagged as 'Player_Audio' not found in the scene.");
    //     }
    // }

    private void PlayAudioClip()
    {
        // Find the audio source tagged as "Player_Audio"
        GameObject audioObject = GameObject.FindWithTag("Player_Audio");
        if (audioObject != null)
        {
            AudioSource audioSource = audioObject.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play(); // Play the audio clip
                Debug.Log("Playing audio clip: Player_Audio");

                // Start a coroutine to wait for the audio clip to finish
                StartCoroutine(WaitForAudioToFinish(audioSource));
            }
            else
            {
                Debug.LogError("AudioSource component not found on GameObject tagged as 'Player_Audio'.");
            }
        }
        else
        {
            Debug.LogError("GameObject tagged as 'Player_Audio' not found in the scene.");
        }
    }

    // Coroutine to wait for the audio to finish playing before transitioning
    private IEnumerator WaitForAudioToFinish(AudioSource audioSource)
    {
        Debug.Log("Waiting for audio clip to finish...");

        // Wait while the audio is playing
        while (audioSource.isPlaying)
        {
            yield return null; // Wait for the next frame
        }

        // Audio finished playing, transition to the next scene
        Debug.Log("Audio clip finished playing. Transitioning to the next scene.");
        Time.timeScale = 1; // Ensure time scale is reset
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Handle password prompt for sudo -s
    private void HandlePasswordPrompt(string input)
    {
        // Normalize input: Remove extra spaces and make case-insensitive
        input = input.Trim().ToLower();

        // Define valid inputs and normalize them
        string[] validInputs = new string[]
        {
            "select * from users where name = \"root\" and password = '' or '1'='1'",
            "' or '1'='1'",
            "1"
        };

        // Normalize valid inputs for consistent comparison
        for (int i = 0; i < validInputs.Length; i++)
        {
            validInputs[i] = validInputs[i].Trim().ToLower();
        }

        // Check if the input matches any valid input
        if (Array.Exists(validInputs, validInput => input == validInput))
        {
            isAdmin = true;
            response.Add(FormatWithColor("Authentication Successful. Welcome, root User.", SuccessColor));
            response.Add(FormatWithColor("Alright, awesome—we’re admin now, partner! Let’s take a look at those permissions.", HintColor));
            response.Add(FormatWithColor("Try using the chmod command, it should help!", HintColor));
        }
        else
        {
            response.Add(FormatWithColor("Incorrect password.", ErrorColor));
        }

        passwordPromptActive = false;
    }

    // Handle chmod command to change permissions
    private void HandleChmod(string command)
    {
        if (!isAdmin)
        {
            response.Add(FormatWithColor("Permission denied. Admin access required to change permissions.", ErrorColor));
            return;
        }

        string[] parts = command.Split(' ');

        if (parts.Length != 3)
        {
            response.Add(FormatWithColor("Usage: chmod <permissions> <file/directory>", ErrorColor));
            return;
        }

        string permission = parts[1];
        string target = parts[2];

        // Check if target exists
        if (!permissions.ContainsKey(target))
        {
            response.Add(FormatWithColor($"File or directory '{target}' does not exist.", ErrorColor));
            return;
        }

        // Handle symbolic or octal permissions (existing logic)
        if (permission.StartsWith("+") || permission.StartsWith("-"))
        {
            UpdatePermissionsSymbolically(target, permission);
        }
        else if (int.TryParse(permission, out int octal))
        {
            UpdatePermissionsOctally(target, octal);
        }
        else
        {
            response.Add(FormatWithColor("Invalid permissions format. Use symbolic (+/-) or octal (e.g., 755).", ErrorColor));
        }
    }

    // Update permissions using symbolic format
    private void UpdatePermissionsSymbolically(string target, string permission)
    {
        char operation = permission[0]; // '+' or '-'
        char mode = permission[1];      // 'r', 'w', or 'x'

        string currentPerms = permissions[target];
        char[] updatedPerms = currentPerms.ToCharArray();

        // Map for symbolic permissions
        Dictionary<char, int[]> permissionIndices = new Dictionary<char, int[]>()
        {
            { 'r', new[] { 1, 4, 7 } }, // Read
            { 'w', new[] { 2, 5, 8 } }, // Write
            { 'x', new[] { 3, 6, 9 } }  // Execute
        };

        if (!permissionIndices.ContainsKey(mode))
        {
            response.Add(FormatWithColor("Invalid permission type. Use 'r', 'w', or 'x'.", ErrorColor));
            return;
        }

        // Apply the operation to all relevant indices
        foreach (int index in permissionIndices[mode])
        {
            updatedPerms[index] = (operation == '+') ? mode : '-';
        }

        permissions[target] = new string(updatedPerms);
        response.Add(FormatWithColor($"Permissions updated: {target} now has {permissions[target]}.", SuccessColor));
    }

    // Update permissions using octal format
    private void UpdatePermissionsOctally(string target, int octal)
    {
        if (octal < 0 || octal > 777)
        {
            response.Add(FormatWithColor("Invalid octal value. Use values between 000 and 777.", ErrorColor));
            return;
        }

        string newPerms = "d";
        string[] rwxMapping = { "---", "--x", "-w-", "-wx", "r--", "r-x", "rw-", "rwx" };

        for (int i = 0; i < 3; i++)
        {
            int digit = (octal / (int)Mathf.Pow(10, 2 - i)) % 10;
            newPerms += rwxMapping[digit];
        }

        permissions[target] = newPerms;
        response.Add(FormatWithColor($"Permissions updated: {target} now has {permissions[target]}.", SuccessColor));
    }

    // List contents of the current directory
    private void ListContents(bool longFormat)
    {
        if (!directories.ContainsKey(currentDirectory))
        {
            response.Add(FormatWithColor("Directory does not exist.", ErrorColor));
            return;
        }

        foreach (string item in directories[currentDirectory])
        {
            if (longFormat)
            {
                string perms = permissions.ContainsKey(item) ? permissions[item] : "----------";
                response.Add(FormatWithColor($"{perms} root root 4096 Oct 29 {item}", DefaultTextColor));
            }
            else
            {
                response.Add(FormatWithColor(item, DefaultTextColor));
            }
        }
    }

    // Display the SQL man page
    private void DisplaySQLManPage()
    {
        response.Add(FormatWithColor("SQL Manual - Basic Queries:", DefaultTextColor));
        response.Add(FormatWithColor("1. select * from table_name; - Fetch all rows from a table.", DefaultTextColor));
        response.Add(FormatWithColor("2. select column1, column2 from table_name where condition; - Fetch specific columns that meet a condition.", DefaultTextColor));
        response.Add(FormatWithColor("3. sql injection example 1: '' or '1'='1'", ErrorColor));
        response.Add(FormatWithColor("4. sql injection example 2: select * from users where name = \"john doe\" and password = \"myp@ss\";", ErrorColor));
        response.Add(FormatWithColor("\nIt seems ' or '1'='1 always makes a condition true no matter what.", HintColor));
        response.Add(FormatWithColor("It sneaks in an or that says, 'If this fails, 1 still equals 1.'", HintColor));
        //response.Add(FormatWithColor("What user table could we explore, partner? Maybe start by fetching data FROM the Users table. But which user are we trying to access?", HintColor));
    }
    
    // Display the chmod man page
    private void DisplayChmodManPage()
    {
        response.Add(FormatWithColor("NAME", HintColor));
        response.Add(FormatWithColor("    chmod - change file mode bits", DefaultTextColor));

        response.Add(FormatWithColor("SYNOPSIS", HintColor));
        response.Add(FormatWithColor("    chmod [mode] file", DefaultTextColor));
        response.Add(FormatWithColor("    chmod 755 file", DefaultTextColor));
        response.Add(FormatWithColor("    chmod +x file", DefaultTextColor));
        response.Add(FormatWithColor("    chmod -r file", DefaultTextColor));

        response.Add(FormatWithColor("DESCRIPTION", HintColor));
        response.Add(FormatWithColor("    chmod changes the file permissions for the specified file or directory.", DefaultTextColor));
        response.Add(FormatWithColor("    Permissions can be set in symbolic (+/-) or octal (e.g., 755) formats.", DefaultTextColor));

        response.Add(FormatWithColor("EXAMPLES", HintColor));
        response.Add(FormatWithColor("    chmod 755 file          - Set read, write, execute for owner, and read, execute for group/others.", DefaultTextColor));
        response.Add(FormatWithColor("    chmod +x file           - Add execute permission.", DefaultTextColor));
        response.Add(FormatWithColor("    chmod -r file           - Remove read permission.", DefaultTextColor));
    }

    // Change directory
    private void ChangeDirectory(string target)
    {
        string newPath = target.StartsWith("/") ? target : (currentDirectory == "/" ? "/" + target : currentDirectory + "/" + target);

        // Handle ".." to move up a directory
        if (target == "..")
        {
            if (currentDirectory != "/")
            {
                int lastSlash = currentDirectory.LastIndexOf('/');
                currentDirectory = lastSlash > 0 ? currentDirectory.Substring(0, lastSlash) : "/";
                response.Add(FormatWithColor($"Moved to {currentDirectory}", SuccessColor));
            }
            else
            {
                response.Add(FormatWithColor("Already at root directory.", ErrorColor));
            }
        }
        // Check if the newPath is a valid directory
        else if (directories.ContainsKey(newPath))
        {
            // Check if the target directory has execute permission ("x")
            if (permissions.ContainsKey(target) && permissions[target][3] == 'x')
            {
                currentDirectory = newPath;
                response.Add(FormatWithColor($"Moved to {currentDirectory}", SuccessColor));
            }
            else
            {
                response.Add(FormatWithColor($"Permission denied: Cannot access {target}", ErrorColor));
            }
        }
        else
        {
            response.Add(FormatWithColor("Directory does not exist.", ErrorColor));
        }
    }

    // Read file contents
    private void ReadFile(string fileName)
    {
        if (directories.ContainsKey(currentDirectory) && directories[currentDirectory].Contains(fileName))
        {
            string filePermission = permissions.ContainsKey(fileName) ? permissions[fileName] : "----------";
            if (filePermission[1] == 'r' || filePermission[4] == 'r' || filePermission[7] == 'r')
            {
                if (fileContents.ContainsKey(fileName))
                {
                    string content = fileContents[fileName];
                    foreach (string line in content.Split('\n')) // Split content into lines
                    {
                        response.Add(FormatWithColor(line, DefaultTextColor));
                    }
                }
                else
                {
                    response.Add(FormatWithColor("File is empty.", DefaultTextColor));
                }
            }
            else
            {
                response.Add(FormatWithColor("Permission Denied", ErrorColor));
            }
        }
        else
        {
            response.Add(FormatWithColor("File does not exist.", ErrorColor));
        }
    }

    // Method to trigger the alarm
    private void TriggerAlarm()
    {
        // Find the alarm GameObject
        GameObject alarmObject = GameObject.Find("Alarm");

        if (alarmObject != null)
        {
            // Access the Animator component and set the Alarm parameter to true
            Animator animator = alarmObject.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("Alarm", true); // Set the Alarm parameter
                response.Add(FormatWithColor("Alarm Triggered! The Facility Is In Lockdown!", SuccessColor));

                // Play the alarm sound effect
                PlayAlarmSound();
                
                // Immediately destroy the terminal and load the next scene
                //Destroy(gameObject);
                //Time.timeScale = 1; //Transition To Level 5 
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                response.Add(FormatWithColor("Error: Alarm Animator not found!", ErrorColor));
            }
        }
        else
        {
            response.Add(FormatWithColor("Error: Alarm object not found!", ErrorColor));
        }
    }

    //Tab Complete Function
    public string TabComplete(string partialInput)
    {
        List<string> possibleMatches = new List<string>();

        // Define possible commands
        string[] commands = { "help", "ls", "ls -l", "cat", "sudo -s", "chmod", "play", "cd", "exit" };

        // Split input into parts
        string[] parts = partialInput.Split(' ');

        if (parts.Length == 1)
        {
            // If only the command is being typed, suggest from the command list
            possibleMatches.AddRange(commands);
        }
        else if (parts.Length == 2)
        {
            // If command + argument is typed, suggest files or directories
            string command = parts[0];
            if (command == "cat" || command == "play" || command == "chmod" || command == "cd")
            {
                // Suggest files/directories in the current directory
                if (directories.ContainsKey(currentDirectory))
                {
                    possibleMatches.AddRange(directories[currentDirectory]);
                }
            }
        }
        else
        {
            // Handle cases with more arguments (e.g., `chmod 755 <file>`)
            return partialInput; // Leave unchanged for now
        }

        // Get the last part of the input (the part being completed)
        string lastPart = parts[parts.Length - 1];

        // Find matches that start with the last part
        var matches = possibleMatches.FindAll(item => item.StartsWith(lastPart));

        if (matches.Count == 1)
        {
            // If exactly one match, auto-complete the input
            parts[parts.Length - 1] = matches[0];
            return string.Join(" ", parts);
        }
        else if (matches.Count > 1)
        {
            // If multiple matches, display them as suggestions
            response.Add(FormatWithColor("Possible matches: " + string.Join(", ", matches), HintColor));
            return partialInput; // Keep input unchanged
        }
        else
        {
            // No matches found
            response.Add(FormatWithColor("No matches found.", ErrorColor));
            return partialInput;
        }
    }

    // Method to play the alarm sound effect
    private void PlayAlarmSound()
    {
        // Find the GameObject tagged as AlarmSound
        GameObject alarmSoundObject = GameObject.FindWithTag("AlarmSound");

        if (alarmSoundObject != null)
        {
            // Get the AudioSource component and play the sound
            AudioSource audioSource = alarmSoundObject.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
                Debug.Log("Playing alarm sound effect.");
            }
            else
            {
                Debug.LogError("AudioSource component not found on the GameObject tagged as 'AlarmSound'.");
            }
        }
        else
        {
            Debug.LogError("GameObject tagged as 'AlarmSound' not found in the scene.");
        }
    }
}