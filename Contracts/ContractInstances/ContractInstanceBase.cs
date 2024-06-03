using Pixeye.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ContractState
{ 
    Ready,
    NPCWaiting,
    InProgress,
    Finished

}

public abstract class ContractInstanceBase : SaveableBehaviour
{

    public ContractState state = ContractState.Finished;


    [SerializeField]
    protected NPC startingNPC;

    public float rewardMultiplier = 1;
    protected float randomBonus =1;
    protected float firstTimeBonus { get { return isNew ? 1.5f : 1f; } }
    
    public ContractInstanceBase requiredContract;
    public bool unique;
    
    public TransporterLicences.LicenceType requiredLicence;
    public MainQuest.MainQuestStage requiredMainQuest = MainQuest.MainQuestStage.Null;

    public System.Action<ContractInstanceBase> OnStarted, OnTalked, OnFinished, OnSetUp;

    public int completedAmount { get; private set; } = 0;
    public bool isNew { get { return completedAmount == 0; } }

    public ContractColor flagType { get; private set; }

    private void Update()
    {
        if (state == ContractState.Finished)
        {
            if (TryCleanupAndSetup())
            {
                state = ContractState.Ready;
                OnSetUp?.Invoke(this);
            }
        }
    }

    public virtual void OnStart(ContractColor _flagType)
    {
        flagType = _flagType;
        if (startingNPC != null)
        {
            state = ContractState.NPCWaiting;
            startingNPC.involvedInContracts.Add(this);
            startingNPC.speech.OnTalkedAboutContract += OnStartingNPCTalkedAboutSomeContract;
        }
        else
            OnStartingNPCTalked();
        OnStarted?.Invoke(this);
    }

    private void OnStartingNPCTalkedAboutSomeContract(ContractInstanceBase talkedAbout)
    {
        if (talkedAbout == this)
        {
            OnStartingNPCTalked();
            startingNPC.speech.OnTalkedAboutContract -= OnStartingNPCTalkedAboutSomeContract;
        }
    }

    protected virtual void OnStartingNPCTalked()
    {
        state = ContractState.InProgress;
        if (startingNPC != null)
        {
            startingNPC?.involvedInContracts.Remove(this);
        }

        OnTalked?.Invoke(this);
        Refs.contracts.OnContractsUpdated.Invoke();
    }

    public virtual void OnFinish()
    {
        state = ContractState.Finished;
        completedAmount++;
        OnFinished?.Invoke(this);
    }

    public virtual bool TryCleanupAndSetup()
    {
        if (requiredContract != null && requiredContract.completedAmount == 0)
            return false;

        if (unique && completedAmount > 0)
            return false;

        if (!TransporterLicences.HasThisOrHigher(requiredLicence))
            return false;

        if (Refs.mainQuest.currentStage < requiredMainQuest)
            return false;

        randomBonus = Random.Range(1, 1.4f);

        return true;
    }

    public virtual string GetDescription()
    {
        if(state == ContractState.NPCWaiting && startingNPC !=null)
            return "Talk with "+startingNPC.data.name+" for details";
        else
        {
            Debug.LogError("Should not get description from here now!");
            return "Description missing!";
        }
    }

    public bool IsMainQuest()
    {
        return completedAmount ==0 && Refs.mainQuest.StageDescriptions[Refs.mainQuest.currentStage].contractsToComplete.Contains(this);
    }


    public virtual int GetReward()
    {
        return 69;
    }

    public virtual string GetDialogueStartingKnot(NPC npc)
    {
        if (npc == startingNPC)
        {
            if (isNew)
                return gameObject.name;
            else
                return gameObject.name + "Again";
        }
        else
        {
            Debug.LogError("Cheking dialogue for wrong NPC");
            return null;
        }
    }


    [System.Serializable]
    public class CustomSave : Save.UniqueSave
    {
        public int contractState;
        public int contractFlag;
        public float randomBonus;
        public int completedAmount;
        public string customData;
    }

    public override void OnSave(Save save)
    {
        CustomSave contractSave = new CustomSave();
        contractSave.id = GetUnqiueID();
        
        contractSave.completedAmount = completedAmount;
        contractSave.contractState = (int)state;
        contractSave.contractFlag = (int)flagType;
        contractSave.randomBonus = randomBonus; 
        save.contracts.Add(contractSave);
    }

    protected CustomSave GetThisSave(Save save)
    {
        return save.contracts.Find(x => x.id == GetUnqiueID());
    }

    public override void OnLoad(Save save)
    {
        CustomSave contractSave = GetThisSave(save);

        completedAmount = contractSave.completedAmount;

        ContractState loadedState = (ContractState)contractSave.contractState;
        contractSave.randomBonus = randomBonus;

        if (loadedState == ContractState.NPCWaiting)
        {
            Refs.contracts.AddContract(this, (ContractColor)contractSave.contractFlag);
        }
        else if (loadedState == ContractState.InProgress)
        {
            Refs.contracts.AddContract(this, (ContractColor)contractSave.contractFlag);
            OnStartingNPCTalkedAboutSomeContract(this);
        }
        else
        {
            state = (ContractState)contractSave.contractState;  
        }

    }

    protected override int GetLoadingPriority()
    {
        return 100;
    }

}
