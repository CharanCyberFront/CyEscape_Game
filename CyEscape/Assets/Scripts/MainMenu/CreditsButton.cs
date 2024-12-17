using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CreditsButton : MonoBehaviour
{
    public void ToCredits()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 7); // Load next scene
    }
}
