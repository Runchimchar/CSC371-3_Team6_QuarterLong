using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaImageController : MonoBehaviour
{
    public RectTransform sliderFill;

    private Slider slider;

    private Vector2 rootPos;
    private float width;

    void Start()
    {
        // Get root position of slider
        rootPos = sliderFill.anchoredPosition;
        // Find width of image
        width = sliderFill.rect.xMax - sliderFill.rect.xMin;
        Debug.Log("width = " + width);
        // Get parent slider
        slider = gameObject.GetComponent<Slider>();
        Invoke("CV", 3);
    }

    // Update position based on slider value
    public void UpdateValue(float value) {
        sliderFill.anchoredPosition = new Vector2(rootPos.x - width*((slider.maxValue-value)/slider.maxValue), rootPos.y);
        Debug.Log("Got update, value = "+value+", anchorX = " + sliderFill.anchoredPosition.x);
    }

    // Change val debug
    public void CV() {
        slider.value -= 1;
        if (slider.value > 0) {
            Invoke("CV", 0.05f);
        }
    }
}
