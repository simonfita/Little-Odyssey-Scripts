using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AmbientZone : MonoBehaviour
{
    public TriggerCollider extents;

    public BoxCollider closestBox;

    [SerializeField]
    private AudioSource src;

    private BoxCollider extentsCollider;

    private void Awake()
    {
        extents.OnTriggerEntered += OnEnter;
        extents.OnTriggerExited += OnExit;
        extentsCollider = extents.gameObject.GetComponent<BoxCollider>();
    }

    private void Update()
    {
        src.maxDistance = (Mathf.Min(extentsCollider.size.x * transform.lossyScale.x, extentsCollider.size.z * transform.lossyScale.z) /2);

        if (src.mute)
            return;

        Vector3 playerPos = Refs.player.transform.position;

        playerPos = closestBox.transform.InverseTransformPoint(playerPos);
        
        Bounds bounds = new Bounds(closestBox.center, closestBox.size);

        Vector3 closestPoint = bounds.ClosestPoint(playerPos);

        closestPoint = closestBox.transform.TransformPoint(closestPoint);

        src.transform.position = closestPoint;

        
    }

    private void OnEnter(Collider col)
    {
        src.mute = false;
    }

    private void OnExit(Collider col)
    {
        src.mute = true;
    }
}
