using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RespawnController : MonoBehaviour
{
    // Follows singleton pattern
    public static RespawnController instance;
    public PlayerMovement pm;
    public Checkpoint[] checkpoints;
    int curCheckpoint;
    public Room firstRoom;
    Room curRoom;
    public event Action CustomActionsOnRespawnReset = delegate { };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // dev variables
    public bool respawnNow = false;
    private void Update()
    {
        if (respawnNow)
        {
            respawnNow = false;
            Respawn();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameController.playerStatus)
        {
            GameController.playerStatus.HealthChangedEvent += RespawnOnDeath;
        }

        // set the default spawn point to first location in level and add it to array
        GameObject go = new GameObject("CheckpointInit");
        Checkpoint init = go.AddComponent<Checkpoint>();
        init.rc = this;
        Vector3 newPos = pm.transform.position;
        newPos.y -= 1;
        go.transform.position = newPos;
        go.transform.forward = pm.transform.forward;
        init.SetIndex(0);
        init.SetPassedThrough();

        Checkpoint[] newCheckpoints = new Checkpoint[checkpoints.Length + 1];
        newCheckpoints[0] = init;
        int i = 1;
        foreach (Checkpoint cp in checkpoints)
        {
            newCheckpoints[i] = cp;
            cp.SetIndex(i);
            i++;
        }
        checkpoints = newCheckpoints;
        curCheckpoint = 0;
        //Vector3 rot = pm.transform.rotation.eulerAngles;
        //spawnDirection = new Vector2(rot.x, rot.y);
        curRoom = firstRoom;
    }

    /*
    public void UpdateSpawn(Vector3 position)
    {
        // Note: the given position should be on the floor of the intended
        // respawn location.
        position.y += 1;
        spawnLoc = position;
        Vector3 rot = pm.transform.rotation.eulerAngles;
        spawnDirection = new Vector2(rot.x, rot.y);
    }
    */

    public void Respawn()
    {
        // resets the level and sends the player to the spawn point
        // TODO reset level
        curRoom.ResetRoom();
        CustomActionsOnRespawnReset();
        Checkpoint cp = checkpoints[curCheckpoint];
        pm.RespawnAt(cp.GetSpawnLocation(), cp.GetSpawnDirection());
    }
    public void SetCurIndex(int index)
    {
        curCheckpoint = index;
    }
    public void SetCurRoom(Room room)
    {
        curRoom = room;
    }
    public void SpawnNext()
    {
        if (curCheckpoint == checkpoints.Length - 1)
        {
            // Can't respawn at next, just respawn at current
            print("ERROR: There is no next checkpoint");
        }
        else { curCheckpoint++; }
        Respawn();
    }
    public void SpawnPrev()
    {
        if (curCheckpoint == 0)
        {
            // Can't respawn at last, just respawn at current
            print("ERROR: There is no previous checkpoint");
        }
        else { curCheckpoint--; }
        Respawn();
    }

    public void RespawnOnDeath()
    {
        PlayerStatus ps = GameController.playerStatus;
        if (ps.GetHealth() <= 0)
        {
            // player is dead, reset health and respawn them
            ps.SetHealth(ps.GetMaxHealth());
            Respawn();
        }
    }
}
