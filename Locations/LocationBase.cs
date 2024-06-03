using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LocationBase : MonoBehaviour
{

    public string locationName;
    
    [SerializeField]
    protected Transform basicPosition;

    public LocationBase parentLocation;

    public virtual Vector3 GetPosition()
    {
        return basicPosition.position;
    }

}
