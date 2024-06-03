using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenQuips : MonoBehaviour
{
    public Token self;
    public UI_Quip quipPrefab;
    private UI_Quip quip;

    public NPC npc;

    public Sprite greetingSprite,sleepingSprite;

    public float minRandomTime, maxRandomTime;

    private void Start()
    {
        quip = Instantiate(quipPrefab);
        quip.transform.SetParent(Refs.ui.labelsCanvas, false);
        quip.GetComponent<UI_WorldToScreenLabel>().toFollow = transform;

        StartCoroutine(RandomTalking());
    }

    public void Greet()
    {
        StartCoroutine(quip.SaySpecific(1f, greetingSprite));
        StartCoroutine(self.rend.SpeechAnimation(1f));
        if (npc != null)
            npc.MakeSound();
    }


    private IEnumerator RandomTalking()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minRandomTime, maxRandomTime));

            if (npc != null && npc.asleep)
            {
                StartCoroutine(quip.SaySpecific(1f, sleepingSprite));
                npc.MakeSleepingSound();
                continue;
            }
            
            StartCoroutine(quip.SayRandom(2f));
            StartCoroutine(self.rend.SpeechAnimation(2f));
            if (npc != null)
                npc.MakeSound();
        }
    
    }



}
