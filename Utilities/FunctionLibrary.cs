using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF
{
    public static class FLib
    {
        //https://stackoverflow.com/questions/39853481/is-point-inside-polygon
        public static bool IsInPolygon(Vector2 testPoint, List<Vector2> vertices)
        {
            if (vertices.Count < 3) return false;
            bool isInPolygon = false;
            var lastVertex = vertices[vertices.Count - 1];
            foreach (var vertex in vertices)
            {
                if (testPoint.y.IsBetween(lastVertex.y, vertex.y))
                {
                    double t = (testPoint.y - lastVertex.y) / (vertex.y - lastVertex.y);
                    double x = t * (vertex.x - lastVertex.x) + lastVertex.x;
                    if (x >= testPoint.x) isInPolygon = !isInPolygon;
                }
                else
                {
                    if (testPoint.y == lastVertex.y && testPoint.x < lastVertex.x && vertex.y > testPoint.y) isInPolygon = !isInPolygon;
                    if (testPoint.y == vertex.y && testPoint.x < vertex.x && lastVertex.y > testPoint.y) isInPolygon = !isInPolygon;
                }

                lastVertex = vertex;
            }

            return isInPolygon;
        }

        public static bool IsBetween(this float x, float a, float b)
        {
            return (x - a) * (x - b) < 0;
        }

        public static Vector2 XZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector3 XZ(this Vector2 v)
        {
            return new Vector3(v.x,0, v.y);
        }

        public static void ReverseForEach<TSource>(this IList<TSource> source, System.Action<TSource,int> func)
        {
            for (int i = source.Count-1; i >=0; i--)
            {
                func.Invoke(source[i],i);
            }
        }


    }
}
