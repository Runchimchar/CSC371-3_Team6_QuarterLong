using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObjectRespawn : MonoBehaviour
{
    [SerializeField] float respawnY;
    Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.y < respawnY)
        {
            transform.position = startPosition;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, respawnY, transform.position.z));
    }
}
