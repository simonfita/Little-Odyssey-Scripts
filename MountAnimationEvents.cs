using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountAnimationEvents : MonoBehaviour
{
    public List<ParticleSystem> stepParticles;

    public AudioSource stepSound;
    public void Step(int foot)
    { 
        stepSound.Play();
        stepParticles[foot].Play();
    }
}
