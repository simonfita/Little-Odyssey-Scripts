using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ContestManager : SaveableBehaviour
{
    public enum ContestStage
    { 
        NotSpawned,
        Spawned,
        DuringDelivery,
        AllDelivered,
        Finished
    }
    [System.Serializable]
    public class ContestState
    {
        public ContestStage contestStage = ContestStage.NotSpawned;
        public float primusScale, tritusScale, eugeneScale;
        public bool primusDelivered, tritusDelivered, eugeneDelivered;
        public int contestWinner;
    }

    private ContestState contestState;
    
    public ContestVegetable primusVeggie, tritusVeggie, eugeneVeggie;
    
    
    public TransportHeavyCargoInstance primusVeggieContract, tritusVeggieContract, eugeneVeggieContract;

    

    protected override void Awake()
    {
        base.Awake();
        contestState = new ContestState();
        primusVeggie.gameObject.SetActive(false);
        tritusVeggie.gameObject.SetActive(false);
        eugeneVeggie.gameObject.SetActive(false);
        primusVeggieContract.OnFinished += (ContractInstanceBase _) => { contestState.primusDelivered = true; };
        tritusVeggieContract.OnFinished += (ContractInstanceBase _) => { contestState.tritusDelivered = true; };
        eugeneVeggieContract.OnFinished += (ContractInstanceBase _) => { contestState.eugeneDelivered = true; };

    }

    private void Update()
    {
        if (contestState.contestStage == ContestStage.NotSpawned && Refs.mainQuest.currentStage >= MainQuest.MainQuestStage.C1)
        {
            primusVeggie.gameObject.SetActive(true);
            tritusVeggie.gameObject.SetActive(true);
            eugeneVeggie.gameObject.SetActive(true);
            contestState.contestStage = ContestStage.Spawned;
        }

        if (contestState.contestStage == ContestStage.Spawned)
        {
            contestState.primusScale = primusVeggie.transform.localScale.x;
            contestState.tritusScale = tritusVeggie.transform.localScale.x;
            contestState.eugeneScale = eugeneVeggie.transform.localScale.x;
            if (primusVeggieContract.state == ContractState.Ready && tritusVeggieContract.state == ContractState.Ready && eugeneVeggieContract.state == ContractState.Ready)
            {
                primusVeggieContract.contractCargo.transform.localScale *= contestState.primusScale;
                tritusVeggieContract.contractCargo.transform.localScale *= contestState.tritusScale;
                eugeneVeggieContract.contractCargo.transform.localScale *= contestState.eugeneScale;
                primusVeggie.gameObject.SetActive(false);
                tritusVeggie.gameObject.SetActive(false);
                eugeneVeggie.gameObject.SetActive(false);
                contestState.contestStage = ContestStage.DuringDelivery;
            }
        }

        if (contestState.contestStage == ContestStage.DuringDelivery)
        {
            if (!primusVeggieContract.isNew && !tritusVeggieContract.isNew && !eugeneVeggieContract.isNew)
            {
                contestState.contestStage = ContestStage.AllDelivered;
            }
        }


    }


    public int ResolveContestWinner()
    {
        if (contestState.contestStage < ContestStage.AllDelivered)
            throw new System.Exception();
        if (contestState.contestStage == ContestStage.Finished)
            return contestState.contestWinner;

        contestState.contestStage = ContestStage.Finished;
       
        
        List<float> sizes = new List<float>() { contestState.primusScale, contestState.eugeneScale, contestState.tritusScale };
        int biggest = sizes.IndexOf(sizes.Max());

        CutsceneManager.PlayCutscene("ContestWinner" + biggest);
        
        return biggest;
    }


    public override void OnSave(Save save)
    {
        //fallback if have not started
        contestState.primusScale = primusVeggie.transform.localScale.x;
        contestState.tritusScale = tritusVeggie.transform.localScale.x;
        contestState.eugeneScale = eugeneVeggie.transform.localScale.x;

        save.contestSave = contestState;
    }

    public override void OnLoad(Save save)
    {
        contestState = save.contestSave;
        
        primusVeggie.gameObject.SetActive(contestState.contestStage == ContestStage.Spawned);
        tritusVeggie.gameObject.SetActive(contestState.contestStage == ContestStage.Spawned);
        eugeneVeggie.gameObject.SetActive(contestState.contestStage == ContestStage.Spawned);

        primusVeggie.transform.localScale *= contestState.primusScale;
        tritusVeggie.transform.localScale *= contestState.tritusScale;
        eugeneVeggie.transform.localScale *= contestState.eugeneScale;

        if (contestState.primusDelivered)
            primusVeggieContract.toCrane.CreateCargo(primusVeggieContract.cargoPrefab).transform.localScale *= contestState.primusScale;
        if (contestState.tritusDelivered)
            tritusVeggieContract.toCrane.CreateCargo(tritusVeggieContract.cargoPrefab).transform.localScale *= contestState.tritusScale;
        if (contestState.eugeneDelivered)
            eugeneVeggieContract.toCrane.CreateCargo(eugeneVeggieContract.cargoPrefab).transform.localScale *= contestState.eugeneScale;



        if (primusVeggieContract.contractCargo !=null)
            primusVeggieContract.contractCargo.transform.localScale *= contestState.primusScale;
        if (tritusVeggieContract.contractCargo != null)
            tritusVeggieContract.contractCargo.transform.localScale *= contestState.tritusScale;
        if (eugeneVeggieContract.contractCargo != null)
            eugeneVeggieContract.contractCargo.transform.localScale *= contestState.eugeneScale;

    }


}
