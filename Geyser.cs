using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geyser : MonoBehaviour
{
    public float minDelay, maxDelay;
    public AudioSource src;
    public ParticleSystem particles;

    private void Start()
    {
        Invoke(nameof(Explode), Random.Range(minDelay, maxDelay));
    }

    private void Explode()
    {
        src.Play();
        particles.Play();
        Invoke(nameof(Explode),Random.Range(minDelay,maxDelay));
    }
}
