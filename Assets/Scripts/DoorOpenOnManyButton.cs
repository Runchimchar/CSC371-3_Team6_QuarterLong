using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenOnManyButton : MonoBehaviour
{
    public ButtonControls[] bcs;
    int buttonsActive;
    bool doorOpen = false;
    private ADoorAnimation _doorAnimator;

    // Start is called before the first frame update
    void Start()
    {
        _doorAnimator = GetComponent<ADoorAnimation>();
        if (_doorAnimator == null)
            Debug.LogError("No Door Animator Found");
        // Sets the door to toggle open/closed on button press event
        foreach(var bc in bcs)
        {
            bc.OnButtonActivate += IncrButtonsActive;
            bc.OnButtonDeactivate += DecrButtonsActive;
        }
    }

    void IncrButtonsActive()
    {
        // Each time a button activates, increment this counter
        buttonsActive++;
        if (buttonsActive == bcs.Length)
        {
            ToggleDoor();
        }
    }
    void DecrButtonsActive()
    {
        // Each time a button deactivates, increment this counter
        buttonsActive--;
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
