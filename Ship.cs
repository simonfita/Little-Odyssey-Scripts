using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (transform.localPosition.x < -4000)
            transform.localPosition += new Vector3(6000, 0, 0);
        
    }
}
