using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Ink.Runtime;
using System;

public class NPC_Speech : MonoBehaviour
{
    [SerializeField]
    private string startingDialogueKnot;

    private ContractInstanceBase lastContractTalkedAbout;

    public System.Action<string> OnNextLine;
    public System.Action<List<Choice>> OnChoices;
    public System.Action OnDialogueEnd;
    public System.Action<ContractInstanceBase> OnTalkedAboutContract;

    public int SelectedChoice;

    public TextAsset dialogueAsset;

    private Story story;

    private NPC self;

    private void Awake()
    {
        self = GetComponent<NPC>();

        story = new Story(dialogueAsset.text);

        story.onError += StoryError;

        BindFunction();
    }

    private void StoryError(string message, Ink.ErrorType type)
    {
        Debug.Log(message + "   " + type);
        EndDialogue();
        StopCoroutine(RunDialogue());
    }

    public string SaveText()
    {
        return story.state.ToJson();
    }
    public void LoadText(string text)
    {
        story.state.LoadJson(text);
    }

    public void StartedDialogue()
    {
        Refs.ui.StartDialogue(self);
        Refs.gamepadController.RequestPointingMode("Speech");

        FillVariables();
        
        StartCoroutine(RunDialogue());
    }

    IEnumerator RunDialogue()
    {
        story.ChoosePathString("main");
        while (true)
        {
            while (story.canContinue)
            {
                OnNextLine.Invoke(story.Continue());
            }
            if (story.currentChoices.Count > 0)
            {
                OnChoices.Invoke(story.currentChoices);

                while (SelectedChoice == -1)
                {
                    yield return new WaitForEndOfFrame();
                }
                story.ChooseChoiceIndex(SelectedChoice);
                SelectedChoice = -1;

            }
            else
                break;
        }
        EndDialogue();

        yield break;

    }
    
    private void EndDialogue()
    {
        self.EndDialogue();

        Refs.ui.EndDialogue();
        Refs.player.EndDialogue();
        Refs.gamepadController.UnrequestPointingMode("Speech");

        OnDialogueEnd?.Invoke();
        
        if (lastContractTalkedAbout != null)
        {
            OnTalkedAboutContract.Invoke(lastContractTalkedAbout);
            lastContractTalkedAbout = null;
        }

    }

    private void FillVariables()
    {
        string startingKnot = startingDialogueKnot;

        //sets the last one
        foreach (ContractInstanceBase contract in self.involvedInContracts)
        {
            string contractStart = contract.GetDialogueStartingKnot(self);
            if (contractStart != null)
            {
                startingKnot = contractStart;
                lastContractTalkedAbout = contract;
                break;

            }
        }
        

        
        if(Refs.mainQuest.currentStage == MainQuest.MainQuestStage.D7) //feast override
        {
            story.variablesState["startingKnot"] = new Path("feastStart");
        }
        else if (story.KnotContainerWithName(startingKnot) != null)
        {
            story.variablesState["startingKnot"] = new Path(startingKnot);
        }
        else
        {
            story.variablesState["startingKnot"] = new Path("start");
            Debug.LogError("Tried to start dialogue at not existing knot: " + startingKnot);
        }





        story.variablesState["npcName"] = self.data.name;
        story.variablesState["npcCity"] = self.locations.home.parentLocation.locationName;

        story.variablesState["isForwarder"] = self.schedule.schedule.name == "Forwarder";

        //story.variablesState["canTrade"] = self.schedule.schedule.name == "Merchant";
        self.trade.FillVariables(story);

  
        story.variablesState["playerMoney"] = Refs.inventory.playerMoney.amount;

        story.variablesState["mainQuestStage"] =  Refs.mainQuest.currentStage.ToString();
        story.variablesState["currentLicence"] = (int)TransporterLicences.currentLicence;

        story.variablesState["isWesternBridgeRepaired"] = Refs.mainQuest.currentStage >= MainQuest.MainQuestStage.B1;



    }

    private void BindFunction()
    {
        story.BindExternalFunction("buyItem", (string id) => { self.trade.BuyItem(id); });
        story.BindExternalFunction("canBuyItem", (string id) => { return self.trade.CanBuyItem(id); }, true);
        story.BindExternalFunction("getItemName", (string id) => { return self.trade.GetItemName(id); },true);
        story.BindExternalFunction("getItemPrice", (string id) => { return self.trade.GetItemPrice(id); },true);

        story.BindExternalFunction("hasItem", (string id) => { return Refs.inventory.HasItem(id); }, true);
        story.BindExternalFunction("giveItem", (string id) => { Refs.inventory.AddItem(id); });
        story.BindExternalFunction("changeMoney", (int amount) => { Refs.inventory.playerMoney.amount+=amount; });

        story.BindExternalFunction("setMainQuest", (string stage) => { SetMainQuest(stage); });

        story.BindExternalFunction("startSandstorm", () => { Refs.mainQuest.StartSandstorms(); });
        story.BindExternalFunction("resolveContest", () => { return Refs.mainQuest.contestManager.ResolveContestWinner(); });
    }

    private void SetMainQuest(string stage)
    {
        if (Enum.TryParse(stage, out MainQuest.MainQuestStage eStage))
            Refs.mainQuest.TrySetStage(eStage);
        else
            Debug.LogError("Can't set stage - " + stage);
    }



    #region Called From Ink

    

    #endregion


}
