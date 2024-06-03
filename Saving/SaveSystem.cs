using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

/*  Order of loading:
 *  GameStats
 *  TransporterLicences
 *  ContractInstances
 *  player, turtle, noticeboards, npcs, worldtime
 *  Contracts
 *  
 *  NOT SAVED:
 *  hat
 *  moving by ship
 *  sandstorms
 *  turtle boosts
*/

//TODO: make it static
public class SaveSystem : MonoBehaviour
{

    public enum SaveDisablingFlag
    { 
        Sailing, Sandstorm, Ending,
    }

    public static HashSet<SaveDisablingFlag> DisablingFlags;

    private void Awake()
    {
        DisablingFlags = new HashSet<SaveDisablingFlag>();
    }


    private void Update()
    {
        if (Keyboard.current.f5Key.wasPressedThisFrame)
            SaveGame("quicksave");
        if (Keyboard.current.f9Key.wasPressedThisFrame)
            LoadGame("quicksave");
    }

    public static bool CanSave()
    {
        return DisablingFlags.Count == 0;
    }

    public void SaveGame(string fileName)
    {
        if (!CanSave())
        {
            Refs.ui.hud.SpawnGameMessege("I can't save now!");
            return;
        }

        Save save = new Save();

        TransporterLicences.OnSave(save);

        foreach (var obj in SaveableBehaviour.all)
        {
            try
            {
                obj.OnSave(save);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        string filePath = Application.persistentDataPath + "/" + fileName + ".txt";
        StreamWriter sw = File.CreateText(filePath);
        sw.Write(save.ToString());
        sw.Close();

        
    }

    public void LoadGame(string fileName)
    {
        string filePath = Application.persistentDataPath + "/" + fileName + ".txt";

        if (!File.Exists(filePath))
            return;
        string saveText = File.ReadAllText(filePath);
        Save save = Save.FromJson(saveText);

        TransporterLicences.OnLoad(save);

        List<SaveableBehaviour> sortedSaveables = SaveableBehaviour.all;
        sortedSaveables.Sort(SaveableBehaviour.CompareByPriority);

        foreach (var obj in sortedSaveables)
        {
            try
            {
                obj.OnLoad(save);
            }
            catch (System.Exception e)
            { 
                Debug.LogException(e);
            }
        }

        Refs.ui.inputHints.Deactivate();
        Refs.playerCamera.TeleportToDesired();
    }

    public static string[] LoadSaveNames()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);

        var files = dir.GetFiles().Where(x =>x.Extension == ".txt" &&x.Name.StartsWith("Save")).OrderBy(p => p.LastWriteTime).Reverse();

        return files.Select(x =>x.Name.Replace(".txt","")).ToArray();

    }
}





