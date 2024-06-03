using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    public SerializedDictionary<string, PlayableDirector> cutscenes;

    public static void PlayCutscene(string cutscene)
    {
        Debug.Log(cutscene);
        FindObjectOfType<CutsceneManager>().cutscenes[cutscene].Play();
    }

    public static bool IsAnyCutscenePlaying()
    {
        foreach (var dir in FindObjectOfType<CutsceneManager>().cutscenes)
        {
            if (dir.Value.state == PlayState.Playing)
                return true;
        }
        return false;
    }
}
