using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RotateObjectToTerrain 
{
    [MenuItem("Tools/Rotate to Terrain #G")]
    public static void RotateTowardTerrain()
    {
        foreach (GameObject selected in Selection.gameObjects)
        {
            Undo.RecordObject(selected.transform, "Rotated object towards terrain");
            Ray ray = new Ray(selected.transform.position+Vector3.up*10, Vector3.down);

            RaycastHit[] hits = Physics.RaycastAll(ray, 100);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.tag == "Terrain")
                {
                   
                    selected.transform.up = hit.normal;
                    selected.transform.position = hit.point;

                }

            }
        }
    }
}
