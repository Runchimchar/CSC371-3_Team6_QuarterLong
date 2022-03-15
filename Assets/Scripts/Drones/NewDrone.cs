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

    public int timeStunned = 3;
    public GameObject lightning;

    bool moving = true;
    private DroneAttack _attackComponent = null;

    private void Start()
    {
        alarm = this.GetComponent<AudioSource>();
        rb = this.GetComponent<Rigidbody>();

        GetComponent<FieldOfView>().SubscribeToVisionEvent(seenPlayer);
        GetComponent<FieldOfView>().SubscribeToPlayerNotSeenEvent(doesntSeePlayer);

        _attackComponent = GetComponent<DroneAttack>();
        if(_attackComponent != null)
        {
            _attackComponent.SubscribeToDroneAttacksEvent(stopMovingDrone);
        }

    }

    void Update()
    {
        if (stunned == true)
        {
            lightning.SetActive(true);
            rb.useGravity = true;
            rb.isKinematic = false;
            StartCoroutine(waiter());
        }
        else
        {
            lightning.SetActive(false);
            rb.useGravity = false;
            rb.isKinematic = true;

            var rotation = Quaternion.LookRotation(player.transform.position - transform.position);
            

            if (moving == true)
            {
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
                    rotation = Quaternion.LookRotation(allWaypoints[current].transform.position - transform.position);
                    //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
                }
                else
                {
                    alarm.enabled = true;
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * speed);
                }
            }
            else if(moving == false)
            {
                StartCoroutine(waitToMove());
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        }
    }

    public void seenPlayer()
    {
        followPlayer = true;
    }

    public void doesntSeePlayer()
    {
        followPlayer = false;
    }

    public void movingDrone()
    {
        moving = true;
    }

    public void stopMovingDrone()
    {
        moving = false;
    }

    void OnCollisionEnter(Collision thing)
    {
        if (thing.gameObject.layer != LayerMask.NameToLayer("Moving"))
        {
            if (thing.gameObject.tag == "EMP")
            {
                stunned = true;
                _attackComponent.SetCanAttack(false);
            }
        }
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(timeStunned);
        stunned = false;
        _attackComponent.SetCanAttack(true);
    }

    IEnumerator waitToMove()
    {
        yield return new WaitForSeconds((float)0.5);
        moving = true;
    }
}
