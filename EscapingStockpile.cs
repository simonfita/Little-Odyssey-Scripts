using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SF;

public class EscapingStockpile : MonoBehaviour
{
    public NavMeshAgent agent;
    public Stockpile stockpile;
    public float radius;
    public float normalSpeed, fastSpeed;
    public float reactionDistance;

    private void Update()
    {
        if (!stockpile.isEmpty)
        {

            if (Vector3.Distance(Refs.player.transform.position, transform.position) > reactionDistance)
            {
                agent.speed = normalSpeed;
                if (agent.remainingDistance < agent.stoppingDistance && !agent.pathPending)
                {
                    Vector2 randomOnCirlcle = Random.insideUnitCircle.normalized * radius;

                    agent.SetDestination(transform.position + randomOnCirlcle.XZ());
                }
            }
            else
            {
                agent.speed = fastSpeed;

                Vector3 diff = transform.position - Refs.player.transform.position;
                diff.y = 0;
                diff.Normalize();
                agent.SetDestination(transform.position+diff * radius);
            }
        }
        else
        {
            agent.speed = 0;
        }
    }
}
