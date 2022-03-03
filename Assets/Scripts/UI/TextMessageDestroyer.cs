using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMessageDestroyer : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDelay;
    public float fadeTime;

    private float fadeStep = 0.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        // Start fading the message
        Invoke("FadeMessage", fadeDelay);
    }

    private void FadeMessage() {
        // Reduce alpha
        canvasGroup.alpha = Mathf.Max(canvasGroup.alpha - fadeStep, 0f);
        Debug.Log("Alpha is " + canvasGroup.alpha);
        // Check if alpha is 0
        if (canvasGroup.alpha <= 0.001f) {
            Destroy(gameObject);
        } else {
            Debug.Log("Invoking in " + (fadeTime * fadeStep) + " seconds");
            Invoke("FadeMessage", fadeTime * fadeStep);
        }
    }
}
