using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLightColorOnButton : MonoBehaviour
{
    public ButtonControls bc;
    // Start is called before the first frame update
    void Start()
    {
        bc.OnButtonActivate += ToggleLightColor;
        bc.OnButtonDeactivate += ToggleLightColor;
    }

    void ToggleLightColor()
    {
        Light l = GetComponent<Light>();
        l.color = Color.green;
    }
}
