using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DreamManager
{
    static public void StartDream()
    {
        Refs.controls.Disable();

        AsyncOperation loadingScene = SceneManager.LoadSceneAsync("DreamMinigame",LoadSceneMode.Additive);
        loadingScene.completed += SceneLoaded;
        
        

    }

    private static void SceneLoaded(AsyncOperation obj)
    {
        GameObject.FindObjectOfType<DreamMinigame>().OnMinigameLost += EndDream;
    }

    static public void EndDream(int score)
    {
        WorldTime.CurrentDayTime = 900;
        Refs.controls.Enable();
        SceneManager.UnloadSceneAsync("DreamMinigame");
    }
}
