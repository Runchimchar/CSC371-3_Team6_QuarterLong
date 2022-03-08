using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events = System.ValueTuple<CEOController.Event, CEOController.Event, CEOController.Event, CEOController.Event>;

public class BossTarget : MonoBehaviour
{
    public Transform location;
    public float speed = 1;
    //public CEOController.Activity activity;
    //public CEOController.DoneWaiting doneWaiting;

    //void Activity()
    //{
    //    activity?.Invoke();
    //}

    //bool DoneWaiting()
    //{
    //    bool? tmp = doneWaiting?.Invoke();
    //    return !tmp.HasValue || tmp.Value;
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(location.position, 0.5f);
    }
}
