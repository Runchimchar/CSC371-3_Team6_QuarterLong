using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disable : MonoBehaviour
{

    public NewDrone newDScript;

    // Update is called once per frame
    void Update()
    {
        



    }

    void OnCollisionEnter(Collision thing)
    {
        if (thing.gameObject.tag == "Drone")
        {
            newDScript.stunned = true;
        }
    }
}
