using DG.Tweening;
using System;
using UnityEngine;

public class SoulSwitcher : MonoBehaviour
{
    private PlayerController player;
    private GhostController ghost;
    private Level selfLevel;
    private static SoulSwitcher current; 
    public static SoulSwitcher currentCollided { get; private set; }

    public static Transform currentCharacterTransform { get; private set; }
    public static event Action becameGhost;
    public static event Action becamePlayer;

    private void Awake()
    {
        selfLevel = GetComponentInParent<Level>();
        Gameplay.levelLoaded += Gameplay_levelLoaded;
    }

    private void Gameplay_levelLoaded(Level level)
    {
        player = Gameplay.Instance.player;
        ghost = Gameplay.Instance.ghost;
        currentCharacterTransform = player.transform;
        Gameplay.levelLoaded -= Gameplay_levelLoaded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!current)
            {
                if (currentCollided)
                {
                    if (currentCharacterTransform == player.transform && currentCollided == this)
                    {
                        currentCollided.ToGhost();
                    }
                }
            }
            else
            {
                if(current == this)
                {
                    current.ToPlayer();
                }
            }
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (currentCollided == this) return;
        if ((collision.CompareTag("Player") || collision.CompareTag("Ghost")) && currentCharacterTransform && collision.transform == currentCharacterTransform.transform)
        {
            currentCollided = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (currentCollided != this) return;
        if ((collision.CompareTag("Player") || collision.CompareTag("Ghost")))
        {
            currentCollided = null;
        }
    }

    private void ToGhost()
    {
        if (current) return;
        if (currentCharacterTransform == ghost.transform) return;
        currentCharacterTransform = null;
        ghost.transform.position = player.transform.position;
        AudioSource.PlayClipAtPoint(Gameplay.Instance.soulSwitchClip, Vector3.zero);

        player.transform.DOScale(0, .25f).SetEase(Ease.InOutCubic).onComplete += () =>
        {
            player.gameObject.SetActive(false);
            ghost.gameObject.SetActive(true);
            ghost.transform.DOScale(0, 0);
            ghost.transform.DOScale(1, .25f).SetEase(Ease.InOutCubic).onComplete += () =>
            {
                current = this;
                currentCharacterTransform = ghost.transform;
                becameGhost?.Invoke();
                Invoke(nameof(ToPlayer), Gameplay.Instance.ghostLifetime);
            };
        };
    }

    private void ToPlayer()
    {
        if (currentCharacterTransform == player.transform) return;
        CancelInvoke(nameof(ToPlayer));
        currentCharacterTransform = null;
        AudioSource.PlayClipAtPoint(Gameplay.Instance.soulSwitchClip, Vector3.zero);
        Transform currentTransform = (currentCollided) ? ghost.transform : current.transform;
        Vector3 spawnPosition = currentTransform.position;
        spawnPosition.y = currentTransform.position.y + 0.25f;
        player.transform.position = spawnPosition;
        ghost.transform.DOScale(0, .25f).SetEase(Ease.InOutCubic).onComplete += () =>
        {
            ghost.gameObject.SetActive(false);
            player.gameObject.SetActive(true);
            player.transform.DOScale(0, 0);
            player.transform.DOScale(1, .25f).SetEase(Ease.InOutCubic).onComplete += () =>
            {
                current = null;
                currentCharacterTransform = player.transform;
                becamePlayer?.Invoke();
            };
        };;
    }
}
