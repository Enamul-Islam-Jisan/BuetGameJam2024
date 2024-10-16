using DG.Tweening;
using UnityEngine;

public class GhostLifetimeElement : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 originalSizeDelta;
    private Sequence sequence;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalSizeDelta = rectTransform.sizeDelta;
        SoulSwitcher.becameGhost += SoulSwitcher_BecameGhost;
        SoulSwitcher.becamePlayer += SoulSwitcher_BecamePlayer; ;
    }

    private void SoulSwitcher_BecamePlayer()
    {
        sequence.Complete();
    }

    private void SoulSwitcher_BecameGhost()
    {
        if (sequence == null)
        {
            sequence = DOTween.Sequence();
            sequence.SetAutoKill(false);
            sequence.Append(rectTransform.DOSizeDelta(originalSizeDelta, 0));
            sequence.Append(rectTransform.DOSizeDelta(new Vector2(0, rectTransform.sizeDelta.y), Gameplay.Instance.ghostLifetime).SetEase(Ease.Linear));
            sequence.Complete();
        }

        sequence.Restart();
    }
}
