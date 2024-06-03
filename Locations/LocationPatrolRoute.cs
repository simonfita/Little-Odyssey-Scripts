using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationPatrolRoute : LocationBase
{

    public List<Transform> patrolStops;

    public Vector3 GetPosition(int stopCount)
    {

        return patrolStops[stopCount % patrolStops.Count].position;
    }
}
