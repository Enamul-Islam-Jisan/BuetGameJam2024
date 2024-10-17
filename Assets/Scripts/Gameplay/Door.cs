using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private Sprite open;
    private SpriteRenderer sr;
    private Collider2D col;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public void Open()
    {
        StartCoroutine(OpenInteral());
    }

    private IEnumerator OpenInteral()
    {
        Gameplay.Instance.QuickLookAt(transform);
        yield return new WaitForSeconds(0.5f);
        sr.sprite = open;
        Destroy(col);
        Destroy(this);
    }
}
