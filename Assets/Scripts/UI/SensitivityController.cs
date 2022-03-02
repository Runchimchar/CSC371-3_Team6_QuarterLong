using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensitivityController : MonoBehaviour
{
    private PlayerMovement movement;

    // Setup
    private void Start() {
        // Get movement script
        movement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        FindMovement();
    }

    private void FindMovement() {
        movement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        if (movement == null) {
            Invoke("FindMovement", 0.1f);
        }
    }

    public void UpdateSensitivity(float val) {
        // Map 1 - 5 to 1/4 - 2
        float mappedVal = Mathf.Max(0.5f*val - 0.5f, 0.25f);
        //Debug.Log(mappedVal);
        if (movement != null) {
            movement.SetLookSensitivityScale(mappedVal);
        }
    }
}
