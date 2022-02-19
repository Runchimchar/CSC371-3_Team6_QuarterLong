using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class Grapple : MonoBehaviour
{
    public LayerMask grapplable;
    public LayerMask movable;
    public Transform ropeStart, vision, player, orientation, grappleGun;
    public AudioController ropeCreak, ropeSlide, grappleExtend;
    public PlayerMovement pm;

    private LineRenderer lr;
    private Vector3 grapplePoint;
    private GameObject grappleObject;
    private Vector3 grappleObjectOffset;
    private float maxDistance = 100f;
    private SpringJoint joint;

    private Transform holdTransform;
    private bool yoinking = false;
    private float yoinkSpeed = 50f;
    private Collider capsuleCollider;

    private bool retracting = false;
    private bool playingRetract = false;
    private float retractSpeed = 1f;
    private float retractMinDistance = 0.5f;

    private Quaternion desiredRotation;
    private float rotationSpeed = 15f;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        holdTransform = transform.Find("PlayerCameraRoot/HoldPosition");
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
    }

    private void Update()
    {
        if (yoinking || joint) UpdateGrapple();
        if (yoinking)
        {
            Rigidbody yorb = grappleObject.GetComponent<Rigidbody>();
            
            Vector3 direction = holdTransform.position - ropeStart.position;
            Ray yoinkRay = new Ray(ropeStart.position, direction);
            RaycastHit yoinkHit;
            if (!yorb.SweepTest(direction, out yoinkHit, direction.magnitude))
                yorb.MovePosition(Vector3.Lerp(yorb.transform.position, holdTransform.position, yoinkSpeed * Time.deltaTime));
            else
            {
                GameObject obstructor = yoinkHit.transform.gameObject;
                Rigidbody obstructorRB = yoinkHit.rigidbody;
                
                if (IsInLayerMask(obstructor, pm.GetAimColliderMask()) && (obstructor.isStatic || !obstructorRB || obstructorRB.isKinematic))
                    yorb.MovePosition(Vector3.Lerp(yorb.transform.position, ropeStart.position + (yoinkHit.distance * direction.normalized), yoinkSpeed * Time.deltaTime));
                else
                    yorb.MovePosition(Vector3.Lerp(yorb.transform.position, holdTransform.position, yoinkSpeed * Time.deltaTime));
            }
        }
        if (joint != null || yoinking) desiredRotation = Quaternion.LookRotation(grapplePoint - grappleGun.position);
        else if (pm.GetAiming()) desiredRotation = Quaternion.LookRotation(pm.GetAimPoint() - grappleGun.position);
        else desiredRotation = grappleGun.parent.rotation;

        grappleGun.rotation = Quaternion.Lerp(grappleGun.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }

    private void FixedUpdate()
    {
        if (joint)
        {
            bool stopped = joint.maxDistance <= retractMinDistance + 0.1f || player.gameObject.GetComponent<Rigidbody>().velocity.magnitude < 0.1;
            
            if (stopped)
            {
                ropeSlide.PlayStop();
                playingRetract = false;
            }

            UpdateGrapple();

            if (retracting)
            {
                joint.maxDistance = Mathf.Max((joint.maxDistance + Vector3.Distance(player.position, grapplePoint)) / 2f - retractSpeed, retractMinDistance);
                if (!stopped && !playingRetract)
                {
                    ropeSlide.PlayRepeat();
                    playingRetract = true;
                }
            }
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void OnGrapple(InputValue value)
    {
        if (value.isPressed) StartGrapple();
        else StopGrapple();
    }

    void OnSprint(InputValue value)
    {
        if (value.isPressed)
        {
            retracting = true;
            if (joint)
            {
                ropeSlide.PlayRepeat();
                playingRetract = true;
            }
        }
        else
        {
            ropeSlide.PlayStop();
            playingRetract = false;
            retracting = false;
        }
    }

    void StartGrapple()
    {
        if (joint) return;

        if (Vector3.Distance(pm.GetAimPoint(), ropeStart.position) <= maxDistance)
        {
            RaycastHit hit = pm.GetHit();
            grappleObject = hit.transform.gameObject;
            grapplePoint = hit.point;
            grappleObjectOffset = grappleObject.transform.position - hit.point;
            if (IsInLayerMask(grappleObject, movable))
            {
                YoinkObject();
                return;
            }
            if (!IsInLayerMask(grappleObject, grapplable)) return;
            Rigidbody cb = grappleObject.GetComponent<Rigidbody>();
            pm.SetGrapple(true);
            grappleExtend.PlayOnce();
            ropeCreak.PlayRepeat();
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.enableCollision = true;
            joint.anchor = ropeStart.position - player.position;
            if (cb != null)
            {
                joint.connectedBody = cb;
                joint.connectedAnchor = grapplePoint - grappleObject.transform.position;
            }
            else joint.connectedAnchor = grapplePoint;
            
            float distanceFromPoint = Vector3.Distance(ropeStart.position, grapplePoint);
            
            joint.maxDistance = distanceFromPoint; // 1f // distanceFromPoint * 0.8f
            joint.minDistance = 0; // distanceFromPoint * 0.25f
            joint.damper = 1f; // 7f 
            joint.massScale = 100f;//4.5f;

            lr.positionCount = 2;
        }
    }

    public void StopGrapple()
    {
        if (joint)
        {
            pm.SetGrapple(false);
            grappleExtend.PlayStop();
            ropeCreak.PlayStop();
            ropeSlide.PlayStop();
            playingRetract = false;
            Destroy(joint);
        }
        if (yoinking)
        {
            StopYoink();
        }

        lr.positionCount = 0;
    }

    void YoinkObject()
    {
        pm.SetYoink(true);
        UpdateGrapple();
        Rigidbody yorb = grappleObject.GetComponent<Rigidbody>();
        Physics.IgnoreCollision(grappleObject.GetComponent<Collider>(), capsuleCollider, true);
        yorb.isKinematic = true;
        yorb.useGravity = false;
        yorb.detectCollisions = false;
        yoinking = true;
        lr.positionCount = 2;
    }

    void StopYoink()
    {
        pm.SetYoink(false);
        Rigidbody yorb = grappleObject.GetComponent<Rigidbody>();
        Physics.IgnoreCollision(grappleObject.GetComponent<Collider>(), capsuleCollider, false);
        yorb.isKinematic = false;
        yorb.useGravity = true;
        yorb.detectCollisions = true;
        yoinking = false;
    }

    void DrawRope()
    {
        if (!joint && !yoinking) return;
        lr.SetPosition(0, ropeStart.position);
        lr.SetPosition(1, grapplePoint);
    }

    void UpdateGrapple()
    {
        if (grappleObject == null)
        {
            StopGrapple();
            return;
        }
        grapplePoint = grappleObject.transform.position - (yoinking ? Vector3.zero : grappleObjectOffset);
        //joint.connectedAnchor = grapplePoint;
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public bool IsYoinking()
    {
        return yoinking;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    public bool Grapplable()
    {
        return Vector3.Distance(pm.GetAimPoint(), ropeStart.position) <= maxDistance &&
        IsInLayerMask(pm.GetHit().transform.gameObject, grapplable);
    }

    public bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }
}