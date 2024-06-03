using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NoticeBoard : SaveableBehaviour, I_Interactable
{

    public GameObject contractsParent;
    public int maxContracts;
    public TransporterLicences.LicenceType requiredLicence;

    public ObjectLabel label;

    private ContractInstanceBase[] allContracts;
    private ContractInstanceBase[] currentContracts;
    private List<ContractInstanceBase> contractQueue = new List<ContractInstanceBase>();

    override protected void Awake()
    {
        base.Awake();

        allContracts = contractsParent.GetComponentsInChildren<ContractInstanceBase>();
        currentContracts = new ContractInstanceBase[maxContracts];

        foreach (ContractInstanceBase contract in allContracts)
        {
            contract.OnSetUp += (ContractInstanceBase _) => { PlaceContractInQueue(contract); };
            contract.OnStarted += RemoveStartedContract;
        }
    }

    private void Start()
    {    
        Refs.contracts.OnContractCompleted += (ContractInstanceBase _)=> { Progress(); };

        Progress();
    }

    private void RemoveStartedContract(ContractInstanceBase contract)
    {
        for (int i = 0; i < maxContracts; i++)
        {
            if (currentContracts[i] == contract)
                currentContracts[i] = null;
        }

        for (int i = contractQueue.Count-1; i >= 0; i--)
        {
            if(contractQueue[i] == contract)
                contractQueue.RemoveAt(i);
        }

        label.SetIsMainQuest(currentContracts.Any(x => (x != null && x.IsMainQuest())));
    }

    private void PlaceContractInQueue(ContractInstanceBase contract)
    {
        if (contractQueue.Contains(contract))
            return;


        if (contract.IsMainQuest())
        {
            contractQueue.Insert(0, contract);
            Progress(); //forcing quest contracts to always be an option
        }
        else if (contract.isNew)
        {
            contractQueue.Insert(Mathf.FloorToInt(contractQueue.Count / 2), contract);
        }
        else
        { 
            contractQueue.Add(contract);
        }
    }
    public void Progress()
    { 
        int removedContract = Random.Range(0, maxContracts);

        if (currentContracts[removedContract] != null && !currentContracts[removedContract].IsMainQuest())
        {
            PlaceContractInQueue(currentContracts[removedContract]);
            currentContracts[removedContract] = null;
        }

        for (int i = 0; i < maxContracts; i++)
        {
            if (contractQueue.Count == 0)
                break;
            
            if (currentContracts[i] == null)
            {
                currentContracts[i] = contractQueue[0];
                contractQueue.RemoveAt(0);
            }
        }

        label.SetIsMainQuest(currentContracts.Any(x => (x!=null && x.IsMainQuest())));
    
    }

    private void OpenBoard()
    {
        if (requiredLicence > TransporterLicences.currentLicence)
        {
            Refs.ui.hud.SpawnGameMessege("I should get Tranporter Licence for this board");
            return;
        }

        if (IsEmpty())
            Progress();
        
        Refs.ui.OpenNoticeBoard(currentContracts);
    }

    private bool IsEmpty()
    {
        foreach (var contract in currentContracts)
        {
            if (contract != null)
                return false;
        }
        return true;
    }


    public InteractionMountingRequirement GetMountingRequirement() { return InteractionMountingRequirement.NotMountedAndMounted; }

    public bool CanBeInteracted() { return Refs.turtle.enabled; }

    public void Interact() { OpenBoard(); }

    public string GetInteractionText() { return "Look"; }

    public Transform GetInteractionTransform() { return transform; }


    [System.Serializable]
    public class CustomSave : Save.UniqueSave
    {
        public string[] currentContracts;
        public List<string> queue;
    }


    public override void OnSave(Save save)
    {
        CustomSave noticeboardSave = new CustomSave();
        noticeboardSave.id = GetUnqiueID();

        noticeboardSave.currentContracts = new string[currentContracts.Length];
        for (int i = 0; i < currentContracts.Length; i++)
        {
            noticeboardSave.currentContracts[i] = currentContracts[i]!=null ? currentContracts[i].GetUnqiueID() : "";
        }

        noticeboardSave.queue = contractQueue.ConvertAll(x => x.GetUnqiueID());

        save.noticeboards.Add(noticeboardSave);
    }

    public override void OnLoad(Save save)
    {
        CustomSave noticeboardSave = save.noticeboards.Find(x => x.id == GetUnqiueID());

        for (int i = 0; i < noticeboardSave.currentContracts.Length; i++)
        {
            if (GUIDManager.TryGetGUID(noticeboardSave.currentContracts[i], out var guid))
            {
                currentContracts[i] = guid.GetComponent<ContractInstanceBase>();
            }
        }

        for (int i = 0; i < noticeboardSave.queue.Count; i++)
        {
            if (GUIDManager.TryGetGUID(noticeboardSave.queue[i], out var guid))
            {
                contractQueue.Insert(i,guid.GetComponent<ContractInstanceBase>());
            }
        }
    }
    protected override int GetLoadingPriority()
    {
        return 700;
    }
}
