using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeControlerScript : MonoBehaviour
{
    public AudioMixer mixer;

    // Get change in slider from 0 - 100
    public void UpdateVolume(float val) {
        // Map 0 -> 100 val to -80 -> 0
        //float mappedVal = val * (85f / 100f) - 80f;
        float mappedVal = 24f*Mathf.Log10(Mathf.Max(0.1f, val)) - 45f;
        //Debug.Log(mappedVal);
        mixer.SetFloat("volume", mappedVal);
    }
}
