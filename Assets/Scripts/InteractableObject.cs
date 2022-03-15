using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractableObject : MonoBehaviour
{
    public event Action ResetObject = delegate { }; // user turns it on
    
    // Start is called before the first frame update
    void Start()
    {
        LeverControls lc;
        ButtonControls bc;
        DoorOnLever dlc;
        DoorOpenOnButton dbc;
        MovableObjectRespawn mc;
        if (lc = GetComponent<LeverControls>())
        {
            ResetObject += lc.Reset;
        }
        else if (bc = GetComponent<ButtonControls>())
        {
            ResetObject += bc.Reset;
        }
        else if (dlc = GetComponent<DoorOnLever>())
        {
            ResetObject += dlc.Reset;
        }
        else if (dbc = GetComponent<DoorOpenOnButton>())
        {
            ResetObject += dbc.Reset;
        }
        else if (mc = GetComponent<MovableObjectRespawn >())
        {
            ResetObject += mc.Reset;
        }
        else
        {
            Debug.LogError("Interactable object script attached to undefined object: " + this.name);
        }
    }

    public void ResetObjectNow()
    {
        ResetObject();
    }
}
