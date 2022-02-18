using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    // Follows singleton pattern
    public PlayerMovement pm;
    Vector3 spawnLoc;
    Vector2 spawnDirection;
    public static RespawnController instance;

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
        // set the default spawn point to first location in level
        spawnLoc = pm.transform.position;
        Vector3 rot = pm.transform.rotation.eulerAngles;
        spawnDirection = new Vector2(rot.x, rot.y);
    }

    public void UpdateSpawn(Vector3 position)
    {
        /* Note: the given position should be on the floor of the intended
         * respawn location. */
        position.y += 1;
        spawnLoc = position;
        Vector3 rot = pm.transform.rotation.eulerAngles;
        spawnDirection = new Vector2(rot.x, rot.y);
    }

    public void Respawn()
    {
        // resets the level and sends the player to the spawn point
        // TODO reset level
        pm.RespawnAt(spawnLoc, spawnDirection);
    }
}
