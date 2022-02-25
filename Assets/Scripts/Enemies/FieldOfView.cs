using System;
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
    [SerializeField, Range(0.001f, 10.0f)] private float _minViewRadius;
    [SerializeField, Range(0.001f, 6.0f)] private float _attackRadius;
    [SerializeField, Range(0.0f, 360.0f)] private float _viewAngle;
    [SerializeField, Range(0.0f, 360.0f)] private float _attackAngle;
    [SerializeField, Range(0.0f, 30.0f)] private float _attackCooldown = 5.0f;
    private float _attackTimer = 100.0f;
    private DroneAttack _droneAttack;
    private event Action _visionEvent;

    public float ViewRadius => _viewRadius;
    public float MinViewRadius => _minViewRadius;
    public float AttackRadius => _attackRadius;
    public float ViewAngle => _viewAngle;
    public float AttackAngle => _attackAngle;

    [SerializeField] private LayerMask _obstacleMask;

    public void SubscribeToVisionEvent(Action fun)
    {
        _visionEvent += fun;
    }

    public void UnsubscribeToVisionEvent(Action fun)
    {
        _visionEvent -= fun;
    }

    public void Start()
    {
        _droneAttack = GetComponent<DroneAttack>();
        StartCoroutine(DetectPlayerWithDelay(0.2f));
    }

    public void Update()
    {
        _attackTimer += Time.deltaTime;
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

    public void DetectPlayer()
    {
        visibleTargets.Clear();
        float playerDist = Vector3.Distance(Player.position, transform.position);
        if (_droneAttack != null && playerDist < _attackRadius && _attackTimer > _attackCooldown)
        {
            Vector3 dirToTarget = (Player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < _attackAngle)
            {
                float distToTarget = Vector3.Distance(transform.position, Player.position);
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, _obstacleMask))
                {
                    visibleTargets.Add(Player);
                    _droneAttack.Attack(Player.gameObject);
                    _attackTimer = 0.0f;
                    if (_visionEvent != null)
                        _visionEvent.Invoke();
                    return;
                }
            }
        }
        else if (playerDist < _viewRadius)
        {
            Vector3 dirToTarget = (Player.position - transform.position).normalized;
            if (playerDist < _minViewRadius || Vector3.Angle(transform.forward, dirToTarget) < _viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, Player.position);
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, _obstacleMask))
                {
                    visibleTargets.Add(Player);
                    if (_visionEvent != null)
                        _visionEvent.Invoke();
                    return;
                }
            }
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
