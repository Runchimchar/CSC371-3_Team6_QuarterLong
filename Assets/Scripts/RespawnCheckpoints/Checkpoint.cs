using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool passedCheckpoint = false;
    public RespawnController rc;
    [SerializeField] Vector3 spawnLoc;
    public Vector2 spawnDirection;
    public int spawnIndex;

    public void Start()
    {
        spawnLoc = transform.position;
        spawnLoc.y += 1; // raises spawn point to a safe spot for player
        spawnDirection = transform.forward; // set temp direction in case we skip to checkpoint
    }
    private void Update()
    {
        if (spawnIndex == 0)
        {
            print("SPAWNLOC OF " + this.name + spawnLoc);
        }
    }
    /*
    public void Init(Vector3 location, Vector2 direction)
    {
        // Allows first spawn point to be initialized
        print("GIVEN LOC " + location.ToString());
        passedCheckpoint = true;
        print("OLD LOC " + this.spawnLoc.ToString());
        spawnLoc = location;
        print("NEW LOC " + this.spawnLoc.ToString());
        spawnDirection = direction;
        spawnIndex = 0;
    }
    */
    private void OnTriggerEnter(Collider other)
    {
        if (!passedCheckpoint && other.gameObject.CompareTag("Player"))
        {
            // Player passes through checkpoint
            passedCheckpoint = true;
            spawnDirection = other.transform.forward;
            rc.SetCurIndex(spawnIndex);
        }
    }
    public Vector3 GetSpawnLocation()
    {
        return spawnLoc;
    }
    public Vector2 GetSpawnDirection()
    {
        return spawnDirection;
    }
    public void SetIndex(int index)
    {
        spawnIndex = index;
    }
    public void SetSpawnLocation(Vector3 location)
    {
        spawnLoc = location;
        print("NAME OF CHECKPOINT" + this.name);
    }
    public void SetSpawnDirection(Vector2 direction)
    {
        spawnDirection = direction;
    }
    public void SetPassedThrough()
    {
        passedCheckpoint = true;
    }
}
