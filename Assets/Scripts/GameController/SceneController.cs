using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public CanvasGroup blackScreen;
    public float fadeTime = 0.5f;
    public float fadeStep = 0.1f;

    private bool changeStarted = false;
    private string sceneName = "";

    // Start a scene transition
    public void ChangeScene(string sceneName) {
        // Make sure we finish before next change
        if (changeStarted) return;

        // Mark that the change started
        changeStarted = true;
        // Save the target scene
        this.sceneName = sceneName;
        // Prevent menuing
        blackScreen.blocksRaycasts = true;
        // Fade back when done loading
        SceneManager.sceneLoaded += OnSceneChange;
        // Start fade
        FadeToBlack();
    }

    private void OnSceneChange(Scene scene, LoadSceneMode sceneMode) {
        FadeFromBlack();
    }

    private void FadeToBlack() {
        // Increase alpha
        blackScreen.alpha = Mathf.Min(blackScreen.alpha + fadeStep, 1f);
        // Check if alpha is 0
        if (blackScreen.alpha < 0.999f) {
            Invoke("FadeToBlack", fadeTime * fadeStep);
        } else {
            // Done fading to black, change scenes
            SceneManager.LoadScene(sceneName);
        }
    }

    private void FadeFromBlack() {
        // Reduce alpha
        blackScreen.alpha = Mathf.Max(blackScreen.alpha - fadeStep, 0f);
        // Check if alpha is 0
        if (blackScreen.alpha <= 0.001f) {
            // Done changing
            // Return control
            blackScreen.blocksRaycasts = false;
            // Cleanup event
            SceneManager.sceneLoaded -= OnSceneChange;
            // Allow another change to start
            changeStarted = false;
        }
        else {
            //Debug.Log("Invoking in " + (fadeTime * fadeStep) + " seconds");
            Invoke("FadeFromBlack", fadeTime * fadeStep);
        }
    }
}