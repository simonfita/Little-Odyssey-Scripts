using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using TMPro;
using System;
using SF;
[System.Serializable]
public struct NPCData
{ 
    public string name;
    public List<AudioClip> talkSounds;
}
[System.Serializable]
public struct NPCLocations
{
    public LocationHome home;
    public LocationStand ownStand;
    public LocationMeetingArea meetingArea;

}

public class NPC : SaveableBehaviour
{
    //Components
    public Token token;
    public NPC_Movement movement;
    public NPC_Speech speech;
    public NPC_Trade trade;
    public NPC_Schedule schedule;

    public AudioSource TalkingSrc;
    public TriggerCollider GreetingTrigger;
    public ObjectLabel label;

    public NPCData data;
    public NPCLocations locations;

    public bool isInvolvedInMainQuest;
    public List<ContractInstanceBase> involvedInContracts = new List<ContractInstanceBase>();

    public bool isInDialogue { get; private set; }

    public UI_Quip quipPrefab;
    public bool asleep;
    public AudioClip sleepSound;
    private List<Token> greetedToday = new List<Token>();

    override protected void Awake()
    {
        base.Awake();

        name = data.name;

        movement.Teleport(locations.home.GetPosition());

        GreetingTrigger.OnTriggerEntered += Greet;
        WorldTime.OnNextDay += () => { greetedToday = new List<Token>(); };
        speech.OnNextLine += (string s) => { StartCoroutine(token.rend.SpeechAnimation(s.Length / 30f)); MakeSound();};

       
    }

    private void Update()
    {
       
        if(!isInDialogue)
        {
            schedule.FollowSchedule();
        }

        UpdateLabel();
    }

    private void UpdateLabel()
    {   
        label.SetIsMainQuest(isInvolvedInMainQuest);
        label.SetContracts(new List<ContractColor>(involvedInContracts.Select(x => x.flagType)));
    }

    private void Greet(Collider other)
    {     
        Token _token = other.gameObject.GetComponentInParent<Token>();
        if (_token != null && !greetedToday.Contains(_token))
        {
            greetedToday.Add(_token);
            token.quips.Greet();
            return;
        }
    }

    public void MakeSound()
    {
        TalkingSrc.clip = data.talkSounds[UnityEngine.Random.Range(0, data.talkSounds.Count)];
        TalkingSrc.Play();
    }

    public void MakeSleepingSound()
    {
        TalkingSrc.clip = sleepSound;
        TalkingSrc.Play();
    }

    public void StartDialogue()
    {

        isInDialogue = true;

        movement.token.canMove = false;
        transform.forward = Refs.player.transform.position- transform.position;
        //movement.token.StopMovementAnimation();

        Refs.player.StartDialogue(this);
        speech.StartedDialogue();
    }

    public void EndDialogue()
    {
        isInDialogue = false;
        movement.token.canMove = true;       
    }


    public static NPC FindNPC(string name)
    {
        foreach (NPC n in FindObjectsOfType<NPC>())
        {
            if (n.data.name == name)
                return n;
        }
        Debug.LogError("Can't find NPC '"+name+"'");
        return null;
    }

    [System.Serializable]
    public class CustomSave : Save.UniqueSave
    {
        public Vector3 position;
        public string dialogue;
        public List<string> offers;
    }

    public override void OnSave(Save save)
    {
        CustomSave npcSave = new CustomSave();
        npcSave.id = GetUnqiueID();
        npcSave.position = transform.position;
        npcSave.dialogue = speech.SaveText();

        npcSave.offers = trade.offerts.Select(x => x.item.name).ToList();

        save.npcs.Add(npcSave);
    }

    public override void OnLoad(Save save)
    {
        CustomSave npcSave = (CustomSave)save.npcs.Find(x => x.id == GetUnqiueID());

        if (!token.inSlot)
        {
            movement.Teleport(npcSave.position);
        }
        speech.LoadText(npcSave.dialogue);

        trade.offerts.ReverseForEach((NPC_Trade.ItemOffer offer, int index ) => {
            if (!npcSave.offers.Contains(offer.item.name))
                trade.offerts.RemoveAt(index);
        });
  
    }
}
