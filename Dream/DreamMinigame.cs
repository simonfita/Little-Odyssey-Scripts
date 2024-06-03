using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DreamMinigame : MonoBehaviour
{
    public DreamStar starPrefab;
    public GameObject starsParent;

    public float newStarFrequency;
    private float newStarTimer;

    public float difficulty;
    public float difficultyIncreaseSpeed;

    public System.Action<int> OnMinigameLost;




    private void Update()
    {
        newStarTimer -= Time.deltaTime;

        if (newStarTimer <= 0) {
            newStarTimer = newStarFrequency;
        
            DreamStar newStar = Instantiate(starPrefab,starsParent.transform);
            newStar.minigame = this;
            newStar.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            newStar.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            newStar.Restart(difficulty);

            newStar.OnPlayerHit += OnPlayerHit;

        }

        difficulty += difficultyIncreaseSpeed * Time.deltaTime;


    }

    private void OnPlayerHit()
    {

        OnMinigameLost?.Invoke(0);
    }
}
