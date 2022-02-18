using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Guns;

public class TestPlayer : MonoBehaviour
{
    private AGun gun;

    private void Start()
    {
        gun = FindObjectOfType<AGun>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Shooting");
            gun.Shoot(Vector3.zero);
        }
    }
}
