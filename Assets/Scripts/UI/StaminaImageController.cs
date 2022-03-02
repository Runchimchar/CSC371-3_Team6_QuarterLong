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
        // Get parent slider
        GetSlider();
    }

    private void GetSlider() {
        slider = gameObject.GetComponent<Slider>();
        if (slider == null) {
            Invoke("GetSlider", 0.1f);
        }
    }

    // Update position based on slider value
    public void UpdateValue(float value) {
        if (slider != null) {
            sliderFill.anchoredPosition = new Vector2(rootPos.x - width * ((slider.maxValue - value) / slider.maxValue), rootPos.y);
        } else {
            slider = gameObject.GetComponent<Slider>();
        }
    }
}
