using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ADoorAnimation : MonoBehaviour
{
    public abstract void PlayClose();
    public abstract void PlayOpen();
    public abstract void ResetAnimation();
}
