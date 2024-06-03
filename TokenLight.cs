using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenLight: MonoBehaviour
{
    [SerializeField]
    Light sunLight;

    [SerializeField]
    Gradient sunGradient;



    private void Update()
    {
        float t = WorldTime.GetDayPercent();
        sunLight.color = sunGradient.Evaluate(t);


    }
}
