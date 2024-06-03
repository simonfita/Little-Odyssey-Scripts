using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DudSystem : MonoBehaviour
{
    public List<GameObject> dudTemplates;

    private IDudResponder currentResponder;
    private RaycastHit hitData;

    public bool tpMode; //cheats

    private void Update()
    {

        if (!Refs.controls.Other.Dud.WasPerformedThisFrame())
            return;

        if (hitData.collider == null)
            return;

        if (tpMode)
        {
            Refs.turtle.Mount();
            Refs.turtle.Teleport(hitData.point+Vector3.up*3);
            return;
        }

        if (currentResponder != null)
        {
            currentResponder.OnDuded();
        }
        else
        {
            GameObject dud = Instantiate(GetDudForGameobject(hitData.transform.gameObject), hitData.point, Quaternion.identity);
            dud.transform.forward = hitData.normal;
        }

    }

    private void FixedUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            hitData = new RaycastHit();
            return;
        }
        
        Ray ray = Refs.playerCamera.cameraComponent.ScreenPointToRay(Refs.controls.Other.MousePosition.ReadValue<Vector2>());

        if (!Physics.Raycast(ray, out hitData, 1000, LayerMask.GetMask("Default", "Water", "Turtle")))
        {
            hitData = new RaycastHit();
            return;
        }

        IDudResponder newResponer = hitData.transform.gameObject.GetComponentInChildren<IDudResponder>();
        if (newResponer == null)
            newResponer = hitData.transform.gameObject.GetComponentInParent<IDudResponder>();

        if (currentResponder == newResponer)
            return;
        
        if(currentResponder != null)
            currentResponder.OnDudUnhovered();
        
        currentResponder = newResponer;
        
        if (currentResponder != null)
            currentResponder.OnDudHovered();
        
    }


    private GameObject GetDudForGameobject(GameObject target)
    {
        Renderer rend = target.GetComponentInChildren<Renderer>();
  

        switch (rend.material.name)
        {
            case "DesertRocks (Instance)":
                return dudTemplates[1];
            case "Ocean (Instance)":
            case "River (Instance)":
                return dudTemplates[2];
            case "Wood (Instance)":
            case "Wood_2 (Instance)":
            case "Rope (Instance)":
                return dudTemplates[3];
            case "Plant (Instance)":
                return dudTemplates[4];

        }
        return dudTemplates[0];
    }
}
