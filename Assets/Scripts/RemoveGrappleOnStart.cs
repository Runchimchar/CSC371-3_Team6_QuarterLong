using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveGrappleOnStart : MonoBehaviour
{
    public bool grappleVal = false;
    // Start is called before the first frame update
    void Start()
    {
        // Removes the grapple on the start of the level
        PlayerStatus ps = GameController.playerStatus;
        ps.grappleStatus = grappleVal;
    }
}
