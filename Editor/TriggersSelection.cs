using Codice.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class TriggerSelection
{


    [MenuItem("Util/SelectTriggers", false)]
    public static void BuildsReleaseAll()
    {
        Selection.objects = GameObject.FindObjectsOfType<Collider>().Where(x =>x.isTrigger && x.gameObject.layer != 2).Select(x =>x.gameObject).ToArray();
    }
    
 
}
