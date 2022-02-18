using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class flyingDronePatrol : MonoBehaviour
{
    // Start is called before the first frame update


    GameObject _destination;
    NavMeshAgent _navMeshAgent;
    GameObject[] allWaypoints;
    int i;

    void Start()
    {
        allWaypoints = GameObject.FindGameObjectsWithTag("waypointTest");
        _navMeshAgent = this.GetComponent<NavMeshAgent>();
        i = 0;
        _destination = allWaypoints[i];
        SetDestination();
    }

    // Update is called once per frame
    void Update()
    {
        float wpRadius = 1;

        if (Vector3.Distance(allWaypoints[i].transform.position, transform.position) < wpRadius)
        {
            i++;
            if (i >= allWaypoints.Length)
            {
                i = 0;
            }
            _destination = allWaypoints[i];
        }
        SetDestination();
    }

    private void SetDestination()
    {
        if (_destination != null)
        {
            Vector3 targetVector = _destination.transform.position;
            _navMeshAgent.SetDestination(targetVector);
        }
    }
}
