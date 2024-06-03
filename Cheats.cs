using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cheats : MonoBehaviour
{
    public TMPro.TMP_InputField consoleInput;
    public List<Canvas> toDisable;

    private string[] cParams;

    private int playerSpeedMult = 1, turtleSpeedMult = 1;

    const int historyDepth = 9;
    private int historyIndex;

    private void Awake()
    {
        consoleInput.gameObject.SetActive(false);

        cParams = new string[50];
        for (int i = 0; i < cParams.Length; i++)
        {
            cParams[i] = "";
        }

    }

    private void Update()
    {
#if !DEMO
        if (Keyboard.current.backquoteKey.wasPressedThisFrame)
        {
            Refs.controls.Disable();
            consoleInput.gameObject.SetActive(true);
            consoleInput.ActivateInputField();
            consoleInput.text = "";
            historyIndex = 0;
        }
        if (consoleInput.IsActive())
        {

            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                historyIndex = Mathf.Clamp(historyIndex + 1, 1, historyDepth);
                consoleInput.text = RetreiveHistory(historyIndex);
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                historyIndex = Mathf.Clamp(historyIndex - 1, 1, historyDepth);
                consoleInput.text = RetreiveHistory(historyIndex);
            }
                       
        
        }
#endif
    }

    private string RetreiveHistory(int i)
    {
        if(0<i && i<=historyDepth)
            return PlayerPrefs.GetString("Cmd"+i, "");
        return "er";
    }

    private void EmplaceHistory(string s)
    {
        for (int i = historyDepth; i > 1; i--)
        {
            PlayerPrefs.SetString("Cmd"+i, PlayerPrefs.GetString("Cmd"+(i-1), ""));
        }
        PlayerPrefs.SetString("Cmd1", s);
    }

    public void ProcessConsole(string command)
    {
        Debug.Log(command);
        EmplaceHistory(command);

        string[] commands = command.Split(' ');

        commands.CopyTo(cParams, 0);

        for (int i = 0; i < cParams.Length; i++)
                cParams[i] = cParams[i].Replace('_', ' ');


        try
        {
            Invoke(commands[0].ToLower(), 0);
        }
        catch
        {
            Debug.LogError("There is no command " + commands[0]);
        }
        if (command == "help")
            return;

        consoleInput.gameObject.SetActive(false);
        consoleInput.DeactivateInputField();
        Refs.controls.Enable();
    }

    protected void reset()
    {
        SceneHandler.Restart();
    }

    protected void tp()
    {
        string locationName;
        switch (cParams[1])
        {
            case "0":
                locationName = "Eugene's Farm";
                break;
            case "1":
                locationName = "Greeting Town";
                break;
            case "2":
                locationName = "Star Town";
                break;
            case "3":
                locationName = "Emperor's City";
                break;
            default:
                locationName = cParams[1];
                break;
        }
        foreach(LocationBase loc in FindObjectsOfType<LocationBase>())
        {
            if (loc.locationName == locationName)
            {
                Refs.turtle.Mount();
                Refs.turtle.Teleport(loc.GetPosition());
                return;
            }
        }
        Debug.LogError("Can't find location "+locationName);

    }

    protected void tpdud()
    {
        FindObjectOfType<DudSystem>().tpMode = int.Parse(cParams[1])>0;
    
    }

    protected void give()
    {
        Refs.inventory.AddItem(cParams[1]);
    }

    protected void allupgrades()
    {
        Refs.inventory.AddItem("WateringCan");
        Refs.inventory.AddItem("HydrationTank");
        Refs.inventory.AddItem("HydrationTank");
        Refs.inventory.AddItem("LeftPack");
        Refs.inventory.AddItem("RightPack");
    }

    protected void pspeed()
    {
        int newSpeed = int.Parse(cParams[1]);
        Refs.player.basePlayerSpeed /= playerSpeedMult;
        Refs.player.basePlayerSpeed *= newSpeed;
        playerSpeedMult = newSpeed;
        
    }

    protected void tspeed()
    {
        int newSpeed = int.Parse(cParams[1]);
        Refs.turtle.mountSpeed /= turtleSpeedMult;
        Refs.turtle.mountSpeed *= newSpeed;
        turtleSpeedMult = newSpeed;
    }

    protected void speed()
    {
        int newSpeed = int.Parse(cParams[1]);
        Refs.player.basePlayerSpeed /= playerSpeedMult;
        Refs.player.basePlayerSpeed *= newSpeed;
        playerSpeedMult = newSpeed;

        newSpeed = int.Parse(cParams[1]);
        Refs.turtle.mountSpeed /= turtleSpeedMult;
        Refs.turtle.mountSpeed *= newSpeed;
        turtleSpeedMult = newSpeed;
    }

    protected void money()
    {
        Refs.inventory.playerMoney.amount += int.Parse(cParams[1]);
    }

    protected void hydration()
    {
        Refs.turtle.hydration.currentHydration += int.Parse(cParams[1]);
    }

    protected void mainquest()
    {
        MainQuest.MainQuestStage stage = System.Enum.Parse<MainQuest.MainQuestStage>(cParams[1]);

        /*for (int i = 0; i < (int)stage-1; i++)
        {
            foreach (var con in Refs.mainQuest.StageDescriptions[(MainQuest.MainQuestStage)i].contractsToComplete)
            {
                if (con.unique)
                {
                    con.state = ContractState.Finished;
                    con.enabled = false;
                }
            }
        }*/

        Refs.mainQuest.TrySetStage(stage);
       
    }

    protected void refreshcontracts()
    {
        foreach (NoticeBoard noticeBoard in FindObjectsOfType<NoticeBoard>())
        {
            noticeBoard.Progress();
        }
    }

    protected void ui()
    {
        if (cParams[1] == "1")
        {
            foreach(Canvas canv in toDisable)
                canv.enabled = true;
        }
        else if (cParams[1] == "0")
        {
            foreach (Canvas canv in toDisable)
                canv.enabled = false;
        }
    }
    protected void maxfps()
    {
        Application.targetFrameRate = int.Parse(cParams[1]);
    }


    protected void settime()
    { 
        WorldTime.CurrentDayTime = float.Parse(cParams[1]); 
    }

    protected void timescale()
    {
        WorldTime.TimeScale = float.Parse(cParams[1]);
    }

    protected void contract()
    {
        foreach (ContractInstanceBase contract in FindObjectsOfType<ContractInstanceBase>())
        {
            if (contract.gameObject.name == cParams[1])
            {
                Refs.contracts.AddContract(contract);
                return;
            }


        }
    
    }

    protected void licence()
    {
        TransporterLicences.SetCurrentLicence((TransporterLicences.LicenceType)int.Parse(cParams[1]));
    }

    protected void save()
    {
        FindObjectOfType<SaveSystem>().SaveGame(cParams[1]);
    }
    
    protected void load()
    {
        SceneHandler.RestartWithSave(cParams[1]);
    }

    protected void call()
    {
        GameObject.Find(cParams[1]).SendMessage(cParams[2], SendMessageOptions.RequireReceiver);
    }

    protected void test()
    {
    }


    private void help()
    {
        MethodInfo[] mInfos = typeof(Cheats).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
        string command = "";
        Debug.Log(mInfos.Length);
        foreach (MethodInfo info in mInfos)
        {
            if (info.DeclaringType == typeof(Cheats) && info.IsFamily)
                command += info.Name + " ";
        }
        consoleInput.text=command;
    }
}
