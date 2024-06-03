using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamStar : MonoBehaviour
{
    public RectTransform rectTransform;
    
    [SerializeField]
    private Rigidbody2D rb;

    public DreamMinigame minigame;

    public System.Action OnPlayerHit;

    public void Restart(float difficulty)
    {
        rectTransform.anchoredPosition = new Vector2(Random.Range(-400, 400), 0)+ Vector2.up*50;
        rb.velocity = Vector2.zero;

        Vector2 dir = new Vector2(Random.Range(-0.3f, 0.3f), -1);
        dir.Normalize();

        dir *= difficulty*100;

        rb.AddForce(dir);

    }

    private void Update()
    {
        if (transform.localPosition.y < -1100)
            Restart(minigame.difficulty);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            OnPlayerHit.Invoke();

    }
}
