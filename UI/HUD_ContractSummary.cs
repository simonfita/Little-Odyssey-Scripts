using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_ContractSummary : MonoBehaviour
{
    public Animator anim;
    public Transform coinParent;

    public AudioSource completionSrc, stampSrc;

    private Money coinsToShow;

    public void ShowReward(ContractInstanceBase contract)
    {
        coinsToShow = new Money(Mathf.Min(contract.GetReward(),20));

        for (int i = 0; i < coinParent.childCount; i++)
        {
            coinParent.GetChild(i).gameObject.SetActive(false);
        }
        anim.SetTrigger("Show");

    }

    public void PlayCompletionSound()
    {
        completionSrc.Play();
    }

    public void PlayStampSound()
    {
        stampSrc.Play();
    }

    public void ShowCoins()
    {
        StartCoroutine(ShowCoins(coinsToShow));
    }

    private IEnumerator ShowCoins(Money amout)
    { 

        for (int i = 0; i < amout.bronze; i++)
        {
            coinParent.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(.5f - i*0.01f);
        }

        yield break;
    }

}
