using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightningBoltScript = DigitalRuby.LightningBolt.LightningBoltScript;
using System;

public class DroneAttack : MonoBehaviour
{
    [SerializeField] private int  _attackDamage = 1;
    [SerializeField] private GameObject _lightningPrefab;
    [SerializeField, Range(0.0f, 20.0f)] private float _attackDuration = 1.0f;
    private PlayerStatus _playerStatus;

    private event Action _droneAttacks;

    public void SubscribeToDroneAttacksEvent(Action fun)
    {
        _droneAttacks += fun;
    }

    public void UnsubscribeToDroneAttacksEvent(Action fun)
    {
        _droneAttacks -= fun;
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerStatus = FindObjectOfType<PlayerStatus>();
    }

    public void Attack(GameObject player)
    {
        if (_droneAttacks != null)
            _droneAttacks.Invoke();
        if (_playerStatus != null)
            _playerStatus.ChangeHealth(-_attackDamage);
        GameObject lightningObject = Instantiate(_lightningPrefab);
        LightningBoltScript lightning = lightningObject.GetComponent<LightningBoltScript>();
        lightning.StartObject = gameObject;
        lightning.EndObject = player;
        lightning.Duration = 0.01f;
        Destroy(lightningObject, _attackDuration);
    }
}
