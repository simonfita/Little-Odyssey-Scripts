using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_Compass : MonoBehaviour
{
  

    void Update()
    {
        Vector3 cameraPos = Refs.playerCamera.transform.forward;
        cameraPos.y = 0;

        float rot = -Vector3.SignedAngle(cameraPos, Vector3.back,Vector3.up);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0,rot)); 
    }
}
