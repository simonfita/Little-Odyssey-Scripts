using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_LocationDisplay : MonoBehaviour
{
    public TMPro.TMP_Text text;
    public Gradient colorAnim;
    public float animTime;

    public void DisplayLocation(string locationName)
    {
        text.text = locationName;
        StartCoroutine(PlayFading());
    }

    private IEnumerator PlayFading()
    {
        while(CutsceneManager.IsAnyCutscenePlaying()) //cutscene hack
            yield return null;

        for (float t = 0; t < animTime; t+=Time.deltaTime)
        {
            text.color = colorAnim.Evaluate(t / animTime);
            yield return null;
        }

        yield break;
    
    }
}
