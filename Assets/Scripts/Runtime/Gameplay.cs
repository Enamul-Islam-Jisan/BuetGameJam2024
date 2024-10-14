using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class Gameplay : SingletonMonoBehaviour<Gameplay>
{
    [SerializeField, Range(1,3)]
    private int maxConCurrentGhost = 1;
    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private GameObject ghostPrefab;
    [SerializeField]
    private CinemachineVirtualCamera playerCamera;
    private CinemachineConfiner2D playerCameraBoundHandler;
    private Transform ghostContainer;
    private ObjectPool<GameObject> ghostPool;
    private Queue<GameObject> activeGhosts = new Queue<GameObject>();
    [SerializeField]
    private GameObject obstacleRevealer;
    private Sequence obstacleRevealAnimation;
    public Status status { get; private set; } = Status.None;
    public Level currentLevel { get; private set; }
    private Level[] levels;
    private int currentLevelIndex;

    public static event StatusUpdateCallback statusUpdated;
    public static event LevelProgressCallback levelLoaded;

    public delegate void StatusUpdateCallback(Status status);
    public delegate void LevelProgressCallback(Level level);

    protected override void Awake()
    {
        base.Awake();
        currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel", 0);
        levels = GetComponentsInChildren<Level>(true);
        playerCameraBoundHandler = playerCamera.GetComponent<CinemachineConfiner2D>();
    }

    private void Start()
    {
        SpawnPlayer();
        LoadCurrentLevel();
    }

    private void SetupObstacleRevealAnimation()
    {
        obstacleRevealer = Instantiate(obstacleRevealer);
        obstacleRevealer.transform.localScale = Vector3.zero;
        obstacleRevealAnimation = DOTween.Sequence();
        obstacleRevealAnimation.SetAutoKill(false);
        obstacleRevealAnimation.Append(obstacleRevealer.transform.DOScale(8, 1));
        obstacleRevealAnimation.Append(obstacleRevealer.transform.DOScale(0, 0.5f));
        obstacleRevealAnimation.onComplete += () =>
        {
            UpdateStatus(Status.Failed);
            UpdateStatus(Status.Running);
            player.gameObject.SetActive(true);
        };
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            SetPause(true);
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            SetPause(false);
        }
    }
    private void UpdateStatus(Status status)
    {
        this.status = status;
        switch (status)
        {
            case Status.None:
                break;
            case Status.Started:
                player.gameObject.SetActive(true);
                break;
            case Status.Running:
                break;
            case Status.Paused:
                break;
            case Status.Failed:
                break;
        }
        statusUpdated?.Invoke(status);
    }

    private void SpawnPlayer()
    {
        ghostContainer = new GameObject("Ghosts").transform;
        ghostPool = new ObjectPool<GameObject>(() =>
        {
            GameObject playerGhost = Instantiate(ghostPrefab, ghostContainer);
            playerGhost.gameObject.SetActive(false);
            return playerGhost;
        }, (ghost) =>
        {
            ghost.gameObject.SetActive(true);
            ghost.transform.position = player.transform.position;
        }, (ghost) =>
        {
            ghost.gameObject.SetActive(false);
        }, Destroy, maxSize: 10);
        player = Instantiate(player);
        player.gameObject.SetActive(false);
        playerCamera.Follow = player.transform;
        player.died += Failed;
    }

    public void SetPause(bool pause)
    {
        if (status != Status.Paused && status != Status.Running) return;
        if (pause)
        {
            UpdateStatus(Status.Paused);
        }
        else
        {
            UpdateStatus(Status.Running);
        }
    }

    public void Failed()
    {
        player.gameObject.SetActive(false);
        if (activeGhosts.Count == maxConCurrentGhost)
        {
            ghostPool.Release(activeGhosts.Dequeue());
        }
        activeGhosts.Enqueue(ghostPool.Get());
        obstacleRevealer.transform.position = player.transform.position;
        if (obstacleRevealAnimation == null)
        {
            SetupObstacleRevealAnimation();
            obstacleRevealAnimation.Play();
        }
        else
        {
            obstacleRevealAnimation.Restart();
        }
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex == levels.Length)
        {
            SceneManager.LoadScene("End");
        }
        PlayerPrefs.SetInt("CurrentLevel", currentLevelIndex);
        Level prevLevel = levels.ElementAtOrDefault(currentLevelIndex - 1);
        if (prevLevel && prevLevel.gameObject.activeSelf)
        {
            prevLevel.gameObject.SetActive(false);
        }
        UnityEngine.Analytics.Analytics.SendEvent("levelCompleted", new Dictionary<string, object>() { { "levelCompleted", currentLevelIndex + 1 } });
        LoadCurrentLevel();
    }

    public void LoadCurrentLevel()
    {
        currentLevel = levels.ElementAtOrDefault(currentLevelIndex);
        currentLevel.gameObject.SetActive(false);
        currentLevel.gameObject.SetActive(true);
        playerCameraBoundHandler.m_BoundingShape2D = currentLevel.CameraBound;
        levelLoaded?.Invoke(currentLevel);
        UpdateStatus(Status.Started);
        UpdateStatus(Status.Running);
    }

    private void OnDestroy()
    {
        player.died -= Failed;
        obstacleRevealAnimation.Kill();
        ghostPool.Dispose();
    }

    public enum Status
    {
        None,
        Started,
        Running,
        Paused,
        Failed,
    }
}
