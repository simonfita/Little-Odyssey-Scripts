using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeastManager : MonoBehaviour
{
    public GameObject openGate, closedGate;
    public NPC salomon,sadiki;
    public LocationMeetingArea feastArea,sadikiArea;
    public Transform tpPosition;


    public void UpdateStage(MainQuest.MainQuestStage stage)
    {

        if (stage >= MainQuest.MainQuestStage.D4) 
        { 
            openGate.SetActive(true); 
            closedGate.SetActive(false); 
        }

        salomon.token.SetActive(stage >= MainQuest.MainQuestStage.D6);

        if (stage == MainQuest.MainQuestStage.D4)
        {
            AttendMeeting meeting = sadiki.schedule.AddOverwrideTask<AttendMeeting>();
            meeting.meetingArea = sadikiArea;
        }


        if (stage == MainQuest.MainQuestStage.D5)
        { 
            StartFeast(); 
        }
        
        
        if (stage == MainQuest.MainQuestStage.D8)
        { 
            EndFeast(); 
        }


    }


    private void StartFeast()
    {
        CutsceneManager.PlayCutscene("Feast");
        SaveSystem.DisablingFlags.Add(SaveSystem.SaveDisablingFlag.Ending);
        foreach (var npc in FindObjectsOfType<NPC>())
        {
            if (npc == sadiki || npc == salomon)
                continue;

            AttendMeeting meeting = npc.schedule.AddOverwrideTask<AttendMeeting>();
            meeting.meetingArea = feastArea;
        }
        Invoke(nameof(TPNPCs),0.1f);

    }

    private void TPNPCs()//hacky solution with delay
    {
        foreach (var npc in FindObjectsOfType<NPC>())
        {
            if (npc == sadiki || npc == salomon)
                continue;

            npc.movement.Teleport(tpPosition.position);

        }
    }

    private void EndFeast()
    {
        SaveSystem.DisablingFlags.Remove(SaveSystem.SaveDisablingFlag.Ending);
        foreach (var npc in FindObjectsOfType<NPC_Schedule>())
        {
            if (npc == salomon)
                continue;
            npc.RemoveOverrideTask();
        }
        CutsceneManager.PlayCutscene("Credits");

    }

}
