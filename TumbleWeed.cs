using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumbleWeed : MonoBehaviour
{
    public Rigidbody rb;
    public Renderer rend;

    public float minForce, maxForce;

    private Vector3 startingPos;
    

    private void Awake()
    {
        startingPos = transform.position;
    }

    private void FixedUpdate()
    {
        rb.AddForce((-Vector3.right + Vector3.up) * Random.Range(minForce, maxForce));


        if (Vector3.Distance(transform.position, startingPos) > 400 && !rend.isVisible)
            transform.position = startingPos;
    
    }


}
