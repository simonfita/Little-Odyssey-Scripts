using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Changeset : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(transform.parent.gameObject);
        GetComponent<TMPro.TMP_Text>().text = "v. " + Resources.Load<BuildData>("BuildData").changeset;
    }
}
