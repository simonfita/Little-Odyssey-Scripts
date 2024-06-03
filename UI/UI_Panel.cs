using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Panel : MonoBehaviour
{
    public UI_PanelType type;

    private void Update()
    {

        if (Mathf.Abs(transform.localPosition.x) < 1 && Mathf.Abs(transform.localPosition.y) < 1)
            return;

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, 5000*Time.deltaTime);
    }

    //TODO: Use
    public virtual void Initialize()
    { 
    
    }
}
