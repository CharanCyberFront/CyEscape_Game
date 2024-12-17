using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // Add this to include IEnumerator

public class SceneTransitionManager : MonoBehaviour
{
    public Image fadeOverlay; // Assign the black overlay image here
    public float fadeDuration = 2f; // Duration of fade in/out in seconds

    private void Start()
    {
        // Ensure the overlay starts fully transparent
        if (fadeOverlay != null)
            fadeOverlay.color = new Color(0, 0, 0, 0);
    }

    public void FadeToNextScene(string nextSceneName)
    {
        StartCoroutine(FadeOutAndLoadScene(nextSceneName));
    }

    private IEnumerator FadeOutAndLoadScene(string nextSceneName)
    {
        // Fade to black
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            fadeOverlay.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Load the next scene
        SceneManager.LoadScene(nextSceneName);

        // Fade back in (if desired, adjust accordingly for level 5 intro)
        timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1 - Mathf.Clamp01(timer / fadeDuration);
            fadeOverlay.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}