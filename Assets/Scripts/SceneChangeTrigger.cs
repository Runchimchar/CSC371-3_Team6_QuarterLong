using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeTrigger : MonoBehaviour
{
    public float delay = 0f;
    public string nextScene = "";
    public ButtonControls optionalButton = null;

    private bool triggered = false;

    void Start() {
        // Make change on button push
        if (optionalButton) {
            optionalButton.OnButtonActivate += ChangeDelayed;
        }
    }

    void ChangeDelayed() {
        triggered = true;
        Invoke("Change", delay);
    }

    void Change() {
        // Cleanup if needed
        if (optionalButton) {
            optionalButton.OnButtonActivate -= ChangeDelayed;
        }

        GameController.sceneController.ChangeScene(nextScene);
    }

    void OnTriggerEnter(Collider other) {
        // Only interact with player
        if (other.tag == "Player" && !triggered && !optionalButton) {
            ChangeDelayed();
        }
    }
}
