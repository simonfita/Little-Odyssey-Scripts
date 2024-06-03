using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Orich : MonoBehaviour
{
    public NavMeshAgent agent;
    public TokenSlot slot;
    public Animator anim;

    private void Update()
    {
        anim.SetBool("isWalking", agent.velocity.sqrMagnitude > 0.1f);
    }


    public void GoTo(Vector3 dest)
    {
        agent.SetDestination(dest);
    }

    public bool ReachedDestination()
    {
        return agent.remainingDistance < agent.stoppingDistance && !agent.pathPending;
    }
}
