using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class Grapple : MonoBehaviour
{
    public LayerMask grapplable;
    public LayerMask movable;
    public LayerMask moving;
    public LayerMask interactable;
    public Transform ropeStart, vision, player, orientation, grappleGun;
    public AudioController ropeCreak, ropeSlide, grappleExtend;
    public PlayerMovement pm;
    public float throwForce = 20f;

    private LineRenderer lr;
    private Vector3 grapplePoint;
    private GameObject grappleObject;
    private Vector3 grappleObjectOffset;
    private float maxDistance = 100f;
    private SpringJoint joint;
    private SpringJoint movableJoint;

    private float originalSleep;
    private float originalDrag;
    private LayerMask originalLayer;

    private Transform holdTransform;
    private bool yoinking = false;
    private Collider capsuleCollider;

    private bool retracting = false;
    private bool playingRetract = false;
    private float retractSpeed = 1f;
    private float retractMinDistance = 0.5f;

    private bool aiming = false;

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
        if (joint != null || yoinking) desiredRotation = Quaternion.LookRotation(grapplePoint - grappleGun.position);
        else if (pm.GetAiming()) desiredRotation = Quaternion.LookRotation(pm.GetAimPoint() - grappleGun.position);
        else desiredRotation = grappleGun.parent.rotation;

        grappleGun.rotation = Quaternion.Lerp(grappleGun.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }

    private void FixedUpdate()
    {
        if (yoinking || joint) UpdateGrapple();
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

    void OnThrow(InputValue value)
    {
        aiming = value.isPressed && !yoinking;
        pm.SetAiming(aiming);
        if (value.isPressed && yoinking) ThrowObject();
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
            if (grappleObject.CompareTag("Button"))
            {
                GrappleButton();
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
        yoinking = true;
        pm.SetYoink(true);
        grappleObject.GetComponent<MovableObjectRespawn>().SetYoink(true, this);

        Rigidbody yorb = grappleObject.GetComponent<Rigidbody>();
        pm.SetGrapple(true);

        originalDrag = yorb.drag;
        originalSleep = yorb.sleepThreshold;
        originalLayer = grappleObject.layer;
        // grappleExtend.PlayOnce();
        // ropeCreak.PlayRepeat();
        movableJoint = grappleObject.AddComponent<SpringJoint>();
        movableJoint.autoConfigureConnectedAnchor = false;
        movableJoint.enableCollision = false;
        movableJoint.connectedAnchor = holdTransform.position;

        movableJoint.maxDistance = 0;
        movableJoint.damper = 1f;
        movableJoint.massScale = 20f * yorb.mass;
        
        yorb.useGravity = false;
        lr.positionCount = 2;
        yorb.freezeRotation = true;
        yorb.drag = 10f;
        yorb.sleepThreshold = 0;
        grappleObject.layer = (int) Mathf.Log(moving.value, 2);
        Physics.IgnoreCollision(grappleObject.GetComponent<Collider>(), player.GetComponentInChildren<CapsuleCollider>(), true);

        UpdateGrapple();
    }

    void StopYoink()
    {
        pm.SetYoink(false);
        grappleObject.GetComponent<MovableObjectRespawn>().SetYoink(false, this);
        Rigidbody yorb = grappleObject.GetComponent<Rigidbody>();
        //grappleExtend.PlayStop();
        //ropeCreak.PlayStop();
        //ropeSlide.PlayStop();
        //playingRetract = false;
        Destroy(movableJoint);
        yorb.useGravity = true;
        yoinking = false;
        lr.positionCount = 0;
        yorb.freezeRotation = false;
        yorb.drag = originalDrag;
        yorb.sleepThreshold = originalSleep;
        grappleObject.layer = originalLayer;

        Physics.IgnoreCollision(grappleObject.GetComponent<Collider>(), player.GetComponentInChildren<CapsuleCollider>(), false);
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
        if (yoinking) movableJoint.connectedAnchor = holdTransform.position;
        //joint.connectedAnchor = grapplePoint;
    }

    void ThrowObject()
    {
        Rigidbody yorb = grappleObject.GetComponent<Rigidbody>();
        Vector3 throwDir = (pm.GetAimPoint() - grappleObject.transform.position).normalized;
        StopYoink();
        yorb.velocity = player.GetComponentInChildren<Rigidbody>().velocity;
        yorb.AddForce(throwDir * throwForce, ForceMode.Impulse);
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

    public GameObject GetGrappleObject()
    {
        return grappleObject;
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

    private void GrappleButton()
    {
        // Activate button
        ButtonControls bc = grappleObject.GetComponent<ButtonControls>();
        if (bc != null)
        {
            bc.Interact();
        }

        /* TODO: animate a little grapple rope hitting the object? */
    }
}
