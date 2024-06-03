using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class WorldInspector : EditorWindow
{
    private int selectedToolbar = 0;

    private List<LocationBase> locations = new List<LocationBase>();

    private List<NPC> npcs = new List<NPC>();

    private Vector2 locScrollPos, npcScrollPos;

    [MenuItem("Window/World Inspector")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(WorldInspector));
    }

    void OnGUI()
    {
        selectedToolbar = GUILayout.Toolbar(selectedToolbar, new string[] { "Locations", "NPC" });

        if (selectedToolbar == 0)
        {
            DrawLocations();
        }
        else if (selectedToolbar == 1)
        {
            DrawNPCs();
        }


    }

    private void DrawLocations()
    {
        GUILayout.Space(30);

        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Refresh"))
            RefreshLocations();
        if (GUILayout.Button("Regenerate"))
            RegenerateLocations();

        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        locScrollPos = GUILayout.BeginScrollView(locScrollPos);

        foreach (LocationBase loc in locations)
        {
            if (loc.GetType() != typeof(LocationSettlement))
                continue;

            bool clicked = GUILayout.Button(loc.locationName);
            if (clicked)
            {
                Selection.activeObject = loc;
                EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
            }
        
        }

        GUILayout.EndScrollView();

    }

    private void DrawNPCs()
    {
        GUILayout.Space(30);
        
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Refresh"))
            RefreshNPCs();

        GUILayout.EndHorizontal();

        GUILayout.Space(15);
        npcScrollPos = GUILayout.BeginScrollView(npcScrollPos);

        foreach (NPC npc in npcs)
        {
            bool clicked = GUILayout.Button(npc.gameObject.name);
            if (clicked)
            {
                Selection.activeObject = npc;
                EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
            }

        }

        GUILayout.EndScrollView();


    }

    private void RefreshLocations()
    {
        locations = GameObject.FindObjectsOfType<LocationBase>().ToList();

        locations.Sort((l1,l2)=>l1.locationName.CompareTo(l2.locationName));
    }

    private void RefreshNPCs()
    {
        npcs = GameObject.FindObjectsOfType<NPC>().ToList();

        npcs.Sort((n1, n2) => n1.data.name.CompareTo(n2.data.name));


    }

    private void RegenerateLocations()
    {

        locations = GameObject.FindObjectsOfType<LocationBase>().ToList();

        foreach (LocationBase loc in locations)
        {
            if (loc.transform.parent == null)
                continue;


            loc.parentLocation = loc.transform.parent.GetComponentInParent<LocationBase>();
            EditorUtility.SetDirty(loc);

        }

        RefreshLocations();
    }

}
