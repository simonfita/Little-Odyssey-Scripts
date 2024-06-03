using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class GUID : MonoBehaviour
{
    public string guid = "";

    private void Awake()
    {
        if (!GUIDManager.CheckForGUID(this, out string newGUID))
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Genereted GUID");
#endif
            guid = newGUID;

        }
    }

    private void OnDestroy()
    {
        GUIDManager.RemoveGUID(this);
    }
}


public static class GUIDManager
{
    private static Dictionary<string, GUID> guids = new Dictionary<string, GUID>();

    public static bool CheckForGUID(GUID currentGUID, out string newGUID)
    {
        newGUID = "error";

        if (currentGUID.guid == "")
        {
            newGUID = GenerateGUID();
            guids.Add(newGUID, currentGUID);
            return false;
        }

        if (guids.ContainsKey(currentGUID.guid))
        {
            if (guids[currentGUID.guid] == currentGUID)
            {
                return true;
            }
            else
            {
                newGUID = GenerateGUID();
                guids.Add(newGUID, currentGUID);
                return false;
            }
        }
        else
        {

            guids.Add(currentGUID.guid, currentGUID);
            return true;
        }


    }

    private static string GenerateGUID()
    {
        string newGUID;
        do
        {
            newGUID = System.Guid.NewGuid().ToString();
        } while (guids.ContainsKey(newGUID));

        return newGUID;
    }

    public static void RemoveGUID(GUID toRemove)
    { 
        guids.Remove(toRemove.guid);
    }

    public static bool TryGetGUID(string guid, out GUID outGUID)
    {
        outGUID = null;

        if (guids.ContainsKey(guid))
        {
            outGUID = guids[guid];
            return true;
        }
        return false;
    }

}
