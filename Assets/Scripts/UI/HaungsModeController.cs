using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaungsModeController : MonoBehaviour
{
    // Haungs mode
    public void UpdateHaungsMode(bool val) {
        GameController.playerStatus.SetHaungsMode(val);
    }
}
