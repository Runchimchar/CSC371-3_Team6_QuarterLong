using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenTimed : MonoBehaviour
{
    public ButtonControls bc;
    public bool doorOpen = false;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // Sets the door to toggle open/closed on button press event
        bc.OnButtonActivate += ToggleDoor;
        bc.OnButtonDeactivate += ToggleDoor;
    }

    void ToggleDoor()
    {
        if (doorOpen)
        {
            animator.Play("DoorClose");
            print("Closing door!");
            doorOpen = false;
        }
        else
        {
            animator.Play("DoorOpen");
            print("Opening door!");
            doorOpen = true;
        }
    }
}
