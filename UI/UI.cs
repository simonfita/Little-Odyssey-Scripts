using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public HUD hud;
    public UI_Panels panels;

    public UI_Dialogue dialogue;
    public UI_Noticeboard noticeboard;
    public UI_Readable readable;
    public UI_InputHints inputHints;

    public Transform labelsCanvas;

    public void StartDialogue(NPC npc)
    {
        panels.OpenStaticPanel(UI_PanelType.Dialogue);
        dialogue.StartDialogue(npc);
    }
    public void EndDialogue()
    {
        panels.CloseStaticPanel();

        dialogue.EndDialogue();
    }

    public void OpenNoticeBoard(ContractInstanceBase[] contracts)
    {
        panels.OpenStaticPanel(UI_PanelType.Noticeboard);
        noticeboard.OpenNoticeBoard(contracts);
    }

    public void OpenReadable(string text)
    {
        panels.OpenStaticPanel(UI_PanelType.Readable);
        readable.OpenReadable(text);
    }
}
