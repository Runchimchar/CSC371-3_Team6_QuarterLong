using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code taken from Sebastian Lague
public class FieldOfView : MonoBehaviour
{
    public NewDrone myNewDroneScript;

    public List<Transform> visibleTargets = new List<Transform>();
    public Transform Player;
    [SerializeField, Range(0.0f, 20.0f)] private float _viewRadius;
    [SerializeField, Range(0.0f, 360.0f)] private float _viewAngle;
    public float ViewRadius => _viewRadius;
    public float ViewAngle => _viewAngle;

    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstacleMask;
    public void Start()
    {
        StartCoroutine(DetectPlayerWithDelay(0.2f));
    }

    // Unity has unit circle with 0 at top and 90 at right
    //    compared to unit circle with 0 on right and 90 at top.
    //    So to get the normal unit circle angle from the unity angle,
    //    we do (90 - theta).
    //    Recall that sin(90 - theta) = cos(theta), so we do some substitutions
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void FindVisibleTargets()
    {
        visibleTargets.Clear();
        
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _targetMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < _viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, _obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    public void DetectPlayer()
    {
        visibleTargets.Clear();

        if (Vector3.Distance(Player.position, transform.position) < _viewRadius)
        {
            Vector3 dirToTarget = (Player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < _viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, Player.position);
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, _obstacleMask))
                {
                    visibleTargets.Add(Player);
                    myNewDroneScript.followPlayer = true;
                }
            }
        }
    }
    public IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    public IEnumerator DetectPlayerWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            DetectPlayer();
        }
    }
}
