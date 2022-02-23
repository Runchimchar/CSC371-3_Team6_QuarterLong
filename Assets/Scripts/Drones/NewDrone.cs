using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewDrone : MonoBehaviour
{
    AudioSource alarm;
    public GameObject[] allWaypoints;
    int current = 0;
    public float speed;
    float wpRadius = 1;
    public float playerDistance = 5;
    public float damping;

    public Transform player;
    public bool followPlayer = false;

    public bool stunned = false;
    Rigidbody rb;


    private void Start()
    {
        alarm = this.GetComponent<AudioSource>();
        rb = this.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (stunned == true)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            StartCoroutine(waiter());
        }
        else
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            if (followPlayer == false)
            {
                alarm.enabled = false;
                if (Vector3.Distance(allWaypoints[current].transform.position, transform.position) < wpRadius)
                {
                    current++;
                    if (current >= allWaypoints.Length)
                    {
                        current = 0;
                    }
                }
                transform.position = Vector3.MoveTowards(transform.position, allWaypoints[current].transform.position, Time.deltaTime * speed);
                var rotation = Quaternion.LookRotation(allWaypoints[current].transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
            }
            else
            {
                alarm.enabled = true;
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * speed);
                var rotation = Quaternion.LookRotation(player.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
                if (Vector3.Distance(transform.position, player.transform.position) > playerDistance)
                {
                    followPlayer = false;
                }
                // TEMPORARY: if the drone gets too close to the player, respawn the player TODO: delete this part
                else if (Vector3.Distance(transform.position, player.transform.position) < 1)
                {
                    followPlayer = false;
                    RespawnController.instance.Respawn();
                }
            }
        }
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(3);
        stunned = false;
    }
}
