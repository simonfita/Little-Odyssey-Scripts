using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAnimal : MonoBehaviour
{
    public Animator anim;

    public bool shouldMove;
    public List<Transform> movementPoints;
    public float moveSpeed, rotationSpeed;
    public float minStayTime, maxStayTime;

    private void Awake()
    {
        if (shouldMove)
            StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (true)
        {
            Transform destination = movementPoints[Random.Range(0, movementPoints.Count)];
            while (Vector3.Distance(transform.position, destination.position) > 1)
            {
                Vector3 dir = destination.position - transform.position;

                Quaternion desiredRot = Quaternion.LookRotation(-dir,transform.up);

                dir = dir.normalized * Time.deltaTime * moveSpeed;

                transform.Translate(dir,Space.World);
                
                transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, rotationSpeed*Time.deltaTime);

                if(anim != null)
                    anim.SetFloat("Speed", 1);

                yield return new WaitForEndOfFrame();
            }

            if (anim != null)
                anim.SetFloat("Speed", 0);
            yield return new WaitForSeconds(Random.Range(minStayTime, maxStayTime));
            
        }
    }
}
