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
    [SerializeField]
    private GhostController ghost;
    [SerializeField]
    private CinemachineVirtualCamera followCamera;
    [SerializeField, Range(0, 10)]
    private float revertDuration = 1.0f;
    [SerializeField]
    private UnityEvent<float> revertTimerOnGoing;
    private float revertTimer;
    private CinemachineConfiner2D playerCameraBoundHandler;
    public Level currentLevel { get; private set; }
    private Transform currentCharacterTransform;
    private Level[] levels;
    private int currentLevelIndex;

    public static event LevelProgressCallback levelLoaded;
    public static event Action characterReverted;

    public delegate void LevelProgressCallback(Level level);

    [SerializeField]
    private RectTransform orbImageContainer;
    [SerializeField]
    private Image orbImagePrefab;

    private List<Sprite> orbSprites = new List<Sprite>();

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
    private void Update()
    {
        if (revertTimer > 0)
        {
            revertTimer -= Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.R))
            {
                revertTimer = 0;
            }

            revertTimerOnGoing?.Invoke(revertTimer / revertDuration);
            if (revertTimer <= 0)
            {
                SwitchCharacter();
            }
        }
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
        if (hitCollider.CompareTag("Obstacle"))
        {
            SwitchCharacter();
        }else if (hitCollider.CompareTag("Orb"))
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


    private void SwitchCharacter()
    {
        Debug.Log(currentCharacterTransform);
        if (currentCharacterTransform == ghost.transform)
        {
            ghost.gameObject.SetActive(false);
            player.gameObject.SetActive(true);
            player.transform.position = ghost.transform.position;
            currentCharacterTransform = player.transform;
            characterReverted?.Invoke();
        }
        else if (currentCharacterTransform == player.transform)
        {
            player.gameObject.SetActive(false);
            ghost.gameObject.SetActive(true);
            ghost.transform.position = player.transform.position;
            currentCharacterTransform = ghost.transform;
            revertTimer = revertDuration;
        }
        followCamera.Follow = currentCharacterTransform;
    }

    private void AddOrb(Sprite sprite)
    {
        if (orbSprites.Contains(sprite)) return;
        orbSprites.Add(sprite);
        Image orbImage = Instantiate(orbImagePrefab, orbImageContainer);
        orbImage.sprite = sprite;
    }
}
