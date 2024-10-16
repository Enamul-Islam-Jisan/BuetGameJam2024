using System;
using UnityEngine;

public class SoulSwitcher : MonoBehaviour
{
    private PlayerController player;
    private GhostController ghost;
    private Level selfLevel;
    private static SoulSwitcher current;
    private static SoulSwitcher currentCollided;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Player") || collision.CompareTag("Ghost")) && collision.transform == currentCharacterTransform)
        {
            currentCollided = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((collision.CompareTag("Player") || collision.CompareTag("Ghost")) && collision.transform == currentCharacterTransform)
        {
            if (currentCollided  && currentCollided == this)
            {
                currentCollided = null;
            }
        }
    }

    private void ToGhost()
    {
        if (current) return;
        if (currentCharacterTransform == ghost.transform) return;
        ghost.transform.position = player.transform.position;
        player.gameObject.SetActive(false);
        ghost.gameObject.SetActive(true);
        current = this;
        currentCharacterTransform = ghost.transform;
        becameGhost?.Invoke();
        Invoke(nameof(ToPlayer), Gameplay.Instance.ghostLifetime);
    }

    private void ToPlayer()
    {
        if (currentCharacterTransform == player.transform) return;
        CancelInvoke(nameof(ToPlayer));
        Transform currentTransform = (currentCollided) ? currentCollided.transform : current.transform;
        Vector3 spawnPosition = currentTransform.position;
        spawnPosition.y = currentTransform.position.y + 0.25f;
        player.transform.position = spawnPosition;

        ghost.gameObject.SetActive(false);
        player.gameObject.SetActive(true);
        current = null;
        currentCharacterTransform = player.transform;
        becamePlayer?.Invoke();
    }
}
