using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JailDrone : MonoBehaviour
{

    AudioSource alarm;

    public float speed;
    float wpRadius = 1;
    public float playerDistance = 5;
    public float damping;

    public Transform player;
    public bool followPlayer = false;

    bool moving = true;

    // Start is called before the first frame update
    void Start()
    {
        alarm = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (followPlayer == true)
        {
            var rotation = Quaternion.LookRotation(player.transform.position - transform.position);
            alarm.enabled = true;
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * speed);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        }
        else
        {
            alarm.enabled = false;
        }
    }
}
