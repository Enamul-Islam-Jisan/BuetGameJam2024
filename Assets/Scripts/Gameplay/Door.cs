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
        sr.sprite = open;
        Gameplay.Instance.QuickLookAt(transform);
        Destroy(col);
        Destroy(this);
    }
}
