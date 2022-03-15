using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Disable : MonoBehaviour
{

    Vector3 startPosition;
    Vector3 startScale;
    Quaternion startRotation;

    Coroutine waitingCoroutine;
    bool waiting;

    public int timeToRespawn = 2;
    public float range = 3f;
    public LayerMask enemy;

    Rigidbody rb;

    public ParticleSystem hitEffect;

    



    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        startScale = transform.localScale;
        rb = this.GetComponent<Rigidbody>();
        waiting = false;
    }

    void OnCollisionEnter(Collision thing)
    {
        if(!waiting && gameObject.layer != LayerMask.NameToLayer("Moving"))
        {
            if (thing.gameObject.tag != "Holder")
            {
                transform.localScale = new Vector3(0, 0, 0);
                hitEffect.Play();

                Collider[] outs = Array.FindAll(
                    Physics.OverlapSphere(transform.position, range, enemy, QueryTriggerInteraction.Collide),
                    x => x.gameObject.CompareTag("Cedric")
                );
                if (outs.Length > 0) outs[0].gameObject.GetComponent<CEOController>().Stun();
                

                waitingCoroutine = StartCoroutine(waiter());

                
            }
        }
    }

    public void DisableNow()
    {
        if (waiting) StopCoroutine(waitingCoroutine);
        waiting = false;
        transform.localScale = new Vector3(0, 0, 0);
        hitEffect.Play();
        StartCoroutine(waiter());
    }


    IEnumerator waiter()
    {
        if (!waiting)
        {
            waiting = true;
            yield return new WaitForSeconds(timeToRespawn);
            if (waiting) ResetEMP(false);
        }
    }

    public void ResetEMP(bool stopCoroutine = true)
    {
        if (stopCoroutine && waiting) StopCoroutine(waitingCoroutine);
        transform.localScale = startScale;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = startRotation;
        transform.position = startPosition;
        waiting = false;
    }
}
