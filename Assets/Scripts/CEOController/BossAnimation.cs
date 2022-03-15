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
    public bool isFlapsOpen()
    {
        return bossAnim.GetCurrentAnimatorStateInfo(10).IsName("nozzle_l_JUST_on");
    }
}
