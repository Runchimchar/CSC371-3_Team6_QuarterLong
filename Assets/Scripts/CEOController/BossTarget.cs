using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events = System.ValueTuple<CEOController.Event, CEOController.Event, CEOController.Event, CEOController.Event>;

public class BossTarget : MonoBehaviour
{
    public Transform location;
    public float speed = 1;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(location.position, 0.5f);
    }
}
