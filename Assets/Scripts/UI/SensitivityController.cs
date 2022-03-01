using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensitivityController : MonoBehaviour
{
    private PlayerMovement movement;

    // Setup
    private void Awake() {
        // Get movement script
        movement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    public void UpdateSensitivity(float val) {
        // Map 1 - 5 to 1/4 - 2
        float mappedVal = Mathf.Max(0.5f*val - 0.5f, 0.25f);
        //Debug.Log(mappedVal);
        movement.SetLookSensitivityScale(mappedVal);
    }
}
