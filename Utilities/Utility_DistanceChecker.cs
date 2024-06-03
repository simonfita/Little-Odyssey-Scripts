using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using System;

[ExecuteAlways]
public class Utility_DistanceChecker : MonoBehaviour
{

#if UNITY_EDITOR
    public Transform child;

    Ray ray;

    private bool isChildTurn = false;

    bool unTapped = true;

    private void Update()
    {
        
        

        if (!Keyboard.current.f10Key.isPressed)
        {
            unTapped = true;
            return;
        }
        if (unTapped)
        {
            isChildTurn = !isChildTurn;
            unTapped = false;
        }

        

        Vector2 mousePos = Pointer.current.position.ReadValue() - new Vector2(1480, 0);
        Debug.Log(mousePos);



        ray = HandleUtility.GUIPointToWorldRay(mousePos);


        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit))
            return;



        if (isChildTurn)
            child.position = hit.point+Vector3.up*5;
        else
        {
            Vector3 lastPos = child.position;

            transform.position = hit.point + Vector3.up * 5;

            child.position = lastPos;
        }


      
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(ray);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position,child.position);
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position, transform.up * -20);
        Gizmos.DrawRay(child.position, transform.up * -20);

        float distance = Vector3.Distance(transform.position, child.position);

        TimeSpan time = TimeSpan.FromSeconds(distance/4);

        //here backslash is must to tell that colon is
        //not the part of format, it just a character that we want in output
        string str = "  " + time.ToString(@"m\:ss")+ "m";


        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;

        style.normal.textColor = Color.blue;
        Handles.Label((transform.position+child.position)/2, "Distance: " + (int)distance + str,style);
    }

#endif
}
