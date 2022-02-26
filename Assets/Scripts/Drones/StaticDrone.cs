using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticDrone : MonoBehaviour
{
    AudioSource alarm;
    public GameObject[] allWaypoints;
    public GameObject staticWaypoint;
    int current = 0;
    public float speed;
    public float playerDistance = 5;
    public float damping;

    //public double rad = 0.99;

    public Transform player;
    public bool followPlayer = false;

    public bool stunned = false;
    Rigidbody rb;

    public int timeStunned = 3;
    public GameObject lightning;
    [SerializeField] private GameObject _laser;


    private void Start()
    {
        alarm = this.GetComponent<AudioSource>();
        rb = this.GetComponent<Rigidbody>();

        GetComponent<FieldOfView>().SubscribeToVisionEvent(seenPlayer);
        GetComponent<FieldOfView>().SubscribeToPlayerNotSeenEvent(doesntSeePlayer);
    }

    void FixedUpdate()
    {
        if (stunned == true)
        {
            lightning.SetActive(true);
            _laser.SetActive(false);
            rb.useGravity = true;
            rb.isKinematic = false;
            StartCoroutine(waiter());
        }
        else
        {
            lightning.SetActive(false);
            rb.useGravity = false;
            rb.isKinematic = true;
            if (followPlayer == false)
            {
                alarm.enabled = false;
                if (Vector3.Distance(staticWaypoint.transform.position, transform.position) != 0)
                {
                    var rotation = Quaternion.LookRotation(staticWaypoint.transform.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
                    transform.position = Vector3.MoveTowards(transform.position, staticWaypoint.transform.position, Time.deltaTime * speed);
                }
                else 
                {

                    var rotation = Quaternion.LookRotation(allWaypoints[current].transform.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);


                    Vector3 dirFromAtoB = (allWaypoints[current].transform.position - transform.position).normalized;
                    float dotProd = Vector3.Dot(dirFromAtoB, transform.forward);

                    if (dotProd >= 0.99)
                    {
                        current++;
                        if (current >= allWaypoints.Length)
                        {
                            current = 0;
                        }
                    }

                }
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

    public void seenPlayer()
    {
        followPlayer = true;
    }

    public void doesntSeePlayer()
    {
        followPlayer = false;
    }


    void OnCollisionEnter(Collision thing)
    {
        if (thing.gameObject.layer != LayerMask.NameToLayer("Moving"))
        {
            if (thing.gameObject.tag == "EMP")
            {
                stunned = true;
            }
        }
    }



    IEnumerator waiter()
    {
        yield return new WaitForSeconds(timeStunned);
        stunned = false;
        _laser.SetActive(true);
    }
}
