using UnityEngine;
using UnityEditor;
public class FindMissingMeshes : EditorWindow
{

    [MenuItem("Window/FindMissingMeshes")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FindMissingMeshes));
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Find Missing Meshes"))
        {
            FindMissing();
        }
    }

    private static void FindMissing()
    {
        MeshFilter[] amf = GameObject.FindObjectsOfType<MeshFilter>();
        foreach (MeshFilter mf in amf)
        {


            if (mf.sharedMesh==null)
                Debug.Log("Missing mesh on game object: " + mf.gameObject.name);
        }

    }
}