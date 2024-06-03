using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Quip : MonoBehaviour
{
    public Image bubble, icon;
    public UI_WorldToScreenLabel label;


    public Sprite[] icons;

    private void Awake()
    {
        bubble.enabled = false;
        icon.enabled = false;
    }

    public IEnumerator SayRandom(float time)
    {
        bubble.enabled = true;
        icon.enabled = true;
        icon.sprite = icons[Random.Range(0, icons.Length)];
        
        yield return new WaitForSeconds(time);
        
        bubble.enabled = false;
        icon.enabled = false;
        yield break;
    }

    public IEnumerator SaySpecific(float time, Sprite sprite)
    {
        bubble.enabled = true;
        icon.enabled = true;
        icon.sprite = sprite;

        yield return new WaitForSeconds(time);

        bubble.enabled = false;
        icon.enabled = false;
        yield break;
    }



    private void OnEnable()
    {
        bubble.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        bubble.gameObject.SetActive(false);
    }
}
