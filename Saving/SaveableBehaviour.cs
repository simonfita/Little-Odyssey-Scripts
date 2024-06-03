using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GUID))]
public abstract class SaveableBehaviour : MonoBehaviour
{
    public static List<SaveableBehaviour> all = new List<SaveableBehaviour>();

    private GUID guid;

    public abstract void OnSave(Save save);

    public abstract void OnLoad(Save save);

    protected virtual void Awake()
    {
        all.Add(this);
        guid = GetComponent<GUID>();
    }

    protected virtual void OnDestroy()
    {
        all.Remove(this);
    }

    protected virtual int GetLoadingPriority()
    {
        return 500;
    }

    public string GetUnqiueID()
    {
        return guid.guid;
    }

    public static int CompareByPriority(SaveableBehaviour a, SaveableBehaviour b)
    { 
        if(a.GetLoadingPriority() == b.GetLoadingPriority())
                return 0;
        if (a.GetLoadingPriority() > b.GetLoadingPriority())
            return 1;
        else
            return -1;

    }
}
