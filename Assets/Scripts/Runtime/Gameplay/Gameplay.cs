using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gameplay : SingletonMonoBehaviour<Gameplay>
{
    [field: SerializeField]
    public PlayerController player { get; private set; }
    [field: SerializeField]
    public GhostController ghost { get; private set; }
    [SerializeField]
    private CinemachineVirtualCamera followCamera;
    [SerializeField, Range(0, 5)]
    private float quickLookAtTime;
    private CinemachineConfiner2D playerCameraBoundHandler;

    public static event LevelProgressCallback levelLoaded;

    public delegate void LevelProgressCallback(Level level);

    [SerializeField]
    private RectTransform orbImageContainer;
    [SerializeField]
    private Image orbImagePrefab;
    [field:SerializeField, Range(0,5)]
    public float ghostLifetime { get; private set; }

    private List<Sprite> orbSprites = new List<Sprite>();
    [SerializeField]
    private int currentLevelIndex;
    public Level currentLevel { get; private set; }
    private Level[] levels;
    private Coroutine quickLookAtCoroutine;

    protected override void Awake()
    {
        base.Awake();
        levels = GetComponentsInChildren<Level>(true);
        playerCameraBoundHandler = followCamera.GetComponent<CinemachineConfiner2D>();

        SoulSwitcher.becameGhost += SoulSwitcher_becameGhost;
        SoulSwitcher.becamePlayer += SoulSwitcher_becameGhost;
    }

    private void SoulSwitcher_becameGhost()
    {
        followCamera.Follow = SoulSwitcher.currentCharacterTransform;
    }

    private void Start()
    {
        SpawnPlayer();
        SpawnGhost();
        LoadCurrentLevel();
    }

    private void Update()
    {
        if(/*endOfGameReached && orbSprites.Count == 5 && */Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(SacrificeSoul());
        }
    }
    private IEnumerator SacrificeSoul()
    {
        player.animator.Play("Reborn");
        orbSprites.Clear();
        Destroy(orbImageContainer.gameObject);
        player.enabled = false;
        ghost.enabled = false;
        yield return new WaitUntil(() => !player.animator.enabled);
        SceneManager.LoadScene("End");
    }
    private void SpawnGhost()
    {
        ghost = Instantiate(ghost);
        ghost.gameObject.SetActive(false);
        followCamera.Follow = player.transform;
    }

    private void SpawnPlayer()
    {
        player = Instantiate(player);
        player.gameObject.SetActive(true);
        followCamera.Follow = player.transform;
        player.onHit += Player_OnHit;
    }

    private void Player_OnHit(Collider2D hitCollider)
    {
        if (hitCollider.CompareTag("Orb"))
        {
            Sprite orbSprite = hitCollider.GetComponent<SpriteRenderer>().sprite;
            AddOrb(orbSprite);
        }
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex == levels.Length)
        {
            player.enabled = false;
            ghost.enabled = false;
            return;
        }
        Level prevLevel = levels.ElementAtOrDefault(currentLevelIndex - 1);
        if (prevLevel && prevLevel.gameObject.activeSelf)
        {
            prevLevel.gameObject.SetActive(false);
        }
        LoadCurrentLevel();
    }

    public void LoadCurrentLevel()
    {
        currentLevel = levels.ElementAtOrDefault(currentLevelIndex);
        if (!currentLevel) return;
        currentLevel.gameObject.SetActive(false);
        currentLevel.gameObject.SetActive(true);
        playerCameraBoundHandler.m_BoundingShape2D = currentLevel.CameraBound;
        levelLoaded?.Invoke(currentLevel);
    }

    private void AddOrb(Sprite sprite)
    {
        if (orbSprites.Contains(sprite)) return;
        orbSprites.Add(sprite);
        Image orbImage = Instantiate(orbImagePrefab, orbImageContainer);
        orbImage.sprite = sprite;
    }

    public void QuickLookAt(Transform target)
    {
        QuickLookAt(target, 0,-1);
    }

    public void QuickLookAt(Transform target, float startDelay = 0, float overrideDuration = -1)
    {
        if (quickLookAtCoroutine != null)
        {
            StopCoroutine(quickLookAtCoroutine);
        }
        quickLookAtCoroutine = StartCoroutine(QuickLookAtInternal(target, startDelay, overrideDuration));
    }


    private IEnumerator QuickLookAtInternal(Transform target, float startDelay = 0, float overrideDuration = -1)
    {
        yield return new WaitForSeconds(startDelay);
        followCamera.Follow = target;
        yield return new WaitForSeconds(((overrideDuration == -1) ? quickLookAtTime : overrideDuration));
        followCamera.Follow = SoulSwitcher.currentCharacterTransform;
    }


    private void OnDestroy()
    {
        SoulSwitcher.becameGhost -= SoulSwitcher_becameGhost;
        SoulSwitcher.becamePlayer -= SoulSwitcher_becameGhost;
    }
}
