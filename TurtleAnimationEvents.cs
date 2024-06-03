using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleAnimationEvents : MonoBehaviour
{
    public GameObject stepParticle;
    public List<Transform> feet;

    public AudioSource leftStep, righStep;

    public void Step(int i)
    {
        if (i == 0)
            leftStep.Play();
        if (i == 1)
            righStep.Play();
        Instantiate(stepParticle, feet[i].position, Quaternion.identity);
    }
}
