using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class DeliverMailInstance : ContractInstanceBase
{
    public List<NPC> possibleTo;

    private List<NPC> to = new List<NPC>();

    public override void OnStart(ContractColor flag)
    {              
        base.OnStart(flag);
    }

    protected override void OnStartingNPCTalked()
    {
        base.OnStartingNPCTalked();
        Item mail = ItemsDatabase.GetItem("Mail");
        Refs.inventory.AddItem(mail, to.Count);
        foreach (NPC npc in to)
        {
            npc.involvedInContracts.Add(this);

            //I USED BECUASE IT'S SO AWFUL IT'S FUNNY
            System.Action<ContractInstanceBase> selfDeletingLambda = null;                
            selfDeletingLambda = (ContractInstanceBase contract) => { if (contract == this) { GaveMail(npc); npc.speech.OnTalkedAboutContract -= selfDeletingLambda; } }; ;
            npc.speech.OnTalkedAboutContract += selfDeletingLambda;

        }
    }

    public void GaveMail(NPC npc)
    {
        npc.involvedInContracts.Remove(this);
        to.Remove(npc);

        Refs.inventory.RemoveItem("Mail");
        Refs.contracts.OnContractsUpdated.Invoke();

        if(to.Count==0)
            OnFinish();
    
    }

   public override void OnFinish()
   {
        base.OnFinish();
   }

    public override bool TryCleanupAndSetup()
    {
        if (!base.TryCleanupAndSetup())
            return false;


        to.AddRange(possibleTo);
        
        return true;

    }

    public override string GetDialogueStartingKnot(NPC npc)
    {
        if (to.Contains(npc))
            return gameObject.name+"Receive";
        else
            return base.GetDialogueStartingKnot(npc);
    }

    public override string GetDescription()
    {
        if(state == ContractState.NPCWaiting)   
            return "Get mail from " + startingNPC.data.name;

        string returns = "Deliver mail to ";
        foreach (NPC npc in to)
        {
            returns += npc.data.name + ", ";
        }
        returns = returns.Substring(0, returns.Length - 2);
        return returns;
    }
    public override int GetReward()
    {
        return Mathf.RoundToInt(3 * rewardMultiplier * firstTimeBonus * randomBonus);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(startingNPC.transform.position, 5);
        foreach (NPC npc in possibleTo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(startingNPC.transform.position, npc.transform.position);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(npc.transform.position, 7);
        }
    }


    public override void OnSave(Save save)
    {
        CustomSave custom = new CustomSave();


        custom.leftNPCs = to.ConvertAll(x => x.GetUnqiueID());

        base.OnSave(save);

        GetThisSave(save).customData = JsonUtility.ToJson(custom);
    }

    public override void OnLoad(Save save)
    {
        CustomSave custom = JsonUtility.FromJson<CustomSave>(GetThisSave(save).customData);
        to = custom.leftNPCs.ConvertAll(x => {  GUIDManager.TryGetGUID(x,out var guid ); return guid.GetComponent<NPC>(); });

        base.OnLoad(save);
    }

    [System.Serializable]
    new class CustomSave
    {
        public List<string> leftNPCs = new List<string>();
    }
}
