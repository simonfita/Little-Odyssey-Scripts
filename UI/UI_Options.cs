using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class UI_Options : MonoBehaviour
{
    public AudioMixer audioMixer;

    private static UI_Options currentOptions;

    private void OnEnable()
    {
        currentOptions = this;
        
    }
    private void Start()
    {
        ApplyOptions();
        gameObject.SetActive(false);
    }

    public static void ApplyOptions()
    {
        currentOptions.audioMixer.SetFloat("volumeMusic", Mathf.Lerp(-40,20,PlayerPrefs.GetFloat("volumeMusic",0)));
        currentOptions.audioMixer.SetFloat("volumeAmbient", Mathf.Lerp(-40,20,PlayerPrefs.GetFloat("volumeAmbient",0)));
        currentOptions.audioMixer.SetFloat("volumeEffects", Mathf.Lerp(-40,20,PlayerPrefs.GetFloat("volumeEffects",0)));

        var cam = FindObjectOfType<PlayerCamera>(); //quick hack
        if (cam != null)
        {
            cam.invertX = PlayerPrefs.GetInt("invertCameraX", 0) == 1;
            cam.invertY = PlayerPrefs.GetInt("invertCameraY", 0) == 1;
            cam.camSpeed = PlayerPrefs.GetFloat("cameraSpeed", 1);
        }


    }
}
