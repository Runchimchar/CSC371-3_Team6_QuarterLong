using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDoorAnimator : ADoorAnimation
{
    [SerializeField] private LeftDoorAnimation _leftDoor;
    [SerializeField] private RightDoorAnimation _rightDoor;

    public override void PlayClose()
    {
        _leftDoor.PlayClose();
        _rightDoor.PlayClose();
    }

    public override void PlayOpen()
    {
        _leftDoor.PlayOpen();
        _rightDoor.PlayOpen();
    }

    public override void ResetAnimation()
    {
        _leftDoor.ResetAnimation();
        _rightDoor.ResetAnimation();
    }
}
