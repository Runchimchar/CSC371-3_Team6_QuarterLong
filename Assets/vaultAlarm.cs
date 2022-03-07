using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vaultAlarm : MonoBehaviour
{

    AudioSource alarm;
    public Transform player;


    // Start is called before the first frame update
    void Start()
    {
        alarm = this.GetComponent<AudioSource>();
        alarm.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.transform.position.y < transform.position.y)
        {
            alarm.enabled = true;
        }
        
    }
}
