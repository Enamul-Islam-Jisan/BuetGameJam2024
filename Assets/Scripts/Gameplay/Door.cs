using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer sr;
    private Collider2D col;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    public void Open()
    {
        StartCoroutine(OpenInteral());
    }

    private IEnumerator OpenInteral()
    {
        Gameplay.Instance.QuickLookAt(transform);
        yield return new WaitForSeconds(0.5f);
        animator.enabled = true;
        col.enabled = false;
        yield return new WaitForSeconds(0.1f);
        AudioSource.PlayClipAtPoint(Gameplay.Instance.doorClip, Vector3.zero, 0.35f);
        Destroy(col);
        Destroy(this);
    }
}
