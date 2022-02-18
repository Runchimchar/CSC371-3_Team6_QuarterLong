using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource sound;

    public AudioClip clip;
    public float lengthOffset = 0f;
    public bool playOnStart = false;
    public bool repeat = false;

    float timer = -1f;
    bool repeating = false;

    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();

        if (playOnStart)
        {
            if (repeat) PlayRepeat();
            else PlayOnce();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (repeat && repeating) PlayRepeat();
    }

    public void PlayRepeat()
    {
        repeat = true;
        repeating = true;
        timer += Time.deltaTime;
        if (timer > clip.length + lengthOffset || timer < 0)
        {
            sound.PlayOneShot(clip);
            timer = 0f;
        }
    }

    public void PlayOnce()
    {
        repeating = false;
        sound.PlayOneShot(clip);
    }

    public void PlayStop()
    {
        repeat = false;
        repeating = false;
        timer = -1f;
        sound.Stop();
    }
}
