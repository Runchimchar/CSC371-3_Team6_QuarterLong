using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenOnButton : MonoBehaviour
{
    public ButtonControls bc;
    public bool doorOpen = false;
    private ADoorAnimation _doorAnimator;

    // Start is called before the first frame update
    void Start()
    {
        _doorAnimator = GetComponent<ADoorAnimation>();
        if (_doorAnimator == null)
            Debug.LogError("No Door Animator Found");
        // Sets the door to toggle open/closed on button press event
        bc.OnButtonActivate += ToggleDoor;
    }

    void ToggleDoor()
    {
        print("Called ToggleDoor");
        if (doorOpen)
        {
            _doorAnimator.PlayClose();
            print("Closing door!");
            doorOpen = false;
        }
        else
        {
            _doorAnimator.PlayOpen();
            print("Opening door!");
            doorOpen = true;
        }
    }
}
