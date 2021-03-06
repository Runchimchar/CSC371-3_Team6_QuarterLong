using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObjectRespawn : MonoBehaviour
{
    [SerializeField] float respawnY;
    [SerializeField] float indicatorDistance = 5;
    [SerializeField] LayerMask colliderMask;
    [SerializeField] bool dontDrawArrow = false;
    Vector3 startPosition;
    float indCurLen = 0;
    float indSpeed = 20f;
    float rotateSpeed = 5f;
    float? startRotation;
    bool yoinking = false;
    Grapple grappleScript;
    LineRenderer lr;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        if (!dontDrawArrow) lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.y < respawnY)
        {
            Reset();
        }

        if (yoinking && !dontDrawArrow)
        {
            if (!startRotation.HasValue) startRotation = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(startRotation.Value * Vector3.up) * Quaternion.Euler(grappleScript.GetMovableRotationOffset() * Vector3.up), rotateSpeed * Time.fixedDeltaTime);
        }
        else if (!dontDrawArrow)
        {
            startRotation = null;
        }
    }

    private void LateUpdate()
    {
        if (yoinking && !dontDrawArrow)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, transform.position);

            Ray dir = new Ray(transform.position, Vector3.down * indicatorDistance);

            if (Physics.Raycast(dir, out RaycastHit hit, indicatorDistance, colliderMask, QueryTriggerInteraction.Ignore))
                indCurLen = Mathf.Lerp(indCurLen, transform.position.y - hit.point.y, Time.deltaTime * indSpeed);
            else
                indCurLen = Mathf.Lerp(indCurLen, indicatorDistance, Time.deltaTime * indSpeed);

            lr.SetPosition(1, transform.position - new Vector3(0, indCurLen - 0.3f, 0));
        }
        else if (!dontDrawArrow)
        {
            lr.positionCount = 0;
            indCurLen = 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, respawnY, transform.position.z));
    }

    public void SetYoink(bool newYoink, Grapple grapple)
    {
        yoinking = newYoink;
        grappleScript = grapple;
    }

    public bool GetYoink()
    {
        return yoinking;
    }
    public void Reset()
    {
        if (yoinking) grappleScript.StopGrapple();
        transform.position = startPosition;
        rb.angularVelocity = rb.velocity = Vector3.zero;
    }
}
