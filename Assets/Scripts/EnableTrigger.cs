using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTrigger : MonoBehaviour
{
    public GameObject toEnable = null;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            toEnable.SetActive(true);
            Destroy(gameObject);
        }
    }
}
