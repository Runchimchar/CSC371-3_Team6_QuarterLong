using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossOpenFlaps : MonoBehaviour
{
    public Animator bossAnim;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openFlaps()
    {
        bossAnim.SetTrigger("Open");
    }
    public void closeFlaps()
    {
        bossAnim.SetTrigger("Close");
    }
}
