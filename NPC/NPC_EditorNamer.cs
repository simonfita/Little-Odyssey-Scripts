using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[ExecuteAlways]
public class NPC_EditorNamer : MonoBehaviour
{
    public NPC npc;
#if UNITY_EDITOR
    void Update()
    {
        if (EditorApplication.isPlaying)
            return;

        if (PrefabUtility.IsPartOfPrefabAsset(gameObject))
            return;
        if (npc == null)
            return;
        if(npc.schedule.schedule !=null)
            gameObject.name = npc.data.name + " (" + npc.schedule.schedule.name + ")";
        else
            gameObject.name = npc.data.name + " (NO SCHEDULE)";

        if (npc.locations.home != null)
            npc.locations.home.gameObject.name = npc.locations.home.locationName + " (" + npc.data.name + ")";
        if (npc.locations.ownStand != null)
            npc.locations.ownStand.gameObject.name = npc.locations.ownStand.locationName + " (" + npc.data.name + ")";
    }
#endif
}
