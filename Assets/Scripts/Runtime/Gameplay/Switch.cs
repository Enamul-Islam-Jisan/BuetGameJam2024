using System;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField]
    private Sprite on;
    private SpriteRenderer sr;
    private Collider2D col;
    public event Action turnedOn;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ghost") || collision.CompareTag("Player"))
        {
            sr.sprite = on;
            turnedOn?.Invoke();
            Destroy(col);
            Destroy(this);
        }
    }
}
