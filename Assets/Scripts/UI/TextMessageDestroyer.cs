using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMessageDestroyer : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDelay;
    public float fadeTime;

    public event System.Action DestroyEvent = delegate { };

    private float fadeStep = 0.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        // Fade in message
        canvasGroup.alpha = 0f;
        FadeInMessage();

        // Prepare to fade out message
        Invoke("FadeOutMessage", fadeDelay + fadeTime);
    }

    private void FadeInMessage() {
        // Increase alpha
        canvasGroup.alpha = Mathf.Min(canvasGroup.alpha + fadeStep, 1f);
        //Debug.Log("Alpha is " + canvasGroup.alpha);
        // Check if alpha is 0
        if (canvasGroup.alpha < 0.999f) {
            Invoke("FadeInMessage", fadeTime * fadeStep);
        }
    }

    private void FadeOutMessage() {
        // Reduce alpha
        canvasGroup.alpha = Mathf.Max(canvasGroup.alpha - fadeStep, 0f);
        //Debug.Log("Alpha is " + canvasGroup.alpha);
        // Check if alpha is 0
        if (canvasGroup.alpha <= 0.001f) {
            Destroy(gameObject, 0.5f);
        }
        else {
            //Debug.Log("Invoking in " + (fadeTime * fadeStep) + " seconds");
            Invoke("FadeOutMessage", fadeTime * fadeStep);
        }
    }

    // Notify that message is being destroyed
    private void OnDestroy() {
        DestroyEvent();
    }
}
