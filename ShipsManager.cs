using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AYellowpaper.SerializedCollections;

public class ShipsManager : MonoBehaviour
{
    public enum SailingShipType
    { 
        SalriseToGreeting,
        SalriseToEmperors,
        Raft,
    }

    public SerializedDictionary<SailingShipType, SailingShip> ships;

    public Animator anim;

    public float waitTime;

    public void OnReachedPort(SailingShipType ship)
    {
        SetShipAnimSpeed(ship, 0);
        StartCoroutine(SailOffWhenReady(ship));
    }

    private IEnumerator SailOffWhenReady(SailingShipType ship)
    { 
        yield return new WaitForSeconds(waitTime);
        while (ship == SailingShipType.SalriseToEmperors && Refs.mainQuest.currentStage < MainQuest.MainQuestStage.C1)
        {
            yield return null;
        }

        while (ships[ship].waitingTrigger.playerIn && !ships[ship].isPlayerSailing)
        {
            yield return null;
        }
        SetShipAnimSpeed(ship, 1);
        yield break;
    }

    public void SetShipTime(SailingShipType ship, float normalizedTime)
    {
        switch (ship)
        {
            case SailingShipType.SalriseToGreeting:
                anim.Play("SalriseToGreeting",0,normalizedTime);
                break;
            case SailingShipType.SalriseToEmperors:
                anim.Play("SalriseToEmperors", 1,normalizedTime);
                break;
            case SailingShipType.Raft:
                anim.Play("Raft", 2,normalizedTime);
                break;
        }
        SetShipAnimSpeed(ship, 1);
    }


    public void ForceExit(SailingShipType shipType)
    {
        if (ships[shipType].isPlayerSailing)
            ships[shipType].TryExit();
        SetShipAnimSpeed(shipType, 0); //raft hack
    }

    private void SetShipAnimSpeed(SailingShipType ship, float speed)
    {
        switch (ship)
        {
            case SailingShipType.SalriseToGreeting:
                anim.SetFloat("SalriseToGreetingSpeed", speed);
                break;
            case SailingShipType.SalriseToEmperors:
                anim.SetFloat("SalriseToEmperorsSpeed", speed);
                break;
            case SailingShipType.Raft:
                anim.SetFloat("RaftSpeed", speed);
                break;
        }
    }



}
