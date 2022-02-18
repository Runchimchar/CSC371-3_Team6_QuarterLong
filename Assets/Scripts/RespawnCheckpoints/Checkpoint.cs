using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    bool passedCheckpoint = false;
    public RespawnController rc;

    private void OnTriggerEnter(Collider other)
    {
        if (!passedCheckpoint && other.gameObject.CompareTag("Player"))
        {
            // Player passes through checkpoint
            rc.UpdateSpawn(transform.position);
            passedCheckpoint = true;
        }
    }
}
