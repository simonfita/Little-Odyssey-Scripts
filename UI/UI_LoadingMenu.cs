using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_LoadingMenu : MonoBehaviour
{
    public GameObject buttonPrefab;
    public VerticalLayoutGroup verticalLayout;

    public GameObject loadingText;


    private void OnEnable()
    {
        Invoke(nameof(SetUp), 0);
    }

    private void SetUp()
    {
        for (int i = verticalLayout.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(verticalLayout.transform.GetChild(i).gameObject);
        }
        foreach (var save in SaveSystem.LoadSaveNames())
        {
            GameObject newButton = Instantiate(buttonPrefab, verticalLayout.transform);
            newButton.GetComponentInChildren<TMP_Text>().text = save;
            newButton.GetComponent<Button>().onClick.AddListener(() => {
                if (loadingText) loadingText.SetActive(true);
                SceneHandler.RestartWithSave(save);
            });
        }
    }

}
