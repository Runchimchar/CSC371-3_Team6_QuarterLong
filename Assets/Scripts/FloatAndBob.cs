using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAndBob : MonoBehaviour
{
    public float rotationSpeed = 10;
    public float bobSpeed = 10;
    Vector2 bobRange;
    float bobHeight = 1;
    float bobDirection; // 1 indicates up, -1 down

    // Start is called before the first frame update
    void Start()
    {
        bobRange = new Vector2(transform.position.y, transform.position.y + bobHeight);
        bobDirection = 1;
        transform.position = new Vector3(transform.position.x, Random.Range(bobRange.x, bobRange.y), transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        Spin();
        Bob();
    }
    
    void Spin()
    {
        Vector3 rotationAxis = new Vector3(0, 1, 0);
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
    void Bob()
    {
        Vector3 newPos = transform.position;
        newPos.y += bobSpeed * bobDirection * Time.deltaTime;
        transform.position = newPos;
        if (transform.position.y < bobRange.x)
        {
            bobDirection = 1;
        }
        else if (transform.position.y > bobRange.y)
        {
            bobDirection = -1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus ps = GameController.playerStatus;
            ps.grappleStatus = true;
            Destroy(gameObject);
        }
    }

}
