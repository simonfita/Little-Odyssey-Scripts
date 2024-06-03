using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBird : MonoBehaviour
{

    public float speed,angle;

    void Update()
    {
        transform.position += transform.forward * speed*Time.deltaTime;
        transform.Rotate(0, angle * Time.deltaTime, 0);
    }

}
