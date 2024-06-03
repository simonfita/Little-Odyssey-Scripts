using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskStates
{
    NotStarted,
    Starting,
    Doing,
    Ending,
    Completed
}


public class TaskBase : MonoBehaviour
{
    
    public TaskStates state { get; private set; } = TaskStates.NotStarted;
    
    public float startTime;

    [SerializeField,HideInInspector]
    protected NPC npc;

    public bool wantsToEnd = false;



    public void Initialize(NPC _npc, float _startTime)
    {
        npc = _npc;
        startTime = _startTime;
        enabled = false;
    }

    protected virtual void OnEnable()
    {
        if (npc == null)
            return;
    }

    protected virtual void Update()
    { 
    
    }

    protected virtual void OnDisable() 
    {
        wantsToEnd = false;
    }

    public virtual bool CanEndTask()
    {
        return true;
    }
}
