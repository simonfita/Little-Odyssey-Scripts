using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    
    private void Awake()
    {
        AudioListener.volume = 0.5f;
        FindObjectOfType<GamepadController>().RequestPointingMode("MainMenu");
    }
    public void NewGame()
    {
        SceneManager.LoadScene("Persistant");
    }


    public void Quit()
    {
        Application.Quit();
    }

    
    public void OpenDiscord()
    {
        Application.OpenURL("https://discord.gg/gnB7cYKP8C");
    }
}
