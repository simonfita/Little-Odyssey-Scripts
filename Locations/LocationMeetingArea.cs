using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationMeetingArea : LocationBase
{
    public float size = 10;

    public override Vector3 GetPosition()
    {
        Vector3 randomPoint = Random.insideUnitSphere * size;
        randomPoint.y = 0;
        return basicPosition.position + randomPoint;
    
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, size);
    }
}
