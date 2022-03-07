using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimation : MonoBehaviour
{
    public Animator bossAnim;

    public void openFlaps()
    {
        bossAnim.SetTrigger("Open");
    }
    public void closeFlaps()
    {
        bossAnim.SetTrigger("Close");
    }
}
