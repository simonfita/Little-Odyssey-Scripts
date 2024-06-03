using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Barmetler.Bezier;

public class CoinSpawner : MonoBehaviour
{
    public LostCoin coinPrefab;
    public int coinAmount;

    public float minRoadDist, maxRoadDist;

    //postion, forward
    private List<(Vector3,Vector3)> points = new List<(Vector3, Vector3)> ();

    private void Start()
    {
        foreach (Road road in FindObjectsOfType<Road>())
        {
            if (!road.gameObject.CompareTag("Road"))
                continue;
            foreach (OrientedPoint point in road.GetEvenlySpacedPoints(3))
                points.Add((road.transform.TransformPoint(point.position), road.transform.TransformDirection(point.forward)));
        }

        for (int i = 0; i < coinAmount; i++)
        {
            LostCoin newCoin =  Instantiate(coinPrefab, GenerateRandomPosition(), Random.rotation, transform);
            newCoin.OnCollected += OnCoinCollected;
        }

    }

    private void OnCoinCollected(LostCoin coin)
    { 
        coin.transform.position = GenerateRandomPosition();
    }

    private Vector3 GenerateRandomPosition()
    {
        (Vector3, Vector3) randomPoint = points[Random.Range(0, points.Count)];

        float randomDistance = Random.Range(minRoadDist, maxRoadDist);
        if (Random.Range(0, 2) == 1)
            randomDistance *= -1;

        Vector3 offset = Vector3.Cross(randomPoint.Item2, Vector3.up) * randomDistance;

        Vector3 randomPosition = randomPoint.Item1 + offset;

        Ray ray = new Ray(randomPosition + Vector3.up * 4, Vector3.down);

        if(Physics.Raycast(ray, out RaycastHit hit, 8))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                return hit.point;
            }
        }
        return GenerateRandomPosition();//no terrain found, try again

    }

}
