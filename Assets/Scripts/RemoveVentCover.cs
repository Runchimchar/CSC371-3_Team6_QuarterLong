using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveVentCover : MonoBehaviour
{
    PlayerMovement pm;

    void RemoveVent()
    {
        if (pm != null) 
            pm.InteractEvent -= RemoveVent;
        this.gameObject.SetActive(false);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("PLAYER IN RANGE");
            // Player enters range of lever
            PlayerMovement pm = other.transform.parent.GetComponent<PlayerMovement>();
            pm.InteractEvent += RemoveVent;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("PLAYER OUT OF RANGE");
            // Player leaves range of lever
            pm = other.transform.parent.GetComponent<PlayerMovement>();
            pm.InteractEvent -= RemoveVent;
        }
    }
}
