using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RepeatingSound : MonoBehaviour
{
    public float minDelay, maxDelay;

    public AudioSource src;

    private bool invoked;

    private void Update()
    {
        if (!src.isPlaying && !invoked)
        {
            Invoke(nameof(Play), Random.Range(minDelay, maxDelay));
            invoked = true;
        }
    }

    private void Play()
    {
        invoked = false;
        src.Play();
    }
    
}
