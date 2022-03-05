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
    Material playerMat;
    string dither = "_DitherStrength";
    float ditherPercent = 0;

    private void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        cam = FindObjectOfType<CinemachineBrain>();
        playerMat = vcam.Follow.parent.Find("Capsule").GetComponent<Renderer>().material;
        playerMat.SetFloat(dither, ditherPercent);
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
    }
}
