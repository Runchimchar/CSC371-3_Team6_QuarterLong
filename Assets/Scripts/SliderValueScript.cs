using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SliderValueScript : MonoBehaviour
{

    public TMP_Text valueText;

    public void UpdateValue(float value) {
        valueText.text = Mathf.RoundToInt(value).ToString();
    }
}
