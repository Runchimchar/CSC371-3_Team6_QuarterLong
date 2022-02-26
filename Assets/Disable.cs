using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Disable : MonoBehaviour
{

    Vector3 startPosition;
    Vector3 startScale;
    Quaternion startRotation;

    public int timeToRespawn = 2;

    Rigidbody rb;

    public ParticleSystem hitEffect;

    



    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        startScale = transform.localScale;
        rb = this.GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision thing)
    {
        if(gameObject.layer != LayerMask.NameToLayer("Moving"))
        {
            if (thing.gameObject.tag != "Holder")
            {
                transform.localScale = new Vector3(0, 0, 0);
                hitEffect.Play();
                

                StartCoroutine(waiter());

                
            }
        }
    }


    IEnumerator waiter()
    {
       
        yield return new WaitForSeconds(timeToRespawn);

        transform.localScale = startScale;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = startRotation;
        transform.position = startPosition;
        


    }
}
