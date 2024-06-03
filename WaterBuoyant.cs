using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBuoyant : MonoBehaviour
{
    public float offset;
    public bool useSea;
    public Transform sea;

    // Update is called once per frame
    void Update()
    {
        if (useSea)
        {

            transform.position = new Vector3(transform.position.x, sea.position.y + offset, transform.position.z);

            Quaternion zToUp = Quaternion.LookRotation(Vector3.up, -transform.forward);
            Quaternion yToz = Quaternion.Euler(90, 0, 0);
            transform.rotation = zToUp * yToz;

        }
        else
        {

            Ray ray = new Ray(transform.parent.position + Vector3.up * 5, Vector3.down * 10);
            Physics.Raycast(ray, out var hit, 10, LayerMask.GetMask("Water"));
            transform.position = new Vector3(transform.position.x, hit.point.y + offset, transform.position.z);

            Quaternion zToUp = Quaternion.LookRotation(hit.normal, -transform.forward);
            Quaternion yToz = Quaternion.Euler(90, 0, 0);
            
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, zToUp * yToz,Time.deltaTime*10);
        }
    }
}
