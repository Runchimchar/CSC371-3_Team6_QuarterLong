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
    
    public float damping;

    public bool stunned = false;
    Rigidbody rb;

    public int timeStunned = 3;
    public GameObject lightning;
    [SerializeField] private GameObject _laser;


    private void Start()
    {
        
        rb = this.GetComponent<Rigidbody>();

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
