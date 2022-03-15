using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCameraController : MonoBehaviour
{
    [SerializeField, Tooltip("Mimimum distance before player material begins to dither")] float minDistanceNoDither = 3f;
    [SerializeField, Tooltip("Distance at which player material is fully dithered")] float fullDitherDistance = 1f;
    CinemachineVirtualCamera vcam;
    CinemachineBrain cam;
    Material playerMat, gunMat1, gunMat2, gunMat3;
    string dither = "_DitherStrength";
    float ditherPercent = 0;

    private void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        cam = FindObjectOfType<CinemachineBrain>();
        playerMat = vcam.Follow.parent.Find("Capsule").GetComponent<Renderer>().material;
        gunMat1 = vcam.Follow.parent.Find("Capsule/Grapple/GrappleGun/Magazine").GetComponent<Renderer>().material;
        gunMat2 = vcam.Follow.parent.Find("Capsule/Grapple/GrappleGun/Pistol").GetComponent<Renderer>().material;
        gunMat3 = vcam.Follow.parent.Find("Capsule/Grapple/GrappleGun/Pistol_part").GetComponent<Renderer>().material;
        playerMat.SetFloat(dither, ditherPercent);
        gunMat1.SetFloat(dither, ditherPercent);
        gunMat2.SetFloat(dither, ditherPercent);
        gunMat3.SetFloat(dither, ditherPercent);
    }

    private void LateUpdate()
    {
        ApplyDistance();
    }

    void ApplyDistance()
    {
        float distance = Vector3.Distance(cam.transform.position, vcam.Follow.position);
        ditherPercent = Mathf.Clamp(minDistanceNoDither - distance, 0, minDistanceNoDither - fullDitherDistance) / (minDistanceNoDither - fullDitherDistance);
        playerMat.SetFloat(dither, ditherPercent);
        gunMat1.SetFloat(dither, ditherPercent);
        gunMat2.SetFloat(dither, ditherPercent);
        gunMat3.SetFloat(dither, ditherPercent);
    }
}
