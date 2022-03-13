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
    private float movableRotationStart;

    // Grapple button attributes (gb)
    private bool gb = false;
    private GameObject gbTarget;
    private float gbSpeed = 5f;
    private bool gbGoingOut = true;

    private bool retracting = false;
    private bool reelingOut = false;
    private bool playingRetract = false;
    private float retractSpeed = 1f;
    private float currentRetractSpeed = 0f;
    private float retractMinDistance = 0.5f;

    private bool aiming = false;

    private Quaternion desiredRotation;
    private float rotationSpeed = 15f;

    private PlayerStatus ps;
    private GameObject visibleGrappleGun;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        holdTransform = transform.Find("PlayerCameraRoot/HoldPosition");
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        ps = FindObjectOfType<PlayerStatus>();
        visibleGrappleGun = grappleGun.Find("GrappleGun").gameObject;
    }

    private void Update()
    {
        if (yoinking || joint || gb) UpdateGrapple();
        if (joint != null || yoinking) desiredRotation = Quaternion.LookRotation(grapplePoint - grappleGun.position);
        else if (pm.GetAiming()) desiredRotation = Quaternion.LookRotation(pm.GetAimPoint() - grappleGun.position);
        else desiredRotation = grappleGun.parent.rotation;

        grappleGun.rotation = Quaternion.Lerp(grappleGun.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }

    private void FixedUpdate()
    {
        //if (yoinking || joint || gb) UpdateGrapple();
        if (joint)
        {
            bool stopped = joint.maxDistance <= retractMinDistance + 0.1f || player.gameObject.GetComponent<Rigidbody>().velocity.magnitude < 0.1;
            
            if (stopped)
            {
                ropeSlide.PlayStop();
                playingRetract = false;
            }

            if (retracting && !reelingOut)
            {
                currentRetractSpeed = Mathf.Lerp(currentRetractSpeed, retractSpeed, 0.1f);
                joint.maxDistance = Mathf.Max((joint.maxDistance + Vector3.Distance(player.position, grapplePoint)) / 2f - currentRetractSpeed, retractMinDistance);
                if (!stopped && !playingRetract)
                {
                    ropeSlide.PlayRepeat();
                    playingRetract = true;
                }
            }
            else if (reelingOut && !retracting)
            {
                currentRetractSpeed = Mathf.Lerp(currentRetractSpeed, -retractSpeed, 0.1f);
                joint.maxDistance = Mathf.Max((joint.maxDistance + Vector3.Distance(player.position, grapplePoint)) / 2f - currentRetractSpeed, retractMinDistance);
                if (!stopped && !playingRetract)
                {
                    ropeSlide.PlayRepeat();
                    playingRetract = true;
                }
            }
            else
            {
                currentRetractSpeed = 0f;
            }
        }

        visibleGrappleGun.SetActive(ps.grappleStatus);
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void OnGrapple(InputValue value)
    {
        if (!ps.grappleStatus)
        {
            StopGrapple();
            return;
        }
        if (value.isPressed) StartGrapple();
        else StopGrapple();
    }

    //void OnSprint(InputValue value)
    //{
    //    if (value.isPressed)
    //    {
    //        retracting = true;
    //        if (joint)
    //        {
    //            ropeSlide.PlayRepeat();
    //            playingRetract = true;
    //        }
    //    }
    //    else
    //    {
    //        ropeSlide.PlayStop();
    //        playingRetract = false;
    //        retracting = false;
    //    }
    //}

    void OnThrow(InputValue value)
    {
        aiming = value.isPressed && !(yoinking || joint != null);
        pm.SetAiming(aiming);
        retracting = value.isPressed;
        if (value.isPressed && yoinking) ThrowObject();
        else if (retracting)
        {
            if (joint != null)
            {
                ropeSlide.PlayRepeat();
                playingRetract = true;
            }
        }
        else
        {
            ropeSlide.PlayStop();
            playingRetract = false;
        }
    }

    void OnReelOut(InputValue value)
    {
        reelingOut = value.isPressed;
        if (value.isPressed)
        {
            if (joint != null)
            {
                ropeSlide.PlayRepeat();
                playingRetract = true;
            }
        }
        else
        {
            ropeSlide.PlayStop();
            playingRetract = false;
        }
    }

    void StartGrapple()
    {
        if (joint) return;

        if (Vector3.Distance(pm.GetAimPoint(), ropeStart.position) <= maxDistance)
        {
            RaycastHit hit = pm.GetHit();
            if (!hit.transform.gameObject) return;
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

        if (!gb) lr.positionCount = 0;
    }

    void YoinkObject()
    {
        yoinking = true;
        movableRotationStart = vision.rotation.eulerAngles.y;
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

        pm.SetGrapple(false);
    }

    private void GrappleButton()
    {
        gb = true;
        gbTarget = grappleObject;
        grapplePoint = ropeStart.position;
        gbGoingOut = true;
        // Activate button
        ButtonControls bc = grappleObject.GetComponent<ButtonControls>();
        if (bc != null)
        {
            bc.Interact();
        }

        lr.positionCount = 2;
    }

    void DrawRope()
    {
        if (lr.positionCount != 2) return;
        lr.SetPosition(0, ropeStart.position);
        lr.SetPosition(1, grapplePoint);
    }

    void UpdateGrapple()
    {
        if (!gb && grappleObject == null)
        {
            StopGrapple();
            return;
        }
        if (gb)
        {
            gbGoingOut = gbGoingOut && Vector3.Distance(grapplePoint, gbTarget.transform.position) > 0.5f;

            if (!(gbGoingOut || Vector3.Distance(grapplePoint, ropeStart.position) > 0.5f))
            {
                lr.positionCount = 0;
                gb = false;
            }

            if (gbGoingOut) grapplePoint = Vector3.Lerp(grapplePoint, gbTarget.transform.position, gbSpeed * Time.deltaTime);
            else grapplePoint = Vector3.Lerp(grapplePoint, ropeStart.position, gbSpeed * Time.deltaTime);
        }
        else grapplePoint = grappleObject.transform.position - (yoinking ? Vector3.zero : grappleObjectOffset);
        if (yoinking) movableJoint.connectedAnchor = holdTransform.position;
        //joint.connectedAnchor = grapplePoint;
    }

    void ThrowObject()
    {
        Rigidbody yorb = grappleObject.GetComponent<Rigidbody>();
        Vector3 bias = 0.2f * Vector3.up;
        Vector3 throwDir = ((pm.GetAimPoint() - grappleObject.transform.position).normalized + bias).normalized;
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

    public float GetMovableRotationOffset()
    {
        return vision.rotation.eulerAngles.y - movableRotationStart;
    }
}
