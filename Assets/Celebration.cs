using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Celebration : MonoBehaviour
{
    public GameObject WinTextCanvasPrefab;
    public GameObject CelebrationParticlesPrefab;
    public Vector3 ParticleOffset;

    public void OnTriggerEnter(Collider other)
    {
        GameObject winText = Instantiate(WinTextCanvasPrefab),
            particles = Instantiate(CelebrationParticlesPrefab);
        winText.transform.SetParent(null);
        particles.transform.SetParent(null);
        particles.transform.position = transform.position + ParticleOffset;
        Destroy(this);
    }
}
