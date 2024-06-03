using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniLight : MonoBehaviour
{
    private void Start()
    {
        FindObjectOfType<WeatherSystem>().AddMiniLight(transform.position);
    }
}
