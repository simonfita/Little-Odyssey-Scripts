using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_OptionsSetting : MonoBehaviour
{
    public string settingName;

    public Slider slider;
    public Toggle toggle;

    private void OnEnable()
    {
        if (!PlayerPrefs.HasKey(settingName)) //default settings
            return;

        if (slider != null)
            slider.value = PlayerPrefs.GetFloat(settingName);
        else if (toggle != null)
            toggle.isOn = PlayerPrefs.GetInt(settingName)>0;

    }

    public void OnSliderChange(float value)
    {
        PlayerPrefs.SetFloat(settingName, value);
        UI_Options.ApplyOptions();
    }

    public void OnToggleChange(bool value)
    {
        PlayerPrefs.SetInt(settingName, value ? 1 : 0);
        UI_Options.ApplyOptions();
    }

}
