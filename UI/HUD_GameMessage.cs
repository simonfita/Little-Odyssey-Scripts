using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class HUD_GameMessage : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    public float displayTime = 4;

    private float lifetime = 0;

    public void SetUp(string _text)
    { 
        text.text = _text;
        Destroy(gameObject,displayTime);
    }

    private void Update()
    {
        lifetime += Time.deltaTime;

        if (lifetime < displayTime / 2)
            return;
        transform.position -= new Vector3(0, Time.deltaTime*200);
    }
}
