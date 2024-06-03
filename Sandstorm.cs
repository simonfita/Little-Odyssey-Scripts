using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Sandstorm : MonoBehaviour
{
    public static bool playerInSandstorm;

    public float speed;

    private List<TriggerCollider> children;
    private int triggerCounter;

    private void Awake()
    {
        children = new List<TriggerCollider>(GetComponentsInChildren<TriggerCollider>());
        foreach (var child in children)
        {
            if (child.transform.parent != transform)
                continue; //ambient zone fix
            child.OnTriggerEntered += (Collider _) => { triggerCounter++; UpdateVisuals(); };
            child.OnTriggerExited += (Collider _) => { triggerCounter--; UpdateVisuals(); };
        }
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (transform.position.z > 1000)
            Destroy(gameObject);
    }

    private void UpdateVisuals()
    {
        bool inSandstorm = triggerCounter > 0;
        playerInSandstorm = inSandstorm;

        if (inSandstorm)
        {
            SaveSystem.DisablingFlags.Add(SaveSystem.SaveDisablingFlag.Sandstorm);
            Shader.SetGlobalInteger("_InSandstorm", 1);
            foreach (var child in children)
                child.gameObject.GetComponent<Renderer>().enabled = false;

        }
        else
        {
            SaveSystem.DisablingFlags.Remove(SaveSystem.SaveDisablingFlag.Sandstorm);
            Shader.SetGlobalInteger("_InSandstorm", 0);
            foreach (var child in children)
                child.gameObject.GetComponent<Renderer>().enabled = true;
        }
    
    }

    private void OnDisable()
    {
        SaveSystem.DisablingFlags.Remove(SaveSystem.SaveDisablingFlag.Sandstorm);
        playerInSandstorm = false;
        Shader.SetGlobalInteger("_InSandstorm", 0);
    }
}
