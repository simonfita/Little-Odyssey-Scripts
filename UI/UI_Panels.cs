using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UI_PanelType
{ 
    Journal,
    Backpack,
    Noticeboard,
    Dialogue,
    Readable

}

public class UI_Panels : MonoBehaviour
{
    public Transform leftPanelParent,centerPanelParent, rightPanelParent,noPanelParent;

    public Transform journalPanel, backpackPanel, noticeboardPanel, dialoguePanel, readablePanel;

    private Transform leftPanel, centerPanel,rightPanel;


    private void Update()
    {
        if (Refs.controls.UI.Journal.IsPressed())
        {
            OpenLeftPanel(UI_PanelType.Journal);
            Refs.ui.inputHints.CompletedAction("Journal");
        }
        else if (Refs.controls.UI.Backpack.IsPressed())
        {
            OpenLeftPanel(UI_PanelType.Backpack);
            Refs.ui.inputHints.CompletedAction("Backpack");
        }
        else
        {
            CloseLeftPanel();
        }
        if (Refs.controls.UI.Journal.WasPressedThisFrame())
            Refs.gamepadController.RequestPointingMode("Journal");       
        if (Refs.controls.UI.Journal.WasReleasedThisFrame())
            Refs.gamepadController.UnrequestPointingMode("Journal");
        if (Refs.controls.UI.Backpack.WasPressedThisFrame())
            Refs.gamepadController.RequestPointingMode("Backpack");
        if (Refs.controls.UI.Backpack.WasReleasedThisFrame())
            Refs.gamepadController.UnrequestPointingMode("Backpack");
    }



    private void OpenLeftPanel(UI_PanelType type)
    {
        if (leftPanel != null)
            CloseLeftPanel();

        Transform panel = null;

        switch (type)
        {
            case UI_PanelType.Backpack:
                panel = backpackPanel;
                break;
            case UI_PanelType.Journal:
                panel = journalPanel;
                break;
        }


        panel.SetParent(leftPanelParent,true);
        leftPanel = panel;


        if (centerPanel != null)
        {
            rightPanel = centerPanel;
            rightPanel.SetParent(rightPanelParent,true);
            centerPanel = null;
        }
    }

    private void CloseLeftPanel()
    {
        if (leftPanel == null)
            return;
        leftPanel.SetParent(noPanelParent,true);
        leftPanel = null;

        if (rightPanel != null)
        {
            centerPanel = rightPanel;
            centerPanel.SetParent(centerPanelParent, true);
            rightPanel = null;
        }


    }

    public void OpenStaticPanel(UI_PanelType type)
    {
        if (rightPanel != null || centerPanel != null)
        {
            Debug.LogError("Static panel already open!");
            return;
        }

        Transform panel;

        switch (type)
        {
            case UI_PanelType.Noticeboard:
                panel = noticeboardPanel;
                break;
            case UI_PanelType.Dialogue:
                panel = dialoguePanel;
                break;
            case UI_PanelType.Readable:
                panel = readablePanel;
                break;
            default:
                Debug.LogError("Can't open this as static panel");
                return;
        }

        if (leftPanel != null)
        {
            panel.SetParent(rightPanelParent,true);
            rightPanel = panel;
        }
        else
        {
            panel.SetParent(centerPanelParent,true);
            centerPanel = panel;
        }

        Refs.player.token.canMove = false;
        Refs.turtle.enabled = false;   
    }

    public void CloseStaticPanel()
    {
        if (centerPanel != null)
        {
            centerPanel.SetParent(noPanelParent,true);
            centerPanel = null;
        }
        else if (rightPanel != null)
        {
            rightPanel.SetParent(noPanelParent,true);
            rightPanel = null;
        }
        else
        {
            Debug.LogError("There is no static parent to remove!");
        }
        if(!Refs.turtle.isMounted)
        Refs.player.token.canMove = true;
        
        
        Refs.turtle.enabled = true;
    }
}
