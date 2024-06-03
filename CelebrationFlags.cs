using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class CelebrationFlags : MonoBehaviour
{

#if UNITY_EDITOR
    [Range(1, 30)]
    public int flagCount = 1;

    public float flagDistance;

    // Update is called once per frame
    void Update()
    {
        if (EditorApplication.isPlaying)
            return;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (i <= flagCount-1)
            {
                transform.GetChild(i).gameObject.SetActive(true);
                transform.GetChild(i).localPosition = new Vector3(i * flagDistance, 0, 0);
            }
            else
                transform.GetChild(i).gameObject.SetActive(false);
        }
    }
#endif
}
