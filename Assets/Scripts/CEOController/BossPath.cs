using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPath : MonoBehaviour
{
    public string pathName;
    [SerializeField] BossTarget[] path;
    public bool loopPath = true;

    private void Start()
    {
        path = GetComponentsInChildren<BossTarget>();
    }

    private void OnDrawGizmosSelected()
    {
        path = GetComponentsInChildren<BossTarget>();
        BossTarget prevTarget = null;

        foreach (BossTarget target in path)
        {
            Gizmos.color = Color.green;
            if (prevTarget) Gizmos.DrawLine(target.location.position, prevTarget.location.position);
            prevTarget = target;
        }

        if (loopPath && prevTarget && path[0] != prevTarget) Gizmos.DrawLine(prevTarget.location.position, path[0].location.position);
    }
}
