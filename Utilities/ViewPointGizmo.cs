using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPointGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawFrustum(Vector3.zero, 40, 30, 10,1);
    }
}
