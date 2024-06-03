using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip sound;

    private Button butt;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!butt.interactable)
            return;
        Sounds.PlayPlayerSound(sound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!butt.interactable)
            return;
        Sounds.PlayPlayerSound(sound);
    }

    private void Awake()
    {
        butt = GetComponent<Button>();


        
    }

}
