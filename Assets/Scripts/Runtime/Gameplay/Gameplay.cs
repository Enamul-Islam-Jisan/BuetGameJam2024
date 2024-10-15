using Cinemachine;
using System;
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
    private Transform currentCharacterTransform;
    private bool canSwitchSoul = true;
    private Level[] levels;

    protected override void Awake()
    {
        base.Awake();
        levels = GetComponentsInChildren<Level>(true);
        playerCameraBoundHandler = followCamera.GetComponent<CinemachineConfiner2D>();
    }

    private void Start()
    {
        SpawnPlayer();
        SpawnGhost();
        LoadCurrentLevel();
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
        currentCharacterTransform = player.transform;
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
            SceneManager.LoadScene("End");
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
}
