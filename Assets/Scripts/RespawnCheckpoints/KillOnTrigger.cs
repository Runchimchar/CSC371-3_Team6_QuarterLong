using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // When player enters trigger, respawn them at last checkpoint
        if (other.CompareTag("Player")) {
            RespawnController.instance.Respawn();
        }
    }
}
