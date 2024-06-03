using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class UI_PauseMenu : MonoBehaviour
{

    public PlayableDirector introCutscene;

    private bool isOpened;

    [SerializeField]
    private Canvas canvas;

    public UI_Options options;
    public UI_LoadingMenu loadingMenu;
    public GameObject controls,loading;
    public List<Button> saveDependant;
    private GameObject submenu;

    private void Awake()
    {
        canvas.enabled = false;
        loading.SetActive(false);
    }

    private void Start()
    {
        Refs.controls.UI.Pause.performed += OpenClose;
    }

    private void OpenClose(InputAction.CallbackContext context)
    {
        if (introCutscene.state == PlayState.Playing)
        {
            introCutscene.time = introCutscene.playableAsset.duration; 
            introCutscene.Evaluate();
            introCutscene.Stop();
            return;
        }

        if (submenu != null)
        {
            CloseSubmenu();
            return;
        }

        if (isOpened)
            Close();
        else
            Open();
    }

    public void Open()
    {
        canvas.enabled = true;
        Time.timeScale = 0f;
        isOpened = true;
        Refs.gamepadController.RequestPointingMode("Pause");
        Refs.ui.inputHints.CompletedAction("Pause");

        foreach (var butt in saveDependant)
        {
            butt.interactable = SaveSystem.CanSave();
        }
    }

    public void Close()
    {
        canvas.enabled = false;
        Time.timeScale = 1.0f;
        isOpened = false;
        Refs.gamepadController.UnrequestPointingMode("Pause");
    }

    public void OpenSubmenu(GameObject _submenu)
    {
        if (submenu != null)
            Debug.LogError("Trying to open another submenu!");

        submenu = _submenu;
        submenu.SetActive(true);
    }
    public void CloseSubmenu()
    { 
        submenu.SetActive(false);
        submenu = null;
    }

    public void Save()
    {
        string saveName = "Save "+System.DateTime.Now.ToString("U").Replace(':',';');
        FindObjectOfType<SaveSystem>().SaveGame(saveName);
        Close();
    }

    private void OnDestroy()
    {
        Refs.controls.UI.Pause.performed -= OpenClose;
    }

    public void ExitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Save();
         Application.Quit();
#endif


    }
}
