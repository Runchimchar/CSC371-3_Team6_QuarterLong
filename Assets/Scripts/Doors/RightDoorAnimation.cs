using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightDoorAnimation : ADoorAnimation
{
    public bool DoesStartClosed = true;
    private bool _isClosed;
    private Quaternion _baseRotation;
    private Animator _animator;
    private void Start()
    {
        _isClosed = DoesStartClosed;
        _baseRotation = transform.rotation;
        _animator = GetComponent<Animator>();
    }

    public override void PlayClose()
    {
        if (!_isClosed)
            _animator.Play("RightDoorClose");
        _isClosed = true;
    }

    public override void PlayOpen()
    {
        if (_isClosed)
            _animator.Play("RightDoorOpen");
        _isClosed = false;
    }

    public override void ResetAnimation()
    {
        _isClosed = DoesStartClosed;
        transform.rotation = _baseRotation;
        _animator.Play("DoorBaseRotation");
    }
}
