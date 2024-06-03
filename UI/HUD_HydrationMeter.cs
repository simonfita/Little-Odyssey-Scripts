using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_HydrationMeter : MonoBehaviour
{
    public Slider bar;

    private Hydration hydration;

    public Image background;

    public Image additionalImage;

    public Color dangerColor;

    public float dangerFrequency;
    public float dangerStart;


    private void Start()
    {
        hydration = Refs.turtle.hydration;
    }

    private void Update()
    {
        float value = hydration.currentHydration / hydration.maxHydration;
        
        bar.value = value;

        float danger = 1 -  Mathf.Clamp(value * (1 / dangerStart), 0, 1);
        float sin = Mathf.Sin(Time.time * dangerFrequency);

        background.color = Color.Lerp(Color.white,dangerColor,danger*sin);

    }

}
