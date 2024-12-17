using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InterpreterLvl5 : MonoBehaviour
{
    private GuardPatrolLvl5 guardPatrol;
    private InteractableLvl5 interactable;
    private List<string> response = new List<string>();

    private bool guardHacked = false;
    private bool waitingForPassword = false;
    private string guardIP = "192.168.1.42";

    private bool sshConnected = false;
    private List<string> randomFiles;

    void Start()
    {
        response = new List<string>();
        
        if (guardPatrol == null)
        {
            guardPatrol = GameObject.FindObjectOfType<GuardPatrolLvl5>();
            Debug.Log("Found guard patrol: " + (guardPatrol != null));
        }

        randomFiles = new List<string>();
    }

    public void SetReferences(GuardPatrolLvl5 guard, InteractableLvl5 interact)
    {
        guardPatrol = guard;
        interactable = interact;
    }

    public List<string> Interpret(string userInput)
    {
        response.Clear();
        ExecuteCommand(userInput.Trim().ToLower());
        return response;
    }

    void ExecuteCommand(string command)
    {
        if (waitingForPassword)
        {
            if (command == "toor")
            {
                response.Add("Access granted. Guard systems compromised.");
                guardHacked = true;
                sshConnected = true;
                GenerateRandomFiles();
            }
            else
            {
                response.Add("Access denied. Invalid password.");
            }
            waitingForPassword = false;
            return;
        }

        if (command.Equals("exit"))
        {
            Destroy(gameObject);
            Time.timeScale = 1;
            return;
        }

        // Handle sleep command
        if (command.StartsWith("sleep"))
        {
            if (guardHacked)
            {
                string[] parts = command.Split(' ');
                if (parts.Length == 2 && int.TryParse(parts[1], out int seconds))
                {
                    if (seconds > 0 && seconds <= 10) // Limit sleep time to reasonable values
                    {
                        response.Add($"Guard will sleep for {seconds} seconds.");
                        if (guardPatrol != null)
                        {
                            guardPatrol.Sleep(seconds);
                        }
                    }
                    else
                    {
                        response.Add("Sleep duration must be between 1 and 10 seconds.");
                    }
                }
                else
                {
                    response.Add("Usage: sleep <seconds>");
                }
                return;
            }
            else
            {
                response.Add("No control over guard systems.");
                return;
            }
        }

        switch (command)
        {
            case "help":
                ShowHelp();
                break;
            case "nmap -p-":
                response.Add("Scanning network...");
                response.Add("Found device at " + guardIP);
                response.Add("PORT     STATE    SERVICE");
                response.Add("22/tcp   open     ssh");
                break;
            case "ssh root@192.168.1.42":
                response.Add("Enter password: ");
                waitingForPassword = true;
                break;
            case "whoami":
                if (sshConnected)
                {
                    response.Add("guard");
                }
                else
                {
                    response.Add("Not connected to any system.");
                }
                break;
            case "ls":
                if (sshConnected)
                {
                    foreach (string file in randomFiles)
                    {
                        response.Add(file);
                    }
                }
                else
                {
                    response.Add("No files found in the current directory.");
                }
                break;
            case "unlock guard":
                if (guardHacked)
                {
                    response.Add("Commanding guard to unlock cell door...");
                    if (guardPatrol != null)
                    {
                        guardPatrol.HandleTerminalCommand("unlock");
                        StartCoroutine(CloseTerminalWithDelay());
                    }
                }
                else
                {
                    response.Add("No control over guard systems.");
                }
                break;
            default:
                response.Add("Unknown command. Type 'help' for available commands.");
                break;
        }
    }

    private IEnumerator CloseTerminalWithDelay()
    {
        yield return new WaitForSeconds(1.5f);
        if (interactable != null)
        {
            interactable.CloseTerminal();
        }
    }

    void ShowHelp()
    {
        response.Add("Available commands:");
        response.Add("nmap -p- : Scan network for devices and open ports");
        response.Add("ssh root@<ip> : Attempt SSH connection to specified IP");
        response.Add("whoami : Display the current user identity");
        response.Add("ls : List files in the current directory");
        if (guardHacked)
        {
            response.Add("unlock guard : Command guard to unlock the cell door");
            response.Add("sleep <seconds> : Put guard to sleep for specified duration (1-10 seconds)");
        }
    }

    void GenerateRandomFiles()
    {
        randomFiles.Clear();
        string[] possibleFiles = { "logs_45.bin", "guard_notes.txt", "data_corrupt.sys", "backup_config.yaml" };
        for (int i = 0; i < 3; i++)
        {
            string randomFile = possibleFiles[Random.Range(0, possibleFiles.Length)];
            randomFiles.Add(randomFile);
        }
    }
}