using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save
{
    public string warning = "THIS IS A SAVE FILE";


    public Vector3 playerPos, turtlePos;
    public Quaternion turtleRot;
    public bool isMounted;

    public List<string> itemNames = new List<string>();
    public List<int> itemAmounts = new List<int>();

    public int money;
    public float hydration;

    public int mainQuestStage;
    public int licence;

    public float worldTime;
    public bool sanstormsEnabled;
    public float sandstormDelay;


    public ContestManager.ContestState contestSave;
    public List<NPC.CustomSave> npcs = new List<NPC.CustomSave>();
    public List<ContractInstanceBase.CustomSave> contracts = new List<ContractInstanceBase.CustomSave>();
    public List<NoticeBoard.CustomSave> noticeboards = new List<NoticeBoard.CustomSave>();
    public List<LivingPalm.CustomSave> palms = new List<LivingPalm.CustomSave>();

    public Save()
    {

    }

    public static Save FromJson(string text)
    {
        return JsonUtility.FromJson<Save>(text.Replace("\n", ""));
    }


    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }

    [System.Serializable]
    public abstract class UniqueSave
    {
        public string id;
    }

}
