using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hydration : SaveableBehaviour
{
    public float maxHydration;
    public float dehydrationSpeed;

    public float currentHydration;

    private Turtle mount;

    override protected void Awake()
    {
        base.Awake();

        currentHydration = maxHydration;
        mount = GetComponent<Turtle>();
        StartCoroutine(MessegeRoutine());
    }

    private void Update()
    {
        currentHydration = Mathf.Max(currentHydration +(mount.controller.velocity.magnitude * -dehydrationSpeed*Time.deltaTime),0);
    }

    public override void OnSave(Save save)
    {
        save.hydration = currentHydration;
    }
    
    public override void OnLoad(Save save)
    {
        currentHydration = save.hydration;
    }

    private IEnumerator MessegeRoutine()
    {
        while (true)
        {
            if (currentHydration / maxHydration < 0.1f)
            {
                Refs.ui.hud.SpawnGameMessege("I have to refill the water at the well!");
            }
            yield return new WaitForSeconds(10f);
        }
    }
}
