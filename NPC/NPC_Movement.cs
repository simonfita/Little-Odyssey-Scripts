using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using SF;

public class NPC_Movement : MonoBehaviour
{
    public NavMeshAgent agent;
    public Token token;

   
    public  Action OnLocationReached;
    public  Action OnLocationNotReached; //TODO: not yet used

    public Vector3 currentDestination;

    public bool isMoving = false;
    public bool reachedLocation = true;

    private bool fixUpRotation;

    private void OnEnable()
    {
        agent.enabled = true;
    }

    private void OnDisable()
    {
        agent.enabled = false;
    }

    public bool GoTo(Vector3 destination, bool _fixUpRotation = false)
    {
        //token.StartMovementAnimation();
        reachedLocation = false;
        currentDestination = destination;

        fixUpRotation = _fixUpRotation;
        isMoving = true;
        if (!agent.SetDestination(destination))
        {
            Debug.LogError(gameObject.name + " can't reach destination " + destination);
            return false;
        }
        return true;
    }

    public bool Teleport(Vector3 position)
    { 
        return agent.Warp(position);
    }

    private void Update()
    {

        agent.isStopped = !token.canMove;

        if (isMoving&& agent.remainingDistance < agent.stoppingDistance&&!agent.pathPending)
        {
            

            isMoving = false;

            //agent.ResetPath();
            //token.StopMovementAnimation();
            reachedLocation = true;
            if (fixUpRotation)
            {
                Vector3 toCamera = (Refs.playerCamera.transform.position - transform.position).XZ().XZ().normalized;
                agent.SetDestination(transform.position + toCamera * agent.stoppingDistance);
                fixUpRotation=false;

            }
            OnLocationReached?.Invoke();
        }
      
    }
}
