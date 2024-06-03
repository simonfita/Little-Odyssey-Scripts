using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine.Playables;
using Steamworks;

public class MainQuest : SaveableBehaviour
{
    [Serializable]
    public struct MainQuestStageDescriptor
    {
        public string name;
        public NPC involvedNPC;
        public List<ContractInstanceBase> contractsToComplete;

    }

    public enum MainQuestStage
    {
        Null,
        A1, A2, A3, A4, A5, A6,
        B1, B2, B3, B4, B5, B6, B7, B8, B9, B10,
        C1, C2, C3, C4, C5, C6, C7, C8, C9, C10,
        D1, D2, D3, D4, D5, D6, D7, D8,

    }

    [SerializedDictionary("Stage", "Options")]
    public SerializedDictionary<MainQuestStage, MainQuestStageDescriptor> StageDescriptions;

    private static Action<MainQuestStage> OnMainQuestStageSet;

    public MainQuestStage currentStage { get; private set; }
    private TMPro.TMP_Text questText;

    public GameObject repairedBridge, brokenBridge;
    
    public ContractInstanceBase sandstormStarter;
    
    public CargoCrane[] contestCranes;

    public ContestManager contestManager;
    public FeastManager feastManager;

    private NPC currentInvolved;

    public AudioSource src;

    private void Start()
    {
        OnMainQuestStageSet = null;

        //TODO better
        questText = GameObject.Find("Quest Text").GetComponent<TMPro.TMP_Text>();

        OnMainQuestStageSet += (MainQuestStage stage) => { if (stage >= MainQuestStage.B1) {repairedBridge.SetActive(true); brokenBridge.SetActive(false); }};
        OnMainQuestStageSet += (MainQuestStage stage) => { if (stage >= MainQuestStage.B5) { Refs.weatherSystem.EnableSandstorms(); } }; //cheats fallback
        OnMainQuestStageSet += (MainQuestStage stage) => { if (stage == MainQuestStage.C9) CutsceneManager.PlayCutscene("ContestStart"); };
        OnMainQuestStageSet += (MainQuestStage stage) => { feastManager.UpdateStage(stage); src.Play(); FindObjectOfType<HUD_MainQuestSummary>().Show(); };
        OnMainQuestStageSet += CheckAchievements;
        Refs.contracts.OnContractsUpdated += UpdateDescription;

        TrySetStage(MainQuestStage.A1);
        BindConditions();

    }



    private void BindConditions()
    {
        foreach (var desc in StageDescriptions)
        {
            foreach (ContractInstanceBase contract in desc.Value.contractsToComplete)
            {
                Action<ContractInstanceBase> lambda = null;
                lambda = (ContractInstanceBase _) =>
                {
                    foreach (ContractInstanceBase contract in desc.Value.contractsToComplete)
                        if (contract.completedAmount == 0)
                            return;
                    TrySetStage(desc.Key + 1);
                    foreach (ContractInstanceBase contract in desc.Value.contractsToComplete)
                        contract.OnFinished -= lambda;
                };

                contract.OnFinished += lambda;
            }
        }
    }

    public void GotTransporterLicence(TransporterLicences.LicenceType type)
    {
        switch (type)
        {
            case TransporterLicences.LicenceType.ShortDistance:
                TrySetStage(MainQuestStage.A5);
                return;
            case TransporterLicences.LicenceType.MediumDistance:
                TrySetStage(MainQuestStage.B2);
                return;
            case TransporterLicences.LicenceType.ExtendedMediumDistance:
                TrySetStage(MainQuestStage.B6);
                return;
            case TransporterLicences.LicenceType.Sailing:
                TrySetStage(MainQuestStage.B9);
                return;
            case TransporterLicences.LicenceType.LongDistance:
                TrySetStage(MainQuestStage.C2);
                return;
            case TransporterLicences.LicenceType.Royal:
                TrySetStage(MainQuestStage.D2);
                return;
        }
    }

    public void TrySetStage(MainQuestStage i, bool force = false)
    {
        if (i <= currentStage && !force)
            return; // can't go back to previous stages

        currentStage = i;
        OnMainQuestStageSet.Invoke(i);
        UpdateDescription();
        Invoke(nameof(SetNPCLabels), 0);
    }

    private void CheckAchievements(MainQuestStage stage)
    {
        if (!SteamManager.Initialized)
            return;

        switch (stage)
        {
            case MainQuestStage.B1:
                SteamUserStats.SetAchievement("LONGSHORE");
                SteamUserStats.StoreStats();
                break;
            case MainQuestStage.C1:
                SteamUserStats.SetAchievement("STAR_ROCK");
                SteamUserStats.StoreStats();
                break;
            case MainQuestStage.D1:
                SteamUserStats.SetAchievement("GRAIN_PLAINS");
                SteamUserStats.StoreStats();
                break;
            case MainQuestStage.D8:
                SteamUserStats.SetAchievement("END");
                SteamUserStats.StoreStats();
                break;
        
        }
        
    }

    public void UpdateDescription()
    {
        questText.text = GetDescription();
    }

    public string GetDescription()
    {        
        return String.Format(StageDescriptions[currentStage].name, GetLeftContracts(),GetEqupimentToBuy());
    }

    private string GetLeftContracts()
    {
        int maxContracts = 0;
        int completedContracts = 0;

        foreach (ContractInstanceBase con in StageDescriptions[currentStage].contractsToComplete)
        {
            maxContracts++;

            if(con.completedAmount>0)
                completedContracts++;
        }

        return String.Format("({0}/{1})", completedContracts, maxContracts);
    }

    private void SetNPCLabels()
    {
        if(currentInvolved!=null)
            currentInvolved.isInvolvedInMainQuest = false;

        currentInvolved = StageDescriptions[currentStage].involvedNPC;

        if (currentInvolved != null)
            currentInvolved.isInvolvedInMainQuest = true;

    }

    private string GetEqupimentToBuy()
    {
        if (Refs.inventory.HasItem("HydrationTank"))
        {
            if (Refs.inventory.HasItem("RightPack"))
                return "Talk to Forwarder Chibale";
            return "Buy Additional Right Pack in Salrise";
        }
        else
        {
            if (Refs.inventory.HasItem("RightPack"))
                return "Buy Hydration Tank in Salrise";
            return "Buy Additional Right Pack and Hydration Tank in Salrise";
        }
    }

    public void StartSandstorms()
    {
        Refs.weatherSystem.EnableSandstorms();
        CutsceneManager.PlayCutscene("Sandstorm");
    }





    public override void OnSave(Save save)
    {
        save.mainQuestStage = (int)currentStage;
    }

    public override void OnLoad(Save save)
    {
        TrySetStage((MainQuestStage)save.mainQuestStage,true);
    }
}
