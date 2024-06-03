using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LostAnimal : MonoBehaviour
{
    public float maxDistance;

    private NavMeshAgent agent;
    private Goods goods;

    private Vector3 startingPosition;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        goods = GetComponent<Goods>();

        startingPosition = transform.position;

        WalkRandom();

    }

    private void Update()
    {
        if (Refs.player.token.heldGoods == goods)
        {
            agent.enabled = false;
        }

    }

    private void WalkRandom()
    {
        if (!agent.enabled)
            return;

        agent.SetDestination(startingPosition+Random.insideUnitSphere*maxDistance);

        Invoke(nameof(WalkRandom), Random.Range(1, 10));
    }

}
