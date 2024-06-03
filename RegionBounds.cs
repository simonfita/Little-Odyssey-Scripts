using SF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum WorldRegion
{
    LongShore, StarRock, GrainPlains, OutOfBounds
}

public class RegionBounds : MonoBehaviour
{
    public static Dictionary<string, RegionBounds> Bounds = new Dictionary<string, RegionBounds>();
    
    public List<Transform> points;
    private List<Vector2> calculatedPoints;


    private void Awake()
    {
        if (Bounds.ContainsKey(gameObject.name))
            return;
        calculatedPoints = new List<Vector2>(points.Select(x => x.position.XZ()));
        Bounds.Add(gameObject.name,this);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < points.Count-1; i++)
        {
            Gizmos.DrawLine(points[i].position, points[i + 1].position);
        }

        Gizmos.DrawLine(points[points.Count-1].position, points[0].position); //last and first

    }




    public bool IsPlayerInside()
    {
        Vector2 playerPos = Refs.player.transform.position.XZ();
        
        return FLib.IsInPolygon(playerPos, calculatedPoints);
    }
    public static WorldRegion GetPlayerRegion()
    {
        if (RegionBounds.Bounds["CoastalBounds"].IsPlayerInside())
            return WorldRegion.LongShore;
        if (RegionBounds.Bounds["CentralBounds"].IsPlayerInside())
            return WorldRegion.StarRock;
        if (RegionBounds.Bounds["EmpireBounds"].IsPlayerInside())
            return WorldRegion.GrainPlains;

        Debug.LogWarning("Player out of regions bounds!");
        return WorldRegion.OutOfBounds;

    }

}
