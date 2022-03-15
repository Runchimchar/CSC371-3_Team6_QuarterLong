using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovableObject : MonoBehaviour
{
    [SerializeField] Material removableMaterial;
    [SerializeField] Material startMaterial;
    [SerializeField] LayerMask movable;
    MovableObjectRespawn MO_ctrl;
    CEOController CEO_ctrl;
    Transform startParent;
    LayerMask startLayer;
    Grapple grapple;
    Rigidbody rb;

    bool materialSetFlip = false;
    bool removed = false;

    int index;

    void Start()
    {
        MO_ctrl = GetComponent<MovableObjectRespawn>();
        CEO_ctrl = FindObjectOfType<CEOController>();
        grapple = FindObjectOfType<Grapple>();
        startParent = transform.parent;
        startLayer = gameObject.layer;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (removed) return;

        if (CEO_ctrl.GetVulnerable())
        {
            if (!materialSetFlip)
            {
                GetComponent<Renderer>().material.CopyPropertiesFromMaterial(removableMaterial);
                materialSetFlip = true;
                gameObject.layer = (int)Mathf.Log(movable.value, 2);
            }

            if (MO_ctrl.GetYoink())
            {
                removed = true;
                rb.isKinematic = false;

                Vector3 tmpPos = transform.position;
                Quaternion tmpRot = transform.rotation;
                //Vector3 tmpParentPos = transform.parent.position;
                transform.parent = null;
                CEO_ctrl.GetAnimator().Rebind();
                transform.position = tmpPos;
                transform.rotation = tmpRot;

                CEO_ctrl.ItemRemoved(index);
            }
        }
        else
        {
            if (materialSetFlip)
            {
                GetComponent<Renderer>().material.CopyPropertiesFromMaterial(startMaterial);
                materialSetFlip = false;
                gameObject.layer = startLayer;
            }
        }
    }

    public void ResetObj()
    {
        removed = false;
        rb.isKinematic = true;
        MO_ctrl.SetYoink(false, grapple);
        GetComponent<Renderer>().material.CopyPropertiesFromMaterial(startMaterial);
        materialSetFlip = false;
        gameObject.layer = startLayer;
        transform.parent = startParent;
        CEO_ctrl.GetAnimator().Rebind();
    }

    public void SetIndex(int newIndex)
    {
        index = newIndex;
    }
}
