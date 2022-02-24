using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAttack : MonoBehaviour
{
    [SerializeField] private int  _attackDamage = 1;
    private PlayerStatus _playerStatus;
    // Start is called before the first frame update
    void Start()
    {
        _playerStatus = FindObjectOfType<PlayerStatus>();
    }

    public void Attack()
    {
        _playerStatus.ChangeHealth(-_attackDamage);
    }
}
