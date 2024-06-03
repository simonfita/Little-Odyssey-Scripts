using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{

    readonly string[] alwaysLoaded = { "TongsDelta"};

    public static void Restart()
    {
        Debug.LogWarning("Restarting");
        SceneLoadInfo.loadingSave = false;
        SceneLoadInfo.saveName = null;
        Time.timeScale = 1;
        SceneManager.LoadScene("Persistant");

    }

    public static void RestartWithSave(string saveName)
    {
        Debug.LogWarning("Loading save");
        SceneLoadInfo.loadingSave = true;
        SceneLoadInfo.saveName = saveName;
        Time.timeScale = 1;
        SceneManager.LoadScene("Persistant");
    }


    private void Awake()
    {
        if (SceneManager.sceneCount == 1) //not the case in editor
        {
            foreach (string scene in alwaysLoaded)
            {

                SceneManager.LoadScene(scene, LoadSceneMode.Additive);
            }
        }
        

    }
    private void Start()
    {
        Refs.Generate();
        TransporterLicences.SetCurrentLicence(TransporterLicences.LicenceType.None);
        Invoke(nameof(LateStart), 0.1f);
    }

    private void LateStart()
    {
        if (SceneLoadInfo.loadingSave)
        {
            FindObjectOfType<SaveSystem>().LoadGame(SceneLoadInfo.saveName);
        }
        else
        {
            CutsceneManager.PlayCutscene("Intro");
        }
        SceneLoadInfo.loadingSave = false;
        SceneLoadInfo.saveName = null;
    }

}

public static class SceneLoadInfo
{
    public static bool loadingSave = false;
    public static string saveName = null;
}
