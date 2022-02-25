using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [SerializeField] private float _pushForce = 1.0f;
    [SerializeField] private int _attackDamage = 1;
    [SerializeField] private float _attackCooldown = 5.0f;

    private float _attackTimer = 0.0f;
    private PlayerStatus _playerStatus;
    private Collider _collider;
    public void Start()
    {
        _playerStatus = FindObjectOfType<PlayerStatus>();
        _collider = GetComponent<Collider>();
        _attackTimer = _attackCooldown + 1.0f;
    }

    public void Update()
    {
        _attackTimer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // damage the player
            if (_attackTimer > _attackCooldown)
            {
                _playerStatus.ChangeHealth(-_attackDamage);
                _attackTimer = 0.0f;
            }

            // bump the player
            Vector3 forceDir = other.transform.position - _collider.ClosestPointOnBounds(other.transform.position);
            forceDir = Vector3.ProjectOnPlane(forceDir, Vector3.up);
            if (Mathf.Abs(forceDir.magnitude) < 0.001f)
                forceDir = transform.right;
            forceDir = forceDir.normalized;
            other.attachedRigidbody.AddForce(forceDir * _pushForce, ForceMode.Impulse);
        }
    }
}
